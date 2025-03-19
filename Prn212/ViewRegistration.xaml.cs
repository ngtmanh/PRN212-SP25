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
    public partial class ViewRegistration : Window
    {
        private User _currentUser;

        public ViewRegistration(User user)
        {
            InitializeComponent();
            _currentUser = user;
            cbStatus.SelectedIndex = 0;
            LoadRegistrations("All", null, null, null, null);
            CheckRole();
        }

        private void LoadRegistrations(string status = null, DateTime? startDate = null, DateTime? endDate = null, DateTime? startDateApproval = null, DateTime? endDateApproval = null)
        {
            using (var context = new Prn212Context())
            {
                var registrations = context.Registrations
                    .Include(r => r.RegistrationDetail)
                    .Include(r => r.ApprovedByNavigation)
                    .Include(r => r.User)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(status) && status != "All")
                {
                    registrations = registrations.Where(r => r.Status == status);
                }

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
                detailWindow.ShowDialog();
                this.Close();
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một đơn đăng ký!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void CheckRole()
        {
            if (_currentUser.Role == "Police")
            {
                btnAddRegistration.Visibility = Visibility.Collapsed;
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            Window nextWindow = _currentUser.Role == "Citizen" ? new CitizenWindow(_currentUser) : new PoliceWindow(_currentUser);
            nextWindow.Show();
            this.Close();
        }

        private void BtnAddRegistration_Click(object sender, RoutedEventArgs e)
        {
            AddRegistration add = new AddRegistration(_currentUser);
            add.Show();
            this.Close();
        }

        private void CbStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbStatus.SelectedItem is ComboBoxItem selectedItem)
            {
                string selectedStatus = selectedItem.Content.ToString();
                LoadRegistrations(selectedStatus, StartDatePicker.SelectedDate, EndDatePicker.SelectedDate);
            }
        }

        private void StartDatePicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadRegistrations((cbStatus.SelectedItem as ComboBoxItem)?.Content.ToString(), StartDatePicker.SelectedDate, EndDatePicker.SelectedDate, null, null);
        }

        private void EndDatePicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadRegistrations((cbStatus.SelectedItem as ComboBoxItem)?.Content.ToString(), null, null, StartDateApprovalPicker.SelectedDate, EndDateApprovalPicker.SelectedDate);
        }


        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            string status = (cbStatus.SelectedItem as ComboBoxItem)?.Content.ToString();
            string searchKey = txtSearch.Text.Trim();

            using (var context = new Prn212Context())
            {
                var registrations = context.Registrations
                    .Include(r => r.RegistrationDetail)
                    .Include(r => r.ApprovedByNavigation)
                    .Include(r => r.User)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(status) && status != "All")
                {
                    registrations = registrations.Where(r => r.Status == status);
                }

                if (!string.IsNullOrEmpty(searchKey))
                {
                    registrations = registrations.Where(r => r.RegistrationDetail.VerifyingIdentity.Contains(searchKey));
                }

                if (_currentUser.Role == "Citizen")
                {
                    registrations = registrations.Where(r => r.UserId == _currentUser.UserId);
                }

                registrationDataGrid.ItemsSource = registrations.ToList();
            }
        }
    }
}