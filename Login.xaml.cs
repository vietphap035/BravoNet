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
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using WinRT;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DACS_1
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Login : Window
    {
        public Login()
        {
            this.InitializeComponent();
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            var windowId = Win32Interop.GetWindowIdFromWindow(hwnd);
            var appWindow = AppWindow.GetFromWindowId(windowId);
            appWindow.SetPresenter(AppWindowPresenterKind.Overlapped);

            appWindow.Presenter.As<OverlappedPresenter>().IsMaximizable = false;
        }
        public async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            // khoi tao bien dang nhap
            string username = UsernameTextBox.Text;
            string password = PasswordTextBox.Password;

            // kiem tra dang nhap
            using (var con = DatabaseConnection.GetConnection())
            {
                try
                {
                    con.Open();
                    string query = "SELECT COUNT(*) FROM accounts WHERE username = @username AND pwd = @password";

                    using (var cmd = DatabaseConnection.CreateCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@password", password);

                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        if (count > 0)
                        {
                            // Dang nhap thanh cong
                            ContentDialog successDialog = new ()
                            {
                                Title = "Login Successful",
                                Content = "Welcome to DACS1!",
                                CloseButtonText = "OK",
                                XamlRoot = this.Content.XamlRoot
                            };
                            await successDialog.ShowAsync();
                            // Chuyen trang thai hoat dong
                            string updateQuery = "UPDATE accounts SET is_online = TRUE WHERE username = @username";
                            using (var updateCmd = DatabaseConnection.CreateCommand(updateQuery, con))
                            {
                                updateCmd.Parameters.AddWithValue("@username", username);
                                updateCmd.ExecuteNonQuery();
                            }
                            // Chuyen den MainWindow
                            var mainWindow = new MainWindow(username);
                            mainWindow.Activate();
                            this.Close();
                        }
                        else
                        {
                            // Dang nhap that bai
                            ContentDialog errorDialog = new()
                            {
                                Title = "Login Failed",
                                Content = "Invalid username or password.",
                                CloseButtonText = "OK",
                                XamlRoot = this.Content.XamlRoot
                            };
                            await errorDialog.ShowAsync();
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Xử lý lỗi kết nối cơ sở dữ liệu
                    ContentDialog errorDialog = new ContentDialog
                    {
                        Title = "Error",
                        Content = $"An error occurred while connecting to the database: {ex.Message}",
                        CloseButtonText = "OK",
                        XamlRoot = this.Content.XamlRoot
                    };
                    await errorDialog.ShowAsync();
                }
            }

        }
    }

}
