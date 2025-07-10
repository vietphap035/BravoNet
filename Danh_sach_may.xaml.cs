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
        }
        public void DsMay_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(typeof(DanhSachMayPage));
        }

        public void DsNV_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(typeof(DanhSachNguoiDungPage));
        }
    }
}
