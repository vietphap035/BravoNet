using DACS_1.Database;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using WinRT.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DACS_1
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        // cài đặt chế độ hiển thị cửa sổ ở chế độ tối đa
        private const int SW_MAXIMIZE = 3;
        public MainWindow(string user)
        {
            InitializeComponent();
            // mở rộng cửa sổ
            var hwnd = WindowNative.GetWindowHandle(this);
            ShowWindow(hwnd, SW_MAXIMIZE);
            this.Closed += MainWindow_Closed;
        }

        // goi đến các trang khác
        public void DsMay_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(typeof(DanhSachMayPage));
        }

        public void DsKH_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(typeof(DanhSachNguoiDungPage));
        }

        public void DsNV_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(typeof(DanhSachNhanVIen));
        }

        public void DsTP_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(typeof(DanhSachThucPham), this);
        }

        public void DsHD_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(typeof(DanhSachHoaDon));
        }
        public void Thongke_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(typeof(Thongkepage));
        }

        // Xử lý sự kiện khi cửa sổ chính đóng
        private void MainWindow_Closed(object sender, WindowEventArgs args)
        {
            SetCurrentUserOffline(App.CurrentUserId);
        }

        // Cập nhật trạng thái người dùng hiện tại là ngoại tuyến
        private void SetCurrentUserOffline(string uid)
        {
            using var conn = DatabaseConnection.GetConnection();
            conn.Open();

            // Cập nhật trạng thái is_online = 0
            var cmd1 = DatabaseConnection.CreateCommand(
                "UPDATE accounts SET is_online = 0 WHERE UId = @UId", conn);
            cmd1.Parameters.AddWithValue("@UId", uid);
            cmd1.ExecuteNonQuery();
            // Tính thời gian đã đăng nhập
            var lastlogout = DateTime.Now;
            DateTime lastLogin = DateTime.MinValue;
            string role = "";

            // Lấy last_login và role
            var cmd2 = DatabaseConnection.CreateCommand("SELECT last_login, roles FROM accounts WHERE UId = @UId", conn);
            cmd2.Parameters.AddWithValue("@UId", uid);
            using (var reader = cmd2.ExecuteReader())
            {
                if (reader.Read())
                {
                    if (reader["last_login"] != DBNull.Value)
                        lastLogin = Convert.ToDateTime(reader["last_login"]);

                    if (reader["roles"] != DBNull.Value)
                        role = reader["roles"].ToString();
                }
            }

            // Tính thời gian đã sử dụng
            TimeSpan sessionDuration = lastlogout - lastLogin;

            if (role == "customer")
            {
                int existingTime = 0;

                // Lấy thời gian còn lại
                var cmd = DatabaseConnection.CreateCommand("SELECT existing_time FROM customer_time WHERE UId = @UId", conn);
                cmd.Parameters.AddWithValue("@UId", uid);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read() && reader["existing_time"] != DBNull.Value)
                    {
                        existingTime = Convert.ToInt32(reader["existing_time"]);
                    }
                }

                // Trừ thời gian đã sử dụng
                int updatedTime = existingTime - (int)sessionDuration.TotalMinutes;
                if (updatedTime < 0) updatedTime = 0;

                // Cập nhật lại thời gian còn lại
                var comd = DatabaseConnection.CreateCommand(
                    "UPDATE customer_time SET existing_time = @time WHERE UId = @UId", conn);
                comd.Parameters.AddWithValue("@time", updatedTime);
                comd.Parameters.AddWithValue("@UId", uid);
                comd.ExecuteNonQuery();
            }
            else
            {
                var cmd = DatabaseConnection.CreateCommand(
                    "UPDATE staffs SET work_time = work_time + @minutes WHERE UId = @UId", conn);
                cmd.Parameters.AddWithValue("@minutes", (int)sessionDuration.TotalMinutes);
                cmd.Parameters.AddWithValue("@UId", uid);
                cmd.ExecuteNonQuery();
            }

            // Cập nhật last_login = NULL
            var cmd3 = DatabaseConnection.CreateCommand(
                "UPDATE accounts SET last_login = NULL WHERE UId = @UId", conn);
            cmd3.Parameters.AddWithValue("@UId", uid);
            cmd3.ExecuteNonQuery();
        }
    }
}
