using Prn212.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Interaction logic for AddMember.xaml
    /// </summary>
    public partial class AddMember : Window
    {
        private readonly Prn212Context _context;
        private User _currentUser;
        public int houseHoldId { get; set; }
        public AddMember(User currentUser)
        {
            _context = new Prn212Context();
            InitializeComponent();
            _currentUser = currentUser;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string fullnameText = txtFullname.Text;
            string relationText = txtRelation.Text;
            string uniqueId = Guid.NewGuid().ToString("N").Substring(0, 8);
            string newEmail = $"user{uniqueId}@gmail.com";


            //if (validate(fullnameText, relationText, out int userId))
            //{
            var newUser = new User
            {
                FullName = fullnameText,
                Email = newEmail,
                Password = "default",
                Role = "Citizen",
                Address = _currentUser.Address,
            };
            _context.Users.Add(newUser);
            _context.SaveChanges();

            var houseHoldMember = new HouseholdMember
            {
                HouseholdId = houseHoldId,
                UserId = newUser.UserId,
                Relationship = relationText
            };

            _context.HouseholdMembers.Add(houseHoldMember);
            _context.SaveChanges();
            Resource.Resource.saveLog(_currentUser.UserId, Resource.ConstLog.HOUSE_HOLD_MEMBER_ADD_SUCCESS, _context);


            MessageBox.Show("Successfull");
            this.Close();

            //}
            //else
            //{
            //    Resource.Resource.saveLog(_currentUser.UserId, Resource.ConstLog.HOUSE_HOLD_MEMBER_ADD_FAIL, _context);
            //}
        }

        //private bool validate(string fullnameText, string relationText, out int userId)
        //{
        //    userId = 0;

        //    if (string.IsNullOrWhiteSpace(fullnameText) || string.IsNullOrWhiteSpace(relationText))
        //    {
        //        MessageBox.Show("Please Input Data");
        //        return false;
        //    }

        //    if (!int.TryParse(fullnameText, out userId))
        //    {
        //        MessageBox.Show("UserID is integer number");

        //        return false;
        //    }
        //    else
        //    {
        //        if (!checkUserId(userId))
        //        {
        //            MessageBox.Show("UserID not found");
        //            return false;
        //        }
        //    }

        //    return true;
        //}

        public bool checkUserId(int userId)
        {
            var data = _context.Users.Where(c => c.UserId == userId).ToList();
            if (data.Count > 0) return true;
            else return false;
        }
    }
}
