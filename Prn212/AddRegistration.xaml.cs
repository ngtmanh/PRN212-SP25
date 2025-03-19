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
using System.Net.Mail;
using System.Net;


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
            //LoadCbRegistrationType();
        }

        //private void LoadCbRegistrationType()
        //{
        //    
        //    var registrationTypes = context.Registrations
        //                                   .Select(r => r.RegistrationType)
        //                                   .Distinct()
        //                                   .ToList();

  
        //    cbRegistrationType.Items.Clear();

        //    foreach (var type in registrationTypes)
        //    {
        //        cbRegistrationType.Items.Add(type);
        //    }
        //}


        private void BtnSubmit_Click(object sender, RoutedEventArgs e)
        {
            string selectedType = ((ComboBoxItem)cbRegistrationType.SelectedItem).Content.ToString();
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
            SendEmailToUser(newRegistration.RegistrationId, selectedType, _currentUser.Email);

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
        private void SendEmailToUser(int registrationId, string registrationType, string userEmail)
        {
            try
            {
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("takhucthienbao@gmail.com", "rvin wacu cqrj wuhn"),
                    EnableSsl = true,
                };

                string subject = "Xác nhận gửi đơn thành công";
                string body = $@"
                    <html>
                        <body>
                            <h3>Thông báo từ hệ thống</h3>
                            <p>Đơn đăng ký của bạn đã được gửi thành công!</p>
                            <table border='1'>
                                <tr>
                                    <td>Mã đơn</td>
                                    <td>{registrationId}</td>
                                </tr>
                                <tr>
                                    <td>Loại đơn</td>
                                    <td>{registrationType}</td>
                                </tr>
                                <tr>
                                    <td>Ngày gửi</td>
                                    <td>{DateTime.Now.ToString("dd/MM/yyyy HH:mm")}</td>
                                </tr>
                            </table>
                            <p>Chúng tôi sẽ xử lý đơn của bạn trong thời gian sớm nhất.</p>
                        </body>
                    </html>";

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("tathienbao2004@gmail.com"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true,
                };
                mailMessage.To.Add(userEmail);

                smtpClient.Send(mailMessage);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to send email: {ex.Message}");
            }
        }
    }

}
