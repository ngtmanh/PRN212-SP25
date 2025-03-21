using Microsoft.EntityFrameworkCore;
using Prn212.Models;
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
            loadData();
        }

        public  void loadData()
        {
            var dg = db.Registrations
                .Include(c => c.RegistrationDetail)
                .Include(c => c.User)
                .Where(c => c.Status == "Approved")
                .ToList();
            registrationDataGrid.ItemsSource = dg;
        }
    }
}
