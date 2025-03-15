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
    public partial class AddRegistration : Window
    {
        private int _userId;
        public AddRegistration(int userId)
        {
            InitializeComponent();
            _userId = userId;
            LoadRegistrationTypes();
        }

        private void LoadRegistrationTypes()
        {
            cmbRegistrationType.ItemsSource = new List<string>
        {
            "Permanent",
            "Temporary",
            "TemporaryStay"
        };
        }

        private void BtnSubmit_Click(object sender, RoutedEventArgs e)
        {
            string selectedType = cmbRegistrationType.SelectedItem as string;
            if (string.IsNullOrEmpty(selectedType))
            {
                MessageBox.Show("Vui lòng chọn loại đăng ký.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var context = new Prn212Context())
            {
                Registration newRegistration = new Registration
                {
                    UserId = _userId,
                    RegistrationType = selectedType,
                    StartDate = DateOnly.FromDateTime(DateTime.Now),
                    Status = "Pending"
                };

                context.Registrations.Add(newRegistration);
                context.SaveChanges();
            }

            MessageBox.Show("Đăng ký thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

}
