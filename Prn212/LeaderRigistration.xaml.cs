using Microsoft.EntityFrameworkCore;
using Prn212.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
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
    /// Interaction logic for LeaderRigistration.xaml
    /// </summary>
    public partial class LeaderRigistration : UserControl
    {
        private readonly Prn212Context db;
        private User _currentUser;

        public LeaderRigistration(
            User currentUser
            )
        {
            db = new Prn212Context();
            InitializeComponent();
            _currentUser = currentUser;
            LoadRegistrations(null, null, null, null);
        }

        private void txtSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtSearch.Text == "Nhập số CCCD/CMND")
            {
                txtSearch.Text = "";
                txtSearch.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        private void txtSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                txtSearch.Text = "Nhập số CCCD/CMND";
                txtSearch.Foreground = new SolidColorBrush(Colors.Gray);
            }
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            string searchKey = txtSearch.Text.Trim();

            if (string.IsNullOrWhiteSpace(searchKey) || searchKey == "Nhập số CCCD/CMND")
            {
                LoadRegistrations(null, null, null, null);
                return;
            }

            var registrations = db.Registrations
                .Include(r => r.RegistrationDetail)
                .Include(r => r.ApprovedByNavigation)
                .Include(r => r.User)
                .AsQueryable();

            registrations = registrations.Where(r => r.RegistrationDetail.VerifyingIdentity.Contains(searchKey));

            if (_currentUser.Role == "Citizen")
            {
                registrations = registrations.Where(r => r.UserId == _currentUser.UserId);
            }

            registrationDataGrid.ItemsSource = registrations.ToList();
        }


        private void StartDatePicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadRegistrations(StartDatePicker.SelectedDate, EndDatePicker.SelectedDate, null, null);
        }

        private void EndDatePicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadRegistrations(null, null, StartDateApprovalPicker.SelectedDate, EndDateApprovalPicker.SelectedDate);
        }

        private void LoadRegistrations(DateTime? startDate = null, DateTime? endDate = null, DateTime? startDateApproval = null, DateTime? endDateApproval = null)
        {
            using (var context = new Prn212Context())
            {
                var registrations = context.Registrations
                    .Include(r => r.RegistrationDetail)
                    .Include(r => r.ApprovedByNavigation)
                    .Include(r => r.User)
                    .AsQueryable();

                if (_currentUser.Role == "Citizen")
                {
                    registrations = registrations.Where(r => r.UserId == _currentUser.UserId);
                }

                if (startDate.HasValue && endDate.HasValue)
                {
                    DateOnly start = DateOnly.FromDateTime(startDate.Value);
                    DateOnly end = DateOnly.FromDateTime(endDate.Value);

                    registrations = registrations
                        .Where(r => r.StartDate >= start && r.StartDate <= end);
                }

                if (startDateApproval.HasValue && endDateApproval.HasValue)
                {
                    DateOnly startApproval = DateOnly.FromDateTime(startDateApproval.Value);
                    DateOnly endApproval = DateOnly.FromDateTime(endDateApproval.Value);

                    registrations = registrations
                        .Where(r => r.StartDate >= startApproval && r.StartDate <= endApproval);
                }

                registrationDataGrid.ItemsSource = registrations.ToList();
            }
        }

        private void BtnViewDetail_Click(object sender, RoutedEventArgs e)
        {
            if (registrationDataGrid.SelectedItem is Registration selectedRegistration)
            {
                ViewRegistrationDetail detailWindow = new ViewRegistrationDetail(selectedRegistration, _currentUser);
                detailWindow.Show();
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một đơn đăng ký!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
