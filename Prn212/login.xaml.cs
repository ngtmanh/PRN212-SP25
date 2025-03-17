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
    public partial class Login : Window
    {
        private readonly Prn212Context _context;

        public Login()
        {
            _context = new Prn212Context();
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string email = txtEmail.Text;
            string password = txtPassword.Password;

            var user = _context.Users.FirstOrDefault(u => u.Email == email && u.Password == password);

            if (user != null)
            {
                MessageBox.Show("Login successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);


                if (user.Role == "AreaLeader")
                {
                    MainWindow window = new MainWindow(user);
                    window.Show();
                }

                if (user.Role == "Citizen")
                {
                    CitizenWindow citizenWindow = new CitizenWindow(user);
                    citizenWindow.Show();
                }

                if (user.Role == "Police")
                {
                    PoliceWindow citizenWindow = new PoliceWindow(user);
                    citizenWindow.Show();
                }

                this.Close();
            }
            else
            {
                MessageBox.Show("Invalid email or password.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            Register registerWindow = new Register(); 
            registerWindow.ShowDialog(); 
        }
    }
}

