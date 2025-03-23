using Microsoft.EntityFrameworkCore;
using Prn212.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using OfficeOpenXml;

namespace Prn212
{
    /// <summary>
    /// Interaction logic for ManageMember.xaml
    /// </summary>
    public partial class ManageMember : UserControl
    {
        private readonly Prn212Context _context;
        private User _currentUser;
        public ManageMember(User user)
        {
            _context = new Prn212Context();
            InitializeComponent();
            _currentUser = user;
            loadData();
        }

        public void loadData()
        {
            var data = _context.HouseholdMembers
                .Include(c => c.Household)
                .Include(c => c.User)
                .Where(c => c.Relationship == "Chủ hộ")
                .ToList();
            ResidentsDataGrid.ItemsSource = data;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Resource.Resource.saveLog(_currentUser.UserId, Resource.ConstLog.HOUSE_HOLD_MEMBER_ADD, _context);
            var selectItem = ResidentsDataGrid.SelectedItem as HouseholdMember;
            if (selectItem != null)
            {
                AddMember addMember = new AddMember(_currentUser);
                addMember.houseHoldId = (int)selectItem.HouseholdId;
                addMember.ShowDialog();
                loadData();
            }
            else
            {
                MessageBox.Show("Vui lòng chọn 1 hộ dân");
            }
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            Resource.Resource.saveLog(_currentUser.UserId, Resource.ConstLog.HOUSE_HOLD_MEMBER_UPDATE, _context);

            var selectItem = ResidentsDataGrid.SelectedItem as HouseholdMember;

            if (selectItem != null)
            {
                int memberId = (int)selectItem.MemberId;
                UpdateMember updateMember = new UpdateMember(memberId, _currentUser);
                updateMember.ShowDialog();
                loadData();
            }
            else
            {
                MessageBox.Show("Vui lòng chọn 1 hộ dân");
            }
        }

        //Hàm này sẽ xóa tất cả thông tin liên quan đến chủ hộ, cả về thông tin hộ gia đình, thành viên gia đình, nhưng sẽ dữ lại dữ liệu của chủ hộ để 
        //lưu thông tin truy vấn sau này
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Resource.Resource.saveLog(_currentUser.UserId, Resource.ConstLog.HOUSE_HOLD_MEMBER_DELETE, _context);

            MessageBoxResult result = MessageBox.Show(
                "Có chắc chắn xóa hộ gia đình này?",
                "Xóa",
                MessageBoxButton.OKCancel,
                MessageBoxImage.Warning
            );

            if (result == MessageBoxResult.OK)
            {
                var selectItem = ResidentsDataGrid.SelectedItem as HouseholdMember;

                if (selectItem != null)
                {
                    
                    var household = _context.Households
                        .FirstOrDefault(h => h.HeadOfHouseholdId == selectItem.UserId);

                    if (household != null)
                    {
                        
                        var householdMembers = _context.HouseholdMembers
                            .Where(hm => hm.HouseholdId == household.HouseholdId)
                            .ToList();

                        
                        var userIds = householdMembers
                            .Where(hm => hm.UserId != household.HeadOfHouseholdId)
                            .Select(hm => hm.UserId)
                            .ToList();

                        var usersToDelete = _context.Users
                            .Where(u => userIds.Contains(u.UserId))
                            .ToList();

                        var headOfHousehold = _context.Users
                            .FirstOrDefault(u => u.UserId == selectItem.UserId);
                        int count = _context.Users.Count();
                        if (headOfHousehold != null)
                        {
                            headOfHousehold.Email = $"user{count}@gmail.com";
                            headOfHousehold.Password = "default" ;
                        }                       
                        _context.HouseholdMembers.RemoveRange(householdMembers); 
                        _context.Users.RemoveRange(usersToDelete); 
                        _context.Households.Remove(household); 
                        _context.SaveChanges();

                        Resource.Resource.saveLog(_currentUser.UserId, Resource.ConstLog.HOUSE_HOLD_MEMBER_DELETE_SUCCESS, _context);

                        MessageBox.Show("Đã xóa hộ khấu");
                        loadData();
                    }
                    else
                    {
                        MessageBox.Show("Có lỗi xảy ra vui lòng thử lại");
                    }
                }
                else
                {
                    MessageBox.Show("Hãy chọn một hộ gia đình");
                }
            }
            else
            {
                Resource.Resource.saveLog(_currentUser.UserId, Resource.ConstLog.HOUSE_HOLD_MEMBER_DELETE_FAIL, _context);
            }
        }




        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            String txtSearch = SearchBox.Text;
            var data = _context.HouseholdMembers
                .Include(c => c.Household)
                .Include(c => c.User)
                .ToList();
            if (txtSearch != null)
            {
                data = data.Where(c => c.User.FullName.Contains(txtSearch)).ToList();
            }
            ResidentsDataGrid.ItemsSource = data;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var residents = ResidentsDataGrid.ItemsSource as List<HouseholdMember>;
            if (residents == null || residents.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để xuất!");
                return;
            }

            string folderPath = "C:\\FPT\\semester 7\\PRN212\\Project";
            string fileName = $"Export_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            string filePath = System.IO.Path.Combine(folderPath, fileName);

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");

                // Ghi tiêu đề
                worksheet.Cells[1, 1].Value = "ID";
                worksheet.Cells[1, 2].Value = "Full Name";
                worksheet.Cells[1, 3].Value = "Address";
                worksheet.Cells[1, 4].Value = "Head of Household";
                worksheet.Cells[1, 5].Value = "Relationship";

                // Ghi dữ liệu từ DataGrid
                int row = 2;
                foreach (var resident in residents)
                {
                    worksheet.Cells[row, 1].Value = resident.MemberId;
                    worksheet.Cells[row, 2].Value = resident.User.FullName;
                    worksheet.Cells[row, 3].Value = resident.User.Address;
                    worksheet.Cells[row, 4].Value = resident.Household.HeadOfHousehold.FullName;
                    worksheet.Cells[row, 5].Value = resident.Relationship;
                    row++;
                }

                // Lưu file Excel
                File.WriteAllBytes(filePath, package.GetAsByteArray());
                MessageBox.Show($"Xuất file Excel thành công: {filePath}");
            }
        }

        private void ButtonView_Click(object sender, RoutedEventArgs e)
        {
            if (ResidentsDataGrid.SelectedItem is HouseholdMember selectedHousehold)
            {
                CitizenHouseHoldDetail citizenHhDetail = new CitizenHouseHoldDetail(selectedHousehold.User);
                citizenHhDetail.Show();
            }
        }
    }
}
