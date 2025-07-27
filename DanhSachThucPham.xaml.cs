using DACS_1.Database;
using DACS_1.Model;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DACS_1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DanhSachThucPham : Page
    {
        private Window _mainWindow;
        public ObservableCollection<ProductModel> MyDataList { get; set; } = [];
        public DanhSachThucPham()
        {
            InitializeComponent();
            MyDataList = new ObservableCollection<ProductModel>(LoadProductList());
        }

        public List<ProductModel> LoadProductList()
        {
            List<ProductModel> productList = [];
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                var query = @"
                SELECT *
                FROM products;
                ";
                var cmd = DatabaseConnection.CreateCommand(query, conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = Convert.ToInt32(reader["product_id"]);
                        string name = reader["name"].ToString();
                        decimal price = Convert.ToDecimal(reader["price"]);
                        int quantity = Convert.ToInt32(reader["quantity"]);
                        ProductModel product = new ProductModel
                        {
                            P_Id = id,
                            P_Name = name,
                            P_Price = price,
                            P_Quantity = quantity
                        };
                        productList.Add(product);
                    }
                }
            }
            return productList;
        }

        private async void Edit_Btn(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is ProductModel product)
            {
                TextBox giaCa = new()
                {
                    PlaceholderText = "Nhập số tiền bán (VND)",
                    Width = 200,
                    Text = product.P_Price.ToString()
                };
                TextBox soluong = new()
                {
                    PlaceholderText = "Nhập số lượng",
                    Width = 200,
                    Text = product.P_Quantity.ToString()
                };
                var layout = new StackPanel
                {
                    Spacing = 10,
                    Children =
                    {
                        CreateRow("Tên món ăn:", new TextBlock { Text = product.P_Name, Width = 200 }),
                        CreateRow("Giá cả:", giaCa),
                        CreateRow("Số lượng đang bán:", soluong)
                    }
                };
                ContentDialog dialog = new()
                {
                    Title = "Điều chỉnh món ăn",
                    Content = layout,
                    PrimaryButtonText = "Xác nhận",
                    CloseButtonText = "Hủy",
                    XamlRoot = btn.XamlRoot
                };
                var result = await dialog.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    // Lấy số tiền nhập vào
                    decimal parsedGia = decimal.TryParse(giaCa.Text, out decimal GiaCa) ? GiaCa : product.P_Price;
                    int parsedSoLuong = int.TryParse(soluong.Text, out int soLuong) ? soLuong : product.P_Quantity;

                    if (parsedGia != product.P_Price || parsedSoLuong != product.P_Quantity)
                    {
                        using var conn = DatabaseConnection.GetConnection();
                        conn.Open();

                        var cmd = conn.CreateCommand();
                        cmd.CommandText = @"
                        UPDATE products 
                        SET 
                            price = @price, 
                            quantity = @quantity 
                        WHERE product_id = @product_id";

                        cmd.Parameters.AddWithValue("@price", GiaCa);
                        cmd.Parameters.AddWithValue("@quantity", soLuong);
                        cmd.Parameters.AddWithValue("@product_id", product.P_Id);

                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
        }
        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            TextBox tenMonAn = new()
            {
                PlaceholderText = "Nhập tên món ăn",
                Width = 200
            };
            TextBox giaCa = new()
            {
                PlaceholderText = "Nhập giá bán (VND)",
                Width = 200
            };
            TextBox soLuong = new()
            {
                PlaceholderText = "Nhập số lượng",
                Width = 200
            };
            // Nút chọn ảnh
            Button chonAnhBtn = new()
            {
                Content = "Chọn ảnh",
                Width = 100
            };
            Image previewAnh = new()
            {
                Width = 100,
                Height = 100,
                Stretch = Stretch.UniformToFill
            };
            // StackPanel chứa nút chọn ảnh + preview
            StackPanel anhPanel = new()
            {
                Orientation = Orientation.Horizontal,
                Spacing = 10,
                Children = { chonAnhBtn, previewAnh }
            };
            var layout = new StackPanel
            {
                Spacing = 10,
                Children =
                {
                    CreateRow("Tên món ăn:", tenMonAn),
                    CreateRow("Giá cả:", giaCa),
                    CreateRow("Số lượng:", soLuong),
                    CreateRow("Ảnh món:", anhPanel)
                }
            };
            string selectedImagePath = "";

            chonAnhBtn.Click += async (s, e) =>
            {
                var picker = new Windows.Storage.Pickers.FileOpenPicker();
                picker.FileTypeFilter.Add(".png");
                picker.FileTypeFilter.Add(".jpg");

                var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(_mainWindow);
                WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);


                var file = await picker.PickSingleFileAsync();
                if (file != null)
                {
                    // Tạo folder "Images" trong LocalFolder
                    var imagesFolder = await Windows.Storage.ApplicationData.Current.LocalFolder
                                        .CreateFolderAsync("Images", Windows.Storage.CreationCollisionOption.OpenIfExists);

                    // Copy file vào folder Images
                    await file.CopyAsync(imagesFolder, file.Name, Windows.Storage.NameCollisionOption.ReplaceExisting);

                    // Lưu đường dẫn truy cập ảnh
                    selectedImagePath = $"ms-appdata:///local/Images/{file.Name}";

                    // Hiển thị preview
                    previewAnh.Source = new BitmapImage(new Uri(selectedImagePath));
                }
            };

            ContentDialog dialog = new()
            {
                Title = "Thêm món ăn mới",
                Content = layout,
                PrimaryButtonText = "Xác nhận",
                CloseButtonText = "Hủy",
                XamlRoot = this.XamlRoot
            };
            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                // Lấy số tiền nhập vào
                string name = tenMonAn.Text;
                decimal price = decimal.TryParse(giaCa.Text, out decimal GiaCa) ? GiaCa : 0;
                int quantity = int.TryParse(soLuong.Text, out int soLuongValue) ? soLuongValue : 0;
                if (!string.IsNullOrEmpty(name) && price > 0 && quantity >= 0)
                {
                    using var conn = DatabaseConnection.GetConnection();
                    conn.Open();
                    var cmd = conn.CreateCommand();
                    cmd.CommandText = @"
                    INSERT INTO products (name, price, quantity, imgPath) 
                    VALUES (@name, @price, @quantity, @imgPath)";
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@price", price);
                    cmd.Parameters.AddWithValue("@quantity", quantity);
                    cmd.Parameters.AddWithValue("@imgPath", selectedImagePath);
                    await cmd.ExecuteNonQueryAsync();
                    // Cập nhật danh sách sản phẩm
                    MyDataList.Add(new ProductModel { P_Name = name, P_Price = price, P_Quantity = quantity });
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
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is Window window)
            {
                _mainWindow = window;
            }
            else
            {
                throw new InvalidOperationException("Không nhận được Window từ Home.");
            }
        }

    }
}
