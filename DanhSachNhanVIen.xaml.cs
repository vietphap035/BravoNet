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
    public sealed partial class DanhSachNhanVIen : Page
    {
        public ObservableCollection<StaffModel> MyDataList { get; set; } = [];
        public DanhSachNhanVIen()
        {
            InitializeComponent();
            MyDataList = new ObservableCollection<StaffModel>(LoadStaffList());
        }

        private List<StaffModel> LoadStaffList()
        {
            List<StaffModel> staffList = [];
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                var query = @"
                SELECT UId, Full_name, basic_salary, bonus_salary, work_time
                FROM staffs;
                ";
                var cmd = DatabaseConnection.CreateCommand(query, conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string uid = reader["UId"].ToString();
                        string fullName = reader["Full_name"].ToString();
                        decimal basicSalary = Convert.ToDecimal(reader["basic_salary"]);
                        decimal bonus = Convert.ToDecimal(reader["bonus_salary"]);
                        int workingHours = Convert.ToInt32(reader["work_time"]);
                        StaffModel staff = new StaffModel
                        {
                            UId = uid,
                            FullName = fullName,
                            basicSalary = basicSalary,
                            bonus = bonus,
                            workingHours = workingHours
                        };
                        staffList.Add(staff);
                    }
                }
            }
            return staffList;
        }

        private async void Edit_Btn(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is StaffModel staff)
            {
                TextBox luongCoBan = new()
                {
                    PlaceholderText = "Nhập số tiền muốn nạp (VND)",
                    Width = 200,
                    Text = staff.basicSalary.ToString()
                };
                TextBox thuong = new()
                {
                    PlaceholderText = "Nhập số tiền thưởng (VND)",
                    Width = 200,
                    Text = staff.bonus.ToString()
                };
                var layout = new StackPanel
                {
                    Spacing = 10,
                    Children =
                    {
                        CreateRow("Tên nhân viên:", new TextBlock { Text = staff.FullName, Width = 200 }),
                        CreateRow("Lương cơ bản:", luongCoBan),
                        CreateRow("Thưởng:", thuong)
                    }
                };
                ContentDialog dialog = new()
                {
                    Title = "Điều chỉnh lương",
                    Content = layout,
                    PrimaryButtonText = "Xác nhận",
                    CloseButtonText = "Hủy",
                    XamlRoot = btn.XamlRoot
                };
                var result = await dialog.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    // Lấy số tiền nhập vào
                    decimal parsedLuong = decimal.TryParse(luongCoBan.Text, out decimal LuongCoBan) ? LuongCoBan : staff.basicSalary;
                    decimal parsedThuong = decimal.TryParse(thuong.Text, out decimal Thuong) ? Thuong : staff.bonus;

                    if (parsedLuong != staff.basicSalary || parsedThuong != staff.bonus)
                    {
                        using var conn = DatabaseConnection.GetConnection();
                        conn.Open();

                        var cmd = conn.CreateCommand();
                        cmd.CommandText = @"
                        UPDATE staffs 
                        SET 
                            basic_salary = @basic_salary, 
                            bonus_salary = @bonus_salary 
                        WHERE UId = @UId";

                        cmd.Parameters.AddWithValue("@basic_salary", LuongCoBan);
                        cmd.Parameters.AddWithValue("@bonus_salary", Thuong);
                        cmd.Parameters.AddWithValue("@UId", staff.UId);

                        await cmd.ExecuteNonQueryAsync();
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
