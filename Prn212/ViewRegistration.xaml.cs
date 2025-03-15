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
    public partial class ViewRegistration : Window
    {
        private User _currentUser;
        public ViewRegistration(User user)
        {
            InitializeComponent();
            _currentUser = user;
            LoadRegistrations();
        }

        private void LoadRegistrations()
        {
            using (var context = new Prn212Context())
            {
                var registrations = context.Registrations
                    .Select(r => new
                    {
                        r.RegistrationType,
                        r.StartDate,
                        r.EndDate,
                        r.Status,
                        ApprovedBy = r.ApprovedByNavigation.FullName,
                        r.Comments,
                        r.RegistrationDetailId
                    }).ToList();

                registrationDataGrid.ItemsSource = registrations;
            }
        }

        private void BtnViewDetail_Click(object sender, RoutedEventArgs e)
        {
            if (registrationDataGrid.SelectedItem is Registration selectedRegistration)
            {
                int registrationDetailId = selectedRegistration.RegistrationDetailId ?? 0;
                ViewRegistrationDetail detailWindow = new ViewRegistrationDetail(registrationDetailId);
                detailWindow.Show();
            }

        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            CitizenWindow citizenWindow = new CitizenWindow(_currentUser);
            citizenWindow.Show();
            this.Close();
        }
    }

}
