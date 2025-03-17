using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
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
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using Prn212.Models;
using System.Net.Mail;
using System.Net;

namespace Prn212
{
    public partial class ViewRegistrationDetail : Window
    {
        private User _currentUser;
        private Prn212Context context;
        private Registration registration;
        public ViewRegistrationDetail(object selectedRegistration, User user)
        {
            InitializeComponent();
            _currentUser = user ?? throw new ArgumentNullException(nameof(user), "Người dùng không được null.");
            registration = selectedRegistration as Registration ?? throw new ArgumentNullException(nameof(selectedRegistration), "Đăng ký không được null.");
            context = new Prn212Context();
            CheckUserPermissions();
            LoadRegistrationDetails(selectedRegistration);
        }

        private void LoadRegistrationDetails(object selectedRegistration)
        {
            if (selectedRegistration is Registration registration)
            {
                if (registration.RegistrationDetail == null)
                {
                    MessageBox.Show("Chi tiết đăng ký không tồn tại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                int registrationId = registration.RegistrationDetail.RegistrationDetailId;
                var registrationDetail = context.Registrations
                    .Include(r => r.RegistrationDetail)
                    .FirstOrDefault(r => r.RegistrationDetail != null && r.RegistrationDetail.RegistrationDetailId == registrationId);

                if (registrationDetail == null)
                {
                    MessageBox.Show("Không tìm thấy đăng ký trong cơ sở dữ liệu!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                txtRegistrationType.Text = registrationDetail.RegistrationType ?? "N/A";
                txtStatus.Text = registrationDetail.Status ?? "N/A";
                txtDescription.Text = registrationDetail.RegistrationDetail?.Description ?? "N/A";
                txtComments.Text = registrationDetail.Comments ?? "N/A";
                txtIdentity.Text = registrationDetail.RegistrationDetail?.VerifyingIdentity ?? "N/A";
            }
            else
            {
                MessageBox.Show("Dữ liệu không hợp lệ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            ViewRegistration vr = new ViewRegistration(_currentUser);
            vr.Show();
            this.Close();
        }

        private void CheckUserPermissions()
        {
            if (_currentUser.Role == "Citizen")
            {
                btnApprove.Visibility = Visibility.Collapsed;
                btnReject.Visibility = Visibility.Collapsed;
            }

            if (_currentUser.Role == "Police")
            {
                txtComments.IsReadOnly = false;
            }
        }

        private void BtnApprove_Click(object sender, RoutedEventArgs e)
        {
            var rg = context.Registrations
            .Include(r => r.User) // Đảm bảo tải thông tin User
            .FirstOrDefault(r => r.RegistrationId == registration.RegistrationId);
            rg.Comments = txtComments.Text;
            rg.ApprovedBy = _currentUser.UserId;
            rg.ApprovedByNavigation = _currentUser;
            rg.Status = "Approved";
            context.SaveChanges();
            string? userEmail = rg.User?.Email;
            if (!string.IsNullOrEmpty(userEmail))
            {
                SendConfirmationEmail(rg.RegistrationId, "Approved", userEmail, rg.Comments);
                MessageBox.Show("Duyệt thành công! Email thông báo đã được gửi.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Không tìm thấy email của công dân để gửi thông báo.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            MessageBox.Show("Duyệt thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
            ViewRegistration vr = new ViewRegistration(_currentUser);
            vr.Show();           

        }

        private void BtnReject_Click(object sender, RoutedEventArgs e)
        {
            var rg = context.Registrations
            .Include(r => r.User) // Đảm bảo tải thông tin User
            .FirstOrDefault(r => r.RegistrationId == registration.RegistrationId);
            rg.Comments = txtComments.Text;
            rg.ApprovedBy = _currentUser.UserId;
            rg.ApprovedByNavigation = _currentUser;
            rg.Status = "Rejected";
            context.SaveChanges();
            string? userEmail = rg.User?.Email;
            if (!string.IsNullOrEmpty(userEmail))
            {
                SendConfirmationEmail(rg.RegistrationId, "Rejected", userEmail, rg.Comments);
                MessageBox.Show("Email thông báo đã được gửi.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Không tìm thấy email của công dân để gửi thông báo.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            MessageBox.Show("Duyệt thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
            ViewRegistration vr = new ViewRegistration(_currentUser);
            vr.Show();            
        }

        private void BtnDownload_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(registration.RegistrationDetail?.ResidenceFileName?.ToString()))
            {
                MessageBox.Show("Không có file nào để tải xuống.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Lấy RegistrationDetailId từ database
            int registrationDetailId = registration.RegistrationDetail.RegistrationDetailId; // Cần lấy ID của bản ghi đang chọn
            var registrationDetail = context.RegistrationDetails.FirstOrDefault(rd => rd.RegistrationDetailId == registrationDetailId);

            if (registrationDetail == null || registrationDetail.ResidenceFileData == null)
            {
                MessageBox.Show("Không tìm thấy file trong database!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Hộp thoại lưu file
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                FileName = registrationDetail.ResidenceFileName,
                Filter = "All Files|*.*"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllBytes(saveFileDialog.FileName, registrationDetail.ResidenceFileData);
                MessageBox.Show("Tải file thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void SendConfirmationEmail(int registrationId, string status, string userEmail, string comments)
        {
            try
            {
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("takhucthienbao@gmail.com", "rvin wacu cqrj wuhn"),
                    EnableSsl = true,
                };

                string subject = $"Thông báo trạng thái đơn: {status}";
                string body = $@"
                    <html>
                        <body>
                            <h3>Thông báo từ hệ thống</h3>
                            <p>Đơn đăng ký của bạn đã được xử lý!</p>
                            <table border='1'>
                                <tr>
                                    <td>Mã đơn</td>
                                    <td>{registrationId}</td>
                                </tr>
                                <tr>
                                    <td>Trạng thái</td>
                                    <td>{status}</td>
                                </tr>
                                <tr>
                                    <td>Thời gian xử lý</td>
                                    <td>{DateTime.Now.ToString("dd/MM/yyyy HH:mm")}</td>
                                </tr>
                                <tr>
                                    <td>Ghi chú</td>
                                    <td>{comments}</td>
                                </tr>
                            </table>
                            <p>Cảm ơn bạn đã sử dụng dịch vụ!</p>
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
