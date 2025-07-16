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

        // Phương thức để xử lý sự kiện khi người dùng nhấn nút "Xem"
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
        // Phương thức để xử lý sự kiện khi người dùng nhấn nút "Chỉnh sửa"
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

        // Phương thức để xử lý sự kiện khi người dùng nhấn nút "Thêm"
        public async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            // Ô nhập tên người dùng
            TextBox userNameBox = new()
            {
                PlaceholderText = "Nhập tên người dùng",
                Width = 200
            };
            // Ô nhập mật khẩu
            PasswordBox passwordBox = new()
            {
                PlaceholderText = "Nhập mật khẩu",
                Width = 200
            };
            // Ô nhập vai trò
            ComboBox roleComboBox = new()
            {
                Width = 200,
                ItemsSource = new List<string> { "customer","staff" },
                SelectedItem = "customer"
            };
            // 1. Thêm TextBox thời gian
            TextBox timeBox = new()
            {
                PlaceholderText = "Nhập số tiền muốn nạp (VND)",
                Width = 200,
                Visibility = Visibility.Visible
            };
            TextBox nameBox = new()
            {
                PlaceholderText = "Nhập tên người dùng",
                Width = 200,
                Visibility = Visibility.Collapsed
            };
            TextBox salaryBox = new()
            {
                PlaceholderText = "Nhập lương (VND)",
                Width = 200,
                Visibility = Visibility.Collapsed
            };
            var layout = new StackPanel
            {
                Spacing = 10,
                Children =
                {
                    CreateRow("Tên người dùng:", userNameBox),
                    CreateRow("Mật khẩu:", passwordBox),
                    CreateRow("Vai trò:", roleComboBox)
                }
            };
            var timeRow = CreateRow("Nạp thêm:", timeBox);
            layout.Children.Add(timeRow);
            var nameRow = CreateRow("Tên nhân viên:", nameBox);
            var salaryRow = CreateRow("Lương:", salaryBox);
            layout.Children.Add(nameRow);
            layout.Children.Add(salaryRow);

            // 2. Xử lý khi thay đổi vai trò
            roleComboBox.SelectionChanged += (s, e) =>
            {
                string selectedRole = roleComboBox.SelectedItem as string;
                if (selectedRole == "customer")
                {
                    timeBox.Visibility = Visibility.Visible;
                    timeRow.Visibility = Visibility.Visible;
                }
                else
                {
                    timeBox.Visibility = Visibility.Collapsed;
                    timeRow.Visibility = Visibility.Collapsed;
                    nameBox.Visibility = Visibility.Visible;
                    nameRow.Visibility = Visibility.Visible;
                    salaryBox.Visibility = Visibility.Visible;
                    salaryRow.Visibility = Visibility.Visible;
                }
            };
            ContentDialog dialog = new()
            {
                Title = "Thêm người dùng mới",
                Content = layout,
                PrimaryButtonText = "Xác nhận",
                CloseButtonText = "Hủy",
                XamlRoot = this.Content.XamlRoot
            };
            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                string userName = userNameBox.Text;
                string password = passwordBox.Password;
                string role = roleComboBox.SelectedItem as string;
                int existingTime = 0;

                if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(role))
                {
                    using var conn = DatabaseConnection.GetConnection();
                    conn.Open();

                    // Thêm account
                    var cmd = DatabaseConnection.CreateCommand(
                        "INSERT INTO accounts (username, pwd, roles) VALUES (@username, @pwd, @roles)", conn);
                    cmd.Parameters.AddWithValue("@username", userName);
                    cmd.Parameters.AddWithValue("@pwd", password);
                    cmd.Parameters.AddWithValue("@roles", role);
                    cmd.ExecuteNonQuery();

                    // Lấy UID
                    cmd = DatabaseConnection.CreateCommand("SELECT UId FROM accounts WHERE username = @username", conn);
                    cmd.Parameters.AddWithValue("@username", userName);
                    string userId = cmd.ExecuteScalar()?.ToString();

                    // Nếu là customer thì thêm thời gian sử dụng
                    if (role == "customer")
                    {
                        int.TryParse(timeBox.Text, out existingTime);
                        int themPhut = existingTime / 1000;

                        cmd = DatabaseConnection.CreateCommand(
                            "INSERT INTO customer_time (UId, existing_time) VALUES (@UId, @existing_time)", conn);
                        cmd.Parameters.AddWithValue("@UId", userId);
                        cmd.Parameters.AddWithValue("@existing_time", themPhut);
                        cmd.ExecuteNonQuery();
                    }
                    else { 
                        // Nếu là staff thì thêm tên và lương
                        string staffName = nameBox.Text;
                        int salary = 0;
                        int.TryParse(salaryBox.Text, out salary);
                        cmd = DatabaseConnection.CreateCommand(
                            "INSERT INTO staff ( Full_name, basic_salary) VALUES ( @name, @salary)", conn);
                        cmd.Parameters.AddWithValue("@name", staffName);
                        cmd.Parameters.AddWithValue("@salary", salary);
                        cmd.ExecuteNonQuery();
                    }

                    // Cập nhật danh sách
                    MyDataList.Add(new UserModel
                    {
                        UserName = userName,
                        Password = password,
                        Role = role,
                        RemainingTime = (role == "customer" ? (existingTime / 1000).ToString() : "0")
                    });
                }


                // Cập nhật danh sách người dùng
                MyDataList.Add(new UserModel { UserName = userName, Password = password, Role = role });
                }
            }

        }
    }

