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
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using Prn212.Models;

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
            var rg = context.Registrations.FirstOrDefault(r => r.RegistrationId == registration.RegistrationId);
            rg.Comments = txtComments.Text;
            rg.ApprovedBy = _currentUser.UserId;
            rg.ApprovedByNavigation = _currentUser;
            rg.Status = "Approved";
            context.SaveChanges();

            MessageBox.Show("Duyệt thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
            ViewRegistration vr = new ViewRegistration(_currentUser);
            vr.Show();           

        }

        private void BtnReject_Click(object sender, RoutedEventArgs e)
        {
            var rg = context.Registrations.FirstOrDefault(r => r.RegistrationId == registration.RegistrationId);
            rg.Comments = txtComments.Text;
            rg.ApprovedBy = _currentUser.UserId;
            rg.ApprovedByNavigation = _currentUser;
            rg.Status = "Rejected";
            context.SaveChanges();

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
    }
}
