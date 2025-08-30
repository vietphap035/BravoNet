using DACS_1.Database;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DACS_1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Thongkepage : Page
    {
        // khai báo các series và labels cho biểu đồ
        public ISeries[] NapTienSeries { get; set; }
        public string[] NapTienLabels { get; set; }

        public ISeries[] HoaDonSeries { get; set; }
        public string[] HoaDonLabels { get; set; }

        public Axis[] NapTienLabelsAxis => new[]
        {
        new Axis { Labels = NapTienLabels }
    };

        public Axis[] HoaDonLabelsAxis => new[]
        {
        new Axis { Labels = HoaDonLabels }
    };

        public Axis[] ValueAxis => new[]
        {
        new Axis { Labeler = value => value.ToString("N0") }
    };

        public Thongkepage()
        {
            InitializeComponent();

            LoadThongKe();

            this.DataContext = this;
        }

        // Hàm này sẽ được gọi khi trang được khởi tạo
        public async void LoadThongKe()
        {
            // Khởi tạo các danh sách để lưu trữ dữ liệu
            List<string> labels = new();
            List<double> tongNapList = new();
            List<double> tongHoaDonList = new();

            using var conn = DatabaseConnection.GetConnection();
            conn.Open();

            // Load nạp tiền
            var cmd1 = DatabaseConnection.CreateCommand(
                @"SELECT DATE(thoi_gian_nap) AS ngay, SUM(so_tien) AS tong_nap
          FROM nap_tien
          GROUP BY ngay
          ORDER BY ngay;", conn);
            using (var reader = cmd1.ExecuteReader())
            {
                while (reader.Read())
                {
                    string date = Convert.ToDateTime(reader["ngay"]).ToString("dd/MM/yyyy");
                    labels.Add(date);
                    tongNapList.Add(Convert.ToDouble(reader["tong_nap"]));
                }
            }

            // Load hóa đơn
            var labelsHoaDon = new List<string>();
            var tongHoaDonListTemp = new List<double>();
            var cmd2 = DatabaseConnection.CreateCommand(
                @"SELECT DATE(o.order_date) AS ngay, SUM(oi.price_at_order * oi.quantity) AS tong_hoa_don
          FROM orders o
          JOIN orders_items oi ON o.order_id = oi.order_id
          GROUP BY ngay
          ORDER BY ngay;", conn);
            using (var reader2 = cmd2.ExecuteReader())
            {
                while (reader2.Read())
                {
                    string date = Convert.ToDateTime(reader2["ngay"]).ToString("dd/MM/yyyy");
                    labelsHoaDon.Add(date);
                    tongHoaDonListTemp.Add(Convert.ToDouble(reader2["tong_hoa_don"]));
                }
            }

            // Set lên chart
            NapTienLabels = labels.ToArray();
            NapTienSeries = new ISeries[]
            {
        new ColumnSeries<double>
        {
            Values = tongNapList,
            Name = "Nạp tiền",
            Fill = new SolidColorPaint(SKColors.Green),
        }
            };

            HoaDonLabels = labelsHoaDon.ToArray();
            HoaDonSeries = new ISeries[]
            {
        new ColumnSeries<double>
        {
            Values = tongHoaDonListTemp,
            Name = "Hóa đơn"
        }
            };

            // Đảm bảo DataContext được cập nhật lại
            this.DataContext = null;
            this.DataContext = this;
        }


    }

}
