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
using Prn212.Models;

namespace Prn212
{
    
    public partial class CitizenHouseHoldDetail : Window
    {
        private Prn212Context _context = new Prn212Context();
        private User _currentUser;
        public CitizenHouseHoldDetail(User user)
        {
            _currentUser = user;
            InitializeComponent();
            LoadHouseHoldData();
        }
        private void LoadHouseHoldData()
        {
            var houseHold = _context.Households
                .FirstOrDefault(h => h.HeadOfHouseholdId == _currentUser.UserId);

            if (houseHold == null)
            {
                var householdMember = _context.HouseholdMembers
                    .FirstOrDefault(m => m.UserId == _currentUser.UserId);

                if (householdMember != null)
                {
                    houseHold = _context.Households
                        .FirstOrDefault(h => h.HouseholdId == householdMember.HouseholdId);
                }
            }

            if (houseHold != null)
            {
                txtHouseHoldId.Text = houseHold.HouseholdId.ToString();
                txtAddress.Text = houseHold.Address;
                txtCreatedDate.Text = houseHold.CreatedDate?.ToString("dd/MM/yyyy") ?? "N/A";
                txtHeadOfHouseHold.Text = _context.Users
                    .Where(u => u.UserId == houseHold.HeadOfHouseholdId)
                    .Select(u => u.FullName)
                    .FirstOrDefault() ?? "N/A";

                var members = _context.HouseholdMembers
                    .Where(m => m.HouseholdId == houseHold.HouseholdId)
                    .Select(m => new
                    {
                        m.MemberId,
                        UserName = _context.Users.Where(u => u.UserId == m.UserId).Select(u => u.FullName).FirstOrDefault(),
                        m.Relationship
                    })
                    .ToList();

                memberDataGrid.ItemsSource = null;
                memberDataGrid.ItemsSource = members;
            }
            else
            {
                MessageBox.Show("Không tìm thấy hộ gia đình của bạn!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
