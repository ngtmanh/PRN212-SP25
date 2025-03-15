using Microsoft.EntityFrameworkCore;
using Prn212.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
    /// Interaction logic for UpdateMember.xaml
    /// </summary>
    public partial class UpdateMember : Window
    {
        private readonly Prn212Sp25Context _context;

        public HouseholdMember householdMember { get; set; }
        public UpdateMember(int menberId)
        {
            _context = new Prn212Sp25Context();
            InitializeComponent();
            loadData(menberId);
        }
        public void loadData(int memberId)
        {
            var data = _context.HouseholdMembers
                .Include(c => c.Household)
                .Include(c => c.User)
                .ToList();

            HouseholdMember h = data.Where(c => c.MemberId == memberId).First();
            txtFullName.Text = h.User.FullName;
            txtAddress.Text = h.User.Address;
            txtEmail.Text = h.User.Email;
            txtRelationship.Text = h.Relationship;

            householdMember = h;

        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            householdMember.User.FullName = txtFullName.Text;
            householdMember.User.Address = txtAddress.Text;
            householdMember.User.Email = txtEmail.Text;
            householdMember.Relationship = txtRelationship.Text;

            _context.SaveChanges();
            Resource.Resoure.saveLog(Resource.Resoure.getUserId(), Resource.ConstLog.HOUSE_HOLD_MEMBER_UPDATE_SUCCESS, _context);
            MessageBox.Show("Update Successfull");
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Resource.Resoure.saveLog(Resource.Resoure.getUserId(), Resource.ConstLog.HOUSE_HOLD_MEMBER_UPDATE_FAIL, _context);

            this.Close();
        }
    }
}
