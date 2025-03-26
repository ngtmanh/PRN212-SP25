using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Windows;
using Prn212.Models;

namespace Prn212
{
    /// <summary>
    /// Interaction logic for ResetPassword.xaml
    /// </summary>
    public partial class ResetPassword : Window
    {
        private string generatedOTP;
        private DateTime otpExpiryTime;
        private static readonly Random random = new Random();
        private readonly Prn212Context context;

        public ResetPassword()
        {
            InitializeComponent();
            context = new Prn212Context();
        }

        private void BtnSendOtp_Click(object sender, RoutedEventArgs e)
        {
            string email = txtEmail.Text.Trim();
            if (string.IsNullOrWhiteSpace(email))
            {
                MessageBox.Show("Vui lòng nhập email!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            
            bool emailExists = context.Users.Any(u => u.Email == email);
            if (!emailExists)
            {
                MessageBox.Show("Email không tồn tại trong hệ thống!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            
            generatedOTP = random.Next(100000, 999999).ToString();
            otpExpiryTime = DateTime.Now.AddMinutes(2);

            
            SendOtpEmail(email, generatedOTP);

            MessageBox.Show("Mã OTP đã được gửi qua email. Vui lòng kiểm tra hộp thư!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnVerifyOtp_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPassword.Password))
            {
                MessageBox.Show("Vui lòng nhập mã OTP!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (DateTime.Now > otpExpiryTime)
            {
                MessageBox.Show("Mã OTP đã hết hạn! Vui lòng gửi lại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (txtPassword.Password == generatedOTP)
            {
                MessageBox.Show("Xác nhận thành công! Bây giờ bạn có thể đặt lại mật khẩu.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                var user = context.Users.FirstOrDefault(u => u.Email == txtEmail.Text);
                ResetPasswordAfterVerifyingOtp reset = new ResetPasswordAfterVerifyingOtp(user);
                reset.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Mã OTP không chính xác! Vui lòng kiểm tra lại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            Login login = new Login();
            login.Show();
            this.Close();
        }

        private void SendOtpEmail(string userEmail, string otp)
        {
            try
            {
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("takhucthienbao@gmail.com", "rvin wacu cqrj wuhn"),
                    EnableSsl = true,
                };

                string subject = "Mã xác nhận OTP";
                string body = $@"
                    <html>
                        <body>
                            <h3>Thông báo từ hệ thống</h3>
                            <p>Mã OTP xác nhận của bạn: <b>{otp}</b></p>
                            <p>Mã sẽ hết hạn sau 2 phút.</p>
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
                MessageBox.Show($"Gửi email thất bại: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
