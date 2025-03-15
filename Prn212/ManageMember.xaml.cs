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
using System.Windows.Shapes;

namespace Prn212
{
    /// <summary>
    /// Interaction logic for ManageMember.xaml
    /// </summary>
    public partial class ManageMember : UserControl
    {
        private readonly Prn212Context _context;
        public ManageMember()
        {
            _context = new Prn212Context();
            InitializeComponent();
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
            Resource.Resoure.saveLog(Resource.Resoure.getUserId(), Resource.ConstLog.HOUSE_HOLD_MEMBER_ADD, _context);
            var selectItem = ResidentsDataGrid.SelectedItem as HouseholdMember;
            if (selectItem != null)
            {
                AddMember addMember = new AddMember();
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
            Resource.Resoure.saveLog(Resource.Resoure.getUserId(), Resource.ConstLog.HOUSE_HOLD_MEMBER_UPDATE, _context);

            var selectItem = ResidentsDataGrid.SelectedItem as HouseholdMember;

            if (selectItem != null)
            {
                int memberId = (int)selectItem.MemberId;
                UpdateMember updateMember = new UpdateMember(memberId);
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
            Resource.Resoure.saveLog(Resource.Resoure.getUserId(), Resource.ConstLog.HOUSE_HOLD_MEMBER_DELETE, _context);

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

                    Resource.Resoure.saveLog(Resource.Resoure.getUserId(), Resource.ConstLog.HOUSE_HOLD_MEMBER_DELETE_SUCCESS, _context);

                    MessageBox.Show("Delete Successfull");
                    loadData();
                }
                else
                {
                    Resource.Resoure.saveLog(Resource.Resoure.getUserId(), Resource.ConstLog.HOUSE_HOLD_MEMBER_DELETE_FAIL, _context);

                    MessageBox.Show("Please choose one house hold");
                }
            }
            else
            {
                Resource.Resoure.saveLog(Resource.Resoure.getUserId(), Resource.ConstLog.HOUSE_HOLD_MEMBER_DELETE_FAIL, _context);

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
    }
}
