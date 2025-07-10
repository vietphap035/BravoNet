using DACS_1.Database;
using DACS_1.Model;
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
            Machines = [];
            this.DataContext = this;

            var loadedMachines = LoadMachines();
            foreach (var machine in loadedMachines)
            {
                Machines.Add(machine);
            }
        }
        private List<Machine> LoadMachines()
        {
            List<Machine> pcList = [];
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                var cmd = DatabaseConnection.CreateCommand("SELECT pc_code, UId, is_active FROM pc", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Machine pc = new()
                        {
                            Id = reader["pc_code"].ToString(),
                            UId = reader["UId"]?.ToString(),
                            ImagePath = reader.GetBoolean("is_active")
                                ? "ms-appx:///Assets/ACTIVE.png"
                                : "ms-appx:///Assets/NOT_ACTIVE.png"
                        };
                        pcList.Add(pc);
                    }
                }

                return pcList;
            }
        }
        private async void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var clickedMachine = e.ClickedItem as Machine;
            ContentDialog successDialog = new()
            {
                Title = "Login Successful",
                Content = clickedMachine?.Id,
                CloseButtonText = "OK",
                XamlRoot = this.Content.XamlRoot
            };
            await successDialog.ShowAsync();
        }
    }
}
