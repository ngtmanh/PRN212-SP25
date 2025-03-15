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
    public partial class ViewRegistrationDetail : Window
    {
        public ViewRegistrationDetail(object selectedRegistration)
        {
            InitializeComponent();
            LoadRegistrationDetails(selectedRegistration);
        }

        private void LoadRegistrationDetails(object selectedRegistration)
        {
            
            dynamic registration = selectedRegistration;

            txtRegistrationType.Text = registration.RegistrationType;
            txtStartDate.Text = registration.StartDate.ToString();
            txtStatus.Text = registration.Status;
            txtIdentity.Text = registration.IdentityNumber;
            txtResidence.Text = registration.ResidenceDocument;
            txtDescription.Text = registration.Description;
            txtComments.Text = registration.Comments ?? "No comments"; 
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
