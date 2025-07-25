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
    public sealed partial class DanhSachHoaDon : Page
    {
        public ObservableCollection<OrderModel> MyDataList { get; set; } = [];
        public DanhSachHoaDon()
        {
            InitializeComponent();

            MyDataList = new ObservableCollection<OrderModel>(LoadOrderList());
        }

        // Hàm này sẽ được gọi khi trang được tải
        private List<OrderModel> LoadOrderList()
        {
            List<OrderModel> orders = new();
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                var query = @"
            SELECT o.order_id, o.order_date, o.order_status, o.UId, a.username
            FROM orders o
            JOIN accounts a ON o.UId = a.UId";

                var cmd = DatabaseConnection.CreateCommand(query, conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var order = new OrderModel
                        {
                            OrderId = reader["order_id"].ToString(),
                            UId = reader["UId"].ToString(),  // vẫn lấy nếu cần
                            OrderDate = Convert.ToDateTime(reader["order_date"]),
                            Status = Convert.ToBoolean(reader["order_status"]),
                            UserName = reader["username"].ToString()
                        };
                        orders.Add(order);
                    }
                }
            }
            return orders;
        }

        public async void Detail_Btn_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is OrderModel order)
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    // Truy vấn để lấy tên người dùng dựa trên UId
                    var query = "SELECT username FROM accounts WHERE UId = @UId";
                    var cmd = DatabaseConnection.CreateCommand(query, conn);
                    cmd.Parameters.AddWithValue("@UId", order.UId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string username = reader["username"].ToString();
                            string date = order.OrderDate.ToString("dd/MM/yyyy");
                            var statusComboBox = new ComboBox
                            {
                                Width = 200,
                                ItemsSource = new List<string> { "Chưa thanh toán", "Đã thanh toán" },
                                SelectedIndex = order.Status ? 1 : 0
                            };
                            var layout = new StackPanel
                            {
                                Spacing = 10,
                                Children =
                                    {
                                        CreateRow("Tên khách hàng:", new TextBlock { Text = username, Width = 200 }),
                                        CreateRow("Thời gian đặt hàng:", new TextBlock { Text = date, Width = 200 }),
                                        CreateRow("Trạng thái:", statusComboBox),
                                        new TextBlock { Text = "Sản phẩm", FontWeight = FontWeights.Bold, Margin = new Thickness(0, 10, 0, 0) }
                                    }
                            }; 
                            // Tạo bảng sản phẩm
                            var grid = new Grid
                            {
                                RowSpacing = 5,
                                ColumnSpacing = 10
                            };

                            // Định nghĩa cột
                            grid.ColumnDefinitions.Add(new ColumnDefinition()); // STT
                            grid.ColumnDefinitions.Add(new ColumnDefinition()); // Tên món
                            grid.ColumnDefinitions.Add(new ColumnDefinition()); // Số lượng
                            grid.ColumnDefinitions.Add(new ColumnDefinition()); // Tổng tiền

                            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                            grid.Children.Add(CreateCell("STT", 0, 0, true));
                            grid.Children.Add(CreateCell("Tên món", 0, 1, true));
                            grid.Children.Add(CreateCell("Số lượng", 0, 2, true));
                            grid.Children.Add(CreateCell("Tổng tiền", 0, 3, true));

                            int row = 1;

                            var items = GetItemOrder(order.OrderId);

                            // Tạo các hàng cho từng món ăn
                            foreach (var item in items)
                            {
                                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                                grid.Children.Add(CreateCell(row.ToString(), row, 0));
                                grid.Children.Add(CreateCell(item.name, row, 1));
                                grid.Children.Add(CreateCell(item.quantity.ToString(), row, 2));
                                grid.Children.Add(CreateCell(item.price_at_order.ToString("N0") + " VNĐ", row, 3));
                                row++;
                            }

                            

                            layout.Children.Add(grid);

                            reader.Close();
                            ContentDialog dialog = new()
                            {
                                Title = "Thông tin đơn hàng",
                                Content = layout,
                                PrimaryButtonText = "Xác nhận",
                                XamlRoot = this.XamlRoot
                            };

                            var result = await dialog.ShowAsync();
                            if (result == ContentDialogResult.Primary)
                            {
                                // Lấy giá trị từ ComboBox
                                bool newStatus = statusComboBox.SelectedIndex == 1;

                                // Cập nhật order object
                                order.Status = newStatus;

                                // Lưu vào database (nếu cần)
                                using (var updateCmd = conn.CreateCommand())
                                {
                                    updateCmd.CommandText = "UPDATE orders SET order_status = @status WHERE order_id = @id";
                                    updateCmd.Parameters.AddWithValue("@status", newStatus ? 1 : 0);
                                    updateCmd.Parameters.AddWithValue("@id", order.OrderId);
                                    updateCmd.ExecuteNonQuery();
                                }
                            }

                        }
                    }
                }
                
            }
        }

        // Lấy danh sách các món ăn trong đơn hàng
        private List<ItemOrder> GetItemOrder(string o_id)
        {
            List<ItemOrder> itemOrders = [];
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                var query = @"
        SELECT oi.product_id, oi.quantity, oi.price_at_order, p.name
        FROM orders_items oi
        JOIN products p ON oi.product_id = p.product_id
        WHERE oi.order_id = @order_id";
                var cmd = DatabaseConnection.CreateCommand(query, conn);
                cmd.Parameters.AddWithValue("@order_id", o_id);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var itemOrder = new ItemOrder
                        {
                            Id = Convert.ToInt32(reader["product_id"]),
                            name = reader["name"].ToString(),
                            quantity = Convert.ToInt32(reader["quantity"]),
                            price_at_order = Convert.ToDecimal(reader["price_at_order"])
                        };
                        itemOrders.Add(itemOrder);
                    }
                }
            }
            return itemOrders;
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

        private static TextBlock CreateCell(string text, int row, int column, bool isHeader = false)
        {
            var tb = new TextBlock
            {
                Text = text,
                FontWeight = isHeader ? FontWeights.Bold : FontWeights.Normal,
                Margin = new Thickness(2)
            };
            Grid.SetRow(tb, row);
            Grid.SetColumn(tb, column);
            return tb;
        }

    }

}
