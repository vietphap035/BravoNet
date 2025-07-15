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

        private const int SW_MAXIMIZE = 3;
        public MainWindow(string user)
        {
            InitializeComponent();
            var hwnd = WindowNative.GetWindowHandle(this);
            ShowWindow(hwnd, SW_MAXIMIZE);
            this.Closed += MainWindow_Closed;
        }
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
            ContentFrame.Navigate(typeof(DanhSachThucPham));
        }

        public void DsHD_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(typeof(DanhSachHoaDon));
        }
        private void MainWindow_Closed(object sender, WindowEventArgs args)
        {
            SetCurrentUserOffline(App.CurrentUserId);
        }

        private void SetCurrentUserOffline(string uid)
        {
            using var conn = DatabaseConnection.GetConnection();
            conn.Open();

            // Update is_online = 0
            var cmd1 = DatabaseConnection.CreateCommand(
                "UPDATE accounts SET is_online = 0 WHERE UId = @UId", conn);
            cmd1.Parameters.AddWithValue("@UId", uid);
            cmd1.ExecuteNonQuery();

            // Reset last_login = NULL
            var cmd2 = DatabaseConnection.CreateCommand(
                "UPDATE accounts SET last_login = NULL WHERE UId = @UId", conn);
            cmd2.Parameters.AddWithValue("@UId", uid);
            cmd2.ExecuteNonQuery();
        }
    }
}
