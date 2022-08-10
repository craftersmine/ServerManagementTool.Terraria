using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;
using Wpf.Ui.Controls.Interfaces;
using MessageBox = Wpf.Ui.Controls.MessageBox;

namespace craftersmine.ServerManagementTool.Terraria.Pages
{
    /// <summary>
    /// Логика взаимодействия для Worlds.xaml
    /// </summary>
    public partial class Worlds : Page
    {
        public Worlds()
        {
            InitializeComponent();
            StaticData.ServerProcess!.ServerRefreshed -= ServerProcess_ServerRefreshed;
            StaticData.ServerProcess.ServerRefreshed += ServerProcess_ServerRefreshed;
            StaticData.RefreshRequested -= StaticData_RefreshRequested;
            StaticData.RefreshRequested += StaticData_RefreshRequested;
            WorldsBox.ItemsSource = StaticData.CurrentServerInstance!.LoadWorlds();
        }

        protected override void OnInitialized(EventArgs e)
        {
            StaticData.ServerProcess!.ServerRefreshed -= ServerProcess_ServerRefreshed;
            StaticData.ServerProcess.ServerRefreshed += ServerProcess_ServerRefreshed;
            StaticData.RefreshRequested -= StaticData_RefreshRequested;
            StaticData.RefreshRequested += StaticData_RefreshRequested;
            WorldsBox.ItemsSource = StaticData.CurrentServerInstance!.LoadWorlds();
            base.OnInitialized(e);
        }

        private void RefreshWorlds()
        {
            WorldsBox.ItemsSource = null;
            WorldsBox.Items.Clear();
            WorldsBox.ItemsSource = StaticData.CurrentServerInstance!.LoadWorlds();
        }

        private void StaticData_RefreshRequested(object? sender, ServerRefreshRequestedEventArgs e)
        {
            RefreshWorlds();
        }

        private void ServerProcess_ServerRefreshed(object? sender, ServerInfoEventArgs e)
        {
            RefreshWorlds();
        }

        private void RefreshClick(object sender, RoutedEventArgs e)
        {
            RefreshWorlds();
        }

        private async void RestoreWorldClick(object sender, RoutedEventArgs e)
        {
            var world = WorldsBox.SelectedItem as ServerWorld;

            if (world is null)
                return;

            if (!world.IsBackup && !world.IsRestoreBackup)
            {
                var dlg = GetDialogHost();
                dlg.ButtonLeftVisibility = Visibility.Collapsed;
                dlg.ButtonRightAppearance = ControlAppearance.Primary;
                dlg.ButtonRightName = "Ok";
                await dlg.ShowAndWaitAsync("Can't restore world", "Selected world is a current world in use, you can't restore that world.");
                return;
            }

            try
            {
                world.Restore();
            }
            catch (Exception exception)
            {
                var dlg = GetDialogHost();
                dlg.ButtonLeftVisibility = Visibility.Collapsed;
                dlg.ButtonRightAppearance = ControlAppearance.Primary;
                dlg.ButtonRightName = "Ok";
                dlg.Show("Can't restore world",
                    "Something went wrong while restoring world file! More info: \r\n" + exception.Message);
            }

            RefreshWorlds();
        }

        private Dialog? GetDialogHost()
        {
            return (Application.Current.MainWindow?.FindName("DialogHost") as Dialog);
        }

        private async void RemoveWorldClick(object sender, RoutedEventArgs e)
        {
            var dlg = GetDialogHost();
            dlg.ButtonLeftVisibility = Visibility.Visible;
            dlg.ButtonLeftAppearance = ControlAppearance.Secondary;
            dlg.ButtonLeftName = "Yes";
            dlg.ButtonRightName = "No";
            var res = await dlg.ShowAndWaitAsync("Remove selected world", "Are you sure you want to remove selected world file? This cannot be undone!");

            if (res == IDialogControl.ButtonPressed.Right)
                return;

            try
            {
                var world = WorldsBox.SelectedItem as ServerWorld;

                if (world is null)
                    return;

                world.Delete();
            }
            catch (Exception exception)
            {
                dlg.Hide();
                dlg.ButtonLeftVisibility = Visibility.Collapsed;
                dlg.ButtonRightName = "Ok";
                dlg.Show("Can't remove world",
                    "Something went wrong while removing world file! More info: \r\n" + exception.Message);
            }

            dlg.Hide();
            RefreshWorlds();
        }

        private void OpenWorldsFolderClick(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer", StaticData.CurrentServerInstance!.Config!.WorldRoot);
        }
    }
}
