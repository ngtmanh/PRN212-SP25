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
using Microsoft.Win32;
using System.IO;



namespace Prn212
{
    /// <summary>
    /// Interaction logic for UpFile.xaml
    /// </summary>
    public partial class UpFile : Window
    {
        public UpFile()
        {
            InitializeComponent();
        }

        private void BtnUpload_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "All Files|*.*"; // Bộ lọc file (có thể chọn theo ý muốn)

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                string fileContent = File.ReadAllText(filePath); // Đọc nội dung file (chỉ áp dụng cho file text)
                MessageBox.Show($"Đã chọn file: {filePath}");
            }
        }

        private void BtnDownload_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text Files|*.txt|All Files|*.*"; // Bộ lọc loại file

            if (saveFileDialog.ShowDialog() == true)
            {
                string filePath = saveFileDialog.FileName;
                File.WriteAllText(filePath, "Nội dung file tải xuống");
                MessageBox.Show($"Đã lưu file: {filePath}");
            }
        }
    }
}
