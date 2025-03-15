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
using Microsoft.EntityFrameworkCore;
using Prn212.Models;

namespace Prn212
{
    public partial class ViewRegistrationDetail : Window
    {
        private int _registrationDetailId;

        public ViewRegistrationDetail(int registrationDetailId)
        {
            InitializeComponent();
            _registrationDetailId = registrationDetailId;
            LoadRegistrationDetail();
        }

        private void LoadRegistrationDetail()
        {
            using (var context = new Prn212Context())
            {
                var detail = context.RegistrationDetails
                    .Include(rd => rd.Household)
                    .ThenInclude(h => h.HouseholdMembers)
                    .FirstOrDefault(rd => rd.RegistrationDetailId == _registrationDetailId);

                if (detail != null)
                {
                    txtVerifyingIdentity.Text = detail.VerifyingIdentity;
                    txtVerifyingResidence.Text = detail.VerifyingResidence;
                    txtHouseholdAddress.Text = detail.Household?.Address ?? "Không có";
                    lstHouseholdMembers.ItemsSource = detail.Household?.HouseholdMembers
                        .Select(hm => $"{hm.User.FullName} - {hm.Relationship}")
                        .ToList();
                }
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

}
