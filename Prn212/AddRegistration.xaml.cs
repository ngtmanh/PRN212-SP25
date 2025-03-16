using System;
using System.Collections.Generic;
using System.IO;
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
using Microsoft.Data.SqlClient;
using Microsoft.Win32;
using Prn212.Models;

namespace Prn212
{
    public partial class AddRegistration : Window
    {
        private User _currentUser;
        private Prn212Context context = new Prn212Context();
        private byte[] _fileData;
        private string _fileName;
        private string _fileType;
        public AddRegistration(User user)
        {
            InitializeComponent();
            _currentUser = user;
        }

        private void BtnSubmit_Click(object sender, RoutedEventArgs e)
        {
            string selectedType = ((ComboBoxItem)cmbRegistrationType.SelectedItem).Content.ToString();
            string description = txtDescription.Text;
            string verifyingIdentity = txtVerifyingIdentity.Text;


            // Tạo RegistrationDetail trước
            var registrationDetail = new RegistrationDetail
            {
                Description = description,
                VerifyingIdentity = verifyingIdentity,
                ResidenceFileName = _fileName,
                ResidenceFileType = _fileType,
                ResidenceFileData = _fileData
            };
            context.RegistrationDetails.Add(registrationDetail);
            context.SaveChanges();
            
            // Lấy RegistrationDetailId mới
            int registrationDetailId = registrationDetail.RegistrationDetailId;

            // Tạo đơn đăng ký
            var newRegistration = new Registration
            {
                UserId = _currentUser.UserId,
                RegistrationType = selectedType,
                RegistrationDetailId = registrationDetailId,
                StartDate = DateOnly.FromDateTime(DateTime.Now),
                Status = "Pending"
            };
            context.Registrations.Add(newRegistration);
            context.SaveChanges();
            
            MessageBox.Show("Registration added successfully!");
            ViewRegistration viewReg = new ViewRegistration(_currentUser);
            viewReg.Show();
            this.Close();
        }
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            ViewRegistration viewReg = new ViewRegistration(_currentUser);
            viewReg.Show();
            this.Close();
        }

        private void BtnUpload_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "All Files|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                _fileData = File.ReadAllBytes(filePath); // Đọc file thành byte[]
                _fileName = System.IO.Path.GetFileName(filePath);
                _fileType = System.IO.Path.GetExtension(filePath);

                txtFileName.Text = _fileName;
            }
        }
    }

}
