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
                MessageBox.Show("Please choose one house hold");
            }
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            Resource.Resource.saveLog(_currentUser.UserId, Resource.ConstLog.HOUSE_HOLD_MEMBER_UPDATE, _context);

            var selectItem = ResidentsDataGrid.SelectedItem as HouseholdMember;

            if (selectItem != null)
            {
                int memberId = (int)selectItem.MemberId;
                UpdateMember updateMember = new UpdateMember(memberId,_currentUser);
//                updateMember.ShowDialog();
                loadData();
            }
            else
            {
                MessageBox.Show("Please choose one house hold");
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Resource.Resource.saveLog(_currentUser.UserId, Resource.ConstLog.HOUSE_HOLD_MEMBER_DELETE, _context);

            MessageBoxResult result = MessageBox.Show(
                "Are you sure to delete?",
                "Delete",
                MessageBoxButton.OKCancel,
                MessageBoxImage.Warning
            );
            if (result == MessageBoxResult.OK)
            {
                var selectItem = ResidentsDataGrid.SelectedItem as HouseholdMember;
                if (selectItem != null)
                {
                    _context.Remove(selectItem);
                    _context.SaveChanges();

                    Resource.Resource.saveLog(_currentUser.UserId, Resource.ConstLog.HOUSE_HOLD_MEMBER_DELETE_SUCCESS, _context);

                    MessageBox.Show("Delete Successfull");
                    loadData();
                }
                else
                {
                    Resource.Resource.saveLog(_currentUser.UserId, Resource.ConstLog.HOUSE_HOLD_MEMBER_DELETE_FAIL, _context);

                    MessageBox.Show("Please choose one house hold");
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
    }
}
