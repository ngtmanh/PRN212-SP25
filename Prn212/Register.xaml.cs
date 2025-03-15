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
    public partial class Register : Window
    {
        private Prn212Context _context = new Prn212Context(); 

        public Register()
        {
            InitializeComponent();
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            string fullName = txtFullName.Text;
            string email = txtEmail.Text;
            string password = txtPassword.Password;
            string address = txtAddress.Text;

            
            if (_context.Users.Any(u => u.Email == email))
            {
                MessageBox.Show("Email đã tồn tại, vui lòng chọn email khác!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            
            var newUser = new User
            {
                FullName = fullName,
                Email = email,
                Password = password,
                Address = address,
                Role = "Citizen" 
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();

            MessageBox.Show("Đăng ký thành công! Vui lòng đăng nhập.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

            this.Close(); // Đóng màn hình đăng ký, quay lại Login
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); // Đóng màn hình đăng ký
        }
    }
}