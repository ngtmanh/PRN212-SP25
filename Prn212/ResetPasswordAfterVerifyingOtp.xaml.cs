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
    /// <summary>
    /// Interaction logic for ResetPasswordAfterVerifyingOtp.xaml
    /// </summary>
    public partial class ResetPasswordAfterVerifyingOtp : Window
    {

        private User userChangePassword;
        private Prn212Context _context;
        public ResetPasswordAfterVerifyingOtp(User user)
        {
            InitializeComponent();
            userChangePassword = user;
            _context = new Prn212Context();
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            var userToUpdate = _context.Users.FirstOrDefault(u => u.UserId == userChangePassword.UserId);
            if (userToUpdate != null)
            {
                if (!string.IsNullOrEmpty(txtPassword.Password) && !string.IsNullOrEmpty(txtRePassword.Password) && !txtPassword.Password.Equals(txtRePassword.Password))
                {
                    MessageBox.Show("Mật khẩu nhập lại không đúng! Vui lòng thử lại", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!string.IsNullOrEmpty(txtPassword.Password))
                {
                    userToUpdate.Password = txtPassword.Password;
                }

                _context.SaveChanges();
                MessageBox.Show("Thay mật khẩu thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                Login login = new Login();
                login.Show();
                this.Close();
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Login login = new Login();  
            login.Show();
            this.Close();
        }
    }
}
