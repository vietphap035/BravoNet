using DACS_1.Database;
using DACS_1.Model;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DACS_1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DanhSachNguoiDungPage : Page
    {
        public ObservableCollection<UserModel> MyDataList { get; set; } = [];

        public DanhSachNguoiDungPage()
        {
            InitializeComponent();

            MyDataList = new ObservableCollection<UserModel>(LoadUserList());
        }

        private List<UserModel> LoadUserList()
        {
            List<UserModel> userList = [];
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();

                var query = @"
            SELECT a.UId, a.username, a.pwd, a.roles, a.is_online, ct.existing_time
            FROM accounts a
            LEFT JOIN customer_time ct ON a.UId = ct.UId;
        ";

                var cmd = DatabaseConnection.CreateCommand(query, conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string uid = reader["UId"].ToString();
                        TimeSpan remainingTime = TimeSpan.Zero;

                        if (reader["existing_time"] != DBNull.Value)
                        {
                            int minutes = Convert.ToInt32(reader["existing_time"]);
                            remainingTime = TimeSpan.FromMinutes(minutes);
                        }
                        else
                        {
                            remainingTime = TimeSpan.Zero;
                        }

                        UserModel user = new()
                        {
                            UId = uid,
                            UserName = reader["username"].ToString(),
                            Password = reader["pwd"].ToString(),
                            Role = reader["roles"].ToString(),
                            IsActived = reader.GetBoolean(reader.GetOrdinal("is_online")),
                            RemainingTime = remainingTime.ToString(@"hh\:mm\:ss")
                        };

                        userList.Add(user);
                    }
                }
            }

            return userList;
        }

        public async void Detail_Btn(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is UserModel clickedUser)
            {
                string message = $"UId: {clickedUser.UId}\n" +
                                 $"Username: {clickedUser.UserName}\n" +
                                 $"Password: {clickedUser.Password}\n" +
                                 $"Role: {clickedUser.Role}\n" +
                                 $"Is Active: {clickedUser.IsActived}\n" +
                                 $"Remaining Time: {clickedUser.RemainingTime}";
                ContentDialog dialog = new()
                {
                    Title = "User Details",
                    Content = message,
                    CloseButtonText = "Close",
                    XamlRoot = this.Content.XamlRoot
                };
                await dialog.ShowAsync();
            }
        }
        private async void Edit_Btn(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is UserModel user)
            {
                // Ô nhập số tiền muốn nạp
                TextBox napThemBox = new()
                {
                    PlaceholderText = "Nhập số tiền muốn nạp (VND)",
                    Width = 200
                };

                var layout = new StackPanel
                {
                    Spacing = 10,
                    Children =
            {
                CreateRow("Tên khách hàng:", new TextBlock { Text = user.UserName, Width = 200 }),
                CreateRow("Thời gian hiện có:", new TextBlock { Text = user.RemainingTime, Width = 200 }),
                CreateRow("Nạp thêm:", napThemBox)
            }
                };

                ContentDialog dialog = new()
                {
                    Title = "Nạp thời gian sử dụng",
                    Content = layout,
                    PrimaryButtonText = "Xác nhận",
                    CloseButtonText = "Hủy",
                    XamlRoot = btn.XamlRoot
                };

                var result = await dialog.ShowAsync();

                if (result == ContentDialogResult.Primary)
                {
                    // Lấy số tiền nhập vào
                    if (int.TryParse(napThemBox.Text, out int soTienNap))
                    {
                        // Giả sử: 1000 VNĐ = 1 phút
                        int themPhut = soTienNap / 1000;

                        using var conn = DatabaseConnection.GetConnection();
                        conn.Open();

                        var cmd = DatabaseConnection.CreateCommand("UPDATE customer_time SET existing_time = existing_time + @themPhut WHERE UId = @UId", conn);
                        cmd.Parameters.AddWithValue("@themPhut", themPhut);
                        cmd.Parameters.AddWithValue("@UId", user.UId);
                        cmd.ExecuteNonQuery();
                        Debug.WriteLine($"Đã nạp {soTienNap} VND => {themPhut} phút");
                        Debug.WriteLine($"Trước khi cộng: {user.RemainingTime}");
                        user.RemainingTimeSpan = user.RemainingTimeSpan.Add(TimeSpan.FromMinutes(themPhut));
                        Debug.WriteLine($"Sau khi cộng: {user.RemainingTime}");
                    }
                }
            }
        }

        private static StackPanel CreateRow(string label, UIElement control)
        {
            return new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Spacing = 10,
                Children =
        {
            new TextBlock
            {
                Text = label,
                Width = 150,
                VerticalAlignment = VerticalAlignment.Center,
                FontWeight = FontWeights.Bold
            },
            control
        }
            };
        }

    }
}
