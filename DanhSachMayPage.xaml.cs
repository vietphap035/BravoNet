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
    public sealed partial class DanhSachMayPage : Page
    {
        public ObservableCollection<Machine> Machines { get; set; }
        public DanhSachMayPage()
        {
            this.InitializeComponent();
            // Tạo ObservableCollection để bind dữ liệu
            Machines = [];
            this.DataContext = this;

            // Tải dữ liệu từ cơ sở dữ liệu và thêm vào ObservableCollection
            var loadedMachines = LoadMachines();
            foreach (var machine in loadedMachines)
            {
                Machines.Add(machine);
            }
        }

        // Phương thức để tải danh sách máy từ cơ sở dữ liệu
        private List<Machine> LoadMachines()
        {
            // Khởi tạo danh sách máy tính
            List<Machine> pcList = [];
            // Sử dụng kết nối đến cơ sở dữ liệu để lấy thông tin máy tính
            using (var conn = DatabaseConnection.GetConnection())
            {
                // Mở kết nối
                conn.Open();
                // Tạo lệnh SQL để lấy thông tin máy tính
                var cmd = DatabaseConnection.CreateCommand("SELECT pc_code, UId, is_active, pc_number FROM pc", conn);
                // Thực thi lệnh và đọc dữ liệu
                using (var reader = cmd.ExecuteReader())
                {
                    // Lặp qua từng dòng dữ liệu trả về
                    while (reader.Read())
                    {
                        // Tạo đối tượng Machine từ dữ liệu đọc được
                        Machine pc = new()
                        {
                            Id = reader["pc_code"].ToString(),
                            UId = reader["UId"]?.ToString(),
                            // Kiểm tra trạng thái hoạt động và gán đường dẫn hình ảnh tương ứng
                            ImagePath = reader.GetBoolean("is_active")
                                ? "ms-appx:///Assets/ACTIVE.png"
                                : "ms-appx:///Assets/NOT_ACTIVE.png",
                            PcNumber = reader.GetInt32("pc_number")
                        };
                        // Thêm đối tượng Machine vào danh sách
                        pcList.Add(pc);
                    }
                }

                return pcList;
            }
        }
        // Sự kiện khi người dùng click vào một máy tính trong GridView
        private async void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Lấy máy tính được click từ sự kiện
            var clickedMachine = e.ClickedItem as Machine;

            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                var cmd = DatabaseConnection.CreateCommand("SELECT * FROM pc WHERE pc_code = @pc_code", conn);
                cmd.Parameters.AddWithValue("@pc_code", clickedMachine.Id);
                cmd.ExecuteNonQuery();

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        if (reader["UId"] == DBNull.Value)
                        {
                            // Nếu máy tính không có người dùng đang hoạt động
                            TextBlock pcText = new() { Text = reader["pc_number"].ToString(), FontSize = 16 };
                            TextBlock status = new() { Text = "Not active", FontSize = 16 };
                            var layout = new StackPanel
                            {
                                Spacing = 10,
                                Children =
                                {
                                    CreateRow("Số máy:", pcText),
                                    CreateRow("Trạng thái", status)
                                }
                            };
                            ContentDialog successDialog = new()
                            {
                                Title = clickedMachine?.PcNumber,
                                Content = layout,
                                CloseButtonText = "OK",
                                XamlRoot = this.Content.XamlRoot
                            };
                            await successDialog.ShowAsync();
                        }
                        else
                        {
                            // Nếu máy tính có người dùng đang hoạt động
                            var uid = reader["UId"].ToString();
                            TextBlock pcText = new() { Text = reader["pc_number"].ToString(), FontSize = 16 };

                            TextBlock userText = new();
                            TextBlock usedTimeText = new();
                            TextBlock remainingTimeText = new();
                            reader.Close();
                            // Lấy account
                            var accountCmd = DatabaseConnection.CreateCommand("SELECT * FROM accounts WHERE UID = @uid", conn);
                            accountCmd.Parameters.AddWithValue("@uid", uid);
                            using (var accountReader = accountCmd.ExecuteReader())
                            {
                                if (accountReader.Read())
                                {
                                    userText.Text = accountReader["username"].ToString();
                                    usedTimeText.Text = accountReader["last_login"].ToString();
                                }
                                accountReader.Close();
                            }
                            
                            // Lấy thời gian còn lại
                            var timeCmd = DatabaseConnection.CreateCommand("SELECT * FROM customer_time WHERE UId = @uid", conn);
                            timeCmd.Parameters.AddWithValue("@uid", uid);
                            using (var timeReader = timeCmd.ExecuteReader())
                            {
                                if (timeReader.Read())
                                {
                                    remainingTimeText.Text = timeReader["existing_time"].ToString();
                                }
                            }

                            // Tạo UI
                            var layout = new StackPanel
                            {
                                Spacing = 10,
                                Children =
        {
            CreateRow("Số máy:", pcText),
            CreateRow("Tên người dùng:", userText),
            CreateRow("Đăng nhập lúc:", usedTimeText),
            CreateRow("Thời gian còn lại:", remainingTimeText)
        }
                            };

                            ContentDialog dialog = new()
                            {
                                Title = "Thông tin máy đang hoạt động",
                                Content = layout,
                                CloseButtonText = "OK",
                                XamlRoot = this.Content.XamlRoot
                            };
                            await dialog.ShowAsync();
                        }

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
