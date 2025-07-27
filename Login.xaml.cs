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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
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
            string packageFamilyName = Windows.ApplicationModel.Package.Current.Id.FamilyName;
            Debug.WriteLine($"PackageFamilyName: {packageFamilyName}");

            // kiem tra dang nhap
            using (var con = DatabaseConnection.GetConnection())
            {
                try
                {
                    con.Open();
                    string query = "SELECT * FROM accounts WHERE username = @username AND pwd = @password";

                    using (var cmd = DatabaseConnection.CreateCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@password", password);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                
                                string userId = reader["UId"].ToString();

                                
                                ContentDialog successDialog = new()
                                {
                                    Title = "Login Successful",
                                    Content = "Welcome to DACS1!",
                                    CloseButtonText = "OK",
                                    XamlRoot = this.Content.XamlRoot
                                };
                                await successDialog.ShowAsync();

                                
                                reader.Close(); 
                                string updateQuery = "UPDATE accounts SET is_online = TRUE WHERE UId = @UId";
                                using (var updateCmd = DatabaseConnection.CreateCommand(updateQuery, con))
                                {
                                    updateCmd.Parameters.AddWithValue("@UId", userId);
                                    updateCmd.ExecuteNonQuery();
                                }
                                string updateLoginQuery = "UPDATE accounts SET last_login = @last_login WHERE UId = @UId";
                                using (var updateCmd = DatabaseConnection.CreateCommand(updateLoginQuery, con))
                                {
                                    updateCmd.Parameters.AddWithValue("@last_login", DateTime.Now);
                                    updateCmd.Parameters.AddWithValue("@UId", userId);
                                    updateCmd.ExecuteNonQuery();
                                }


                                App.CurrentUserId = userId;

                                
                                var mainWindow = new MainWindow(username); 
                                mainWindow.Activate();
                                this.Close();
                            }
                            else
                            {
                                
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
