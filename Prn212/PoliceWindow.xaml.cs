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

    public partial class PoliceWindow : Window
    {
        private User _currentUser;
        public PoliceWindow(User user)
        {
            InitializeComponent();
            _currentUser = user;
            LoadImage();
        }

        private void LoadImage()
        {
            ImageBrush brush = new ImageBrush();
            brush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/building.jpg"));
            this.Background = brush;
        }

        private void BtnProfile_Click(object sender, RoutedEventArgs e)
        {
            CitizenProfile profileWindow = new CitizenProfile(_currentUser);
            profileWindow.Show();
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Bạn có chắc chắn muốn đăng xuất?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown(); 
            }
        }

        private void BtnViewRegistration_Click(object sender, RoutedEventArgs e)
        {
            ViewRegistration viewRegistration = new ViewRegistration(_currentUser);
            viewRegistration.Show();
            this.Close();
        }
    }
}
