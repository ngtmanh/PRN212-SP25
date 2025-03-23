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
    public partial class CitizenProfile : Window
    {
        private Prn212Context _context = new Prn212Context();
        private User _currentUser;

        public CitizenProfile(User user)
        {
            InitializeComponent();
            _currentUser = user;
            LoadUserData();
        }

        private void LoadUserData()
        {
            txtFullName.Text = _currentUser.FullName;
            txtEmail.Text = _currentUser.Email;
            txtAddress.Text = _currentUser.Address;
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            var userToUpdate = _context.Users.FirstOrDefault(u => u.UserId == _currentUser.UserId);
            if (userToUpdate != null)
            {
                userToUpdate.FullName = txtFullName.Text;
                userToUpdate.Email = txtEmail.Text;
                userToUpdate.Address = txtAddress.Text;

                if (!string.IsNullOrEmpty(txtOldPassword.Password) && !txtOldPassword.Password.Equals(userToUpdate.Password))
                {
                    MessageBox.Show("Mật khẩu hiện tại sai! Vui lòng thử lại", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

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
                MessageBox.Show("Cập nhật thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                CitizenWindow citizenWindow = new CitizenWindow(_currentUser);
                citizenWindow.Show();
                this.Close();
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
