using System;
using System.Collections.Generic;
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
using craftersmine.ServerManagementTool.Terraria.Pages;
using craftersmine.ServerManagementTool.Terraria.Properties;
using Microsoft.Win32;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;
using Wpf.Ui.Controls.Interfaces;
using Wpf.Ui.Mvvm.Services;
using Path = System.IO.Path;

namespace craftersmine.ServerManagementTool.Terraria
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : UiWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            RootNavigation.Frame = RootFrame;
            Accent.ApplySystemAccent();
            var dlgSvc = new DialogService();
            dlgSvc.SetDialogControl(DialogHost);

            InstanceNameTextBlock.Text = StaticData.CurrentServerInstance!.Name;

            StaticData.RefreshRequested += StaticData_RefreshRequested;
            StaticData.RequestRefresh(true);
        }

        private void StaticData_RefreshRequested(object? sender, ServerRefreshRequestedEventArgs e)
        {
            InstanceNameTextBlock.Text = StaticData.CurrentServerInstance!.Name;
            if (e.ReinitializeProcessInstance)
                StaticData.ServerProcess = new TerrariaServerProcess(StaticData.CurrentServerInstance.ExecutablePath,
                    StaticData.CurrentServerInstance.ConfigFile);
            
            StaticData.ServerProcess!.ServerRefreshed -= ServerProcess_ServerRefreshed;
            StaticData.ServerProcess.ServerRefreshed += ServerProcess_ServerRefreshed;
            CheckServerExecutable();

            
        }

        private void ServerProcess_ServerRefreshed(object? sender, ServerInfoEventArgs e)
        {
            switch (e.ServerState)
            {
                case ServerState.Stopped:
                    ChangeUiState(true);
                    break;
                case ServerState.Running:
                case ServerState.Starting:
                case ServerState.Stopping:
                    ChangeUiState(false);
                    break;
            }
        }

        private void ChangeUiState(bool state)
        {
            Dispatcher.Invoke(() =>
            {
                InstanceMenuButton.IsEnabled = state;
            });
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            CheckServerExecutable();
        }

        private async void CheckServerExecutable()
        {
            DialogHost.ButtonLeftName = "Select server executable...";
            DialogHost.ButtonRightName = "Exit";
            DialogHost.Title = "Unable to locate server executable";
            DialogHost.Message =
                "Unable to locate Terraria Server executable. Without it you can't launch server, please locate and select executable to continue.";

            if (string.IsNullOrWhiteSpace(StaticData.CurrentServerInstance!.ExecutablePath))
            {
                var clickedBtn = await DialogHost.ShowAndWaitAsync();
                switch (clickedBtn)
                {
                    case IDialogControl.ButtonPressed.Left:
                        OpenFileDialog dlg = new OpenFileDialog();
                        dlg.Multiselect = false;
                        dlg.Filter = "Applications (*.exe)|*.exe|All Files (*.*)|*.*";
                        dlg.FileName = "TerrariaServer.exe";
                        var ok = dlg.ShowDialog();
                        if (ok.HasValue && ok.Value)
                        {
                            StaticData.CurrentServerInstance.ExecutablePath = dlg.FileName;
                            StaticData.CurrentServerInstance.SaveToFile(StaticData.CurrentServerInstance.InstanceFilePath);
                            DialogHost.Hide();
                            StaticData.RequestRefresh(true);
                        }
                        else Application.Current.Shutdown();
                        break;
                    case IDialogControl.ButtonPressed.Right:
                        Application.Current.Shutdown();
                        break;
                }
            }
        }

        private void InstanceMenuButton_OnClick(object sender, RoutedEventArgs e)
        {
            InstanceMenuButton.ContextMenu!.IsOpen = true;
        }

        private void InstanceMenuButton_OnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void EditInstance_Click(object sender, RoutedEventArgs e)
        {
            RootFrame.Navigate(new InstanceEditor(false));
        }

        private void OpenInstance_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Multiselect = false;
            dlg.DefaultExt = StaticData.InstanceExtension;
            dlg.Filter = string.Format("Terraria Server Instance (*.{0})|*.{0}|XML Files (*.xml)|*.xml|Supported Server Instances (*.{0}; *.xml)|*.{0};*.xml|All Files (*.*)|*.*", StaticData.InstanceExtension);
            dlg.Title = "Open a Terraria Server Instance";
            var res = dlg.ShowDialog();

            if (res.HasValue && res.Value)
            {
                StaticData.CurrentServerInstance = ServerInstance.LoadFromFile(dlg.FileName);
                Settings.Default.LastInstanceFile = dlg.FileName;
                Settings.Default.Save();
                StaticData.RequestRefresh();
            }
        }

        private void NewInstance_Click(object sender, RoutedEventArgs e)
        {
            RootFrame.Navigate(new InstanceEditor(true));
        }

        private void OpenDefaultInstance_Click(object sender, RoutedEventArgs e)
        {
            string defaultInstancePath = Path.Combine(StaticData.AppDataRoot, "defaultInstance." + StaticData.InstanceExtension);
            ServerInstance.CreateDefault(defaultInstancePath);
            StaticData.CurrentServerInstance = ServerInstance.LoadFromFile(defaultInstancePath);
            Settings.Default.LastInstanceFile = defaultInstancePath;
            Settings.Default.Save();
            StaticData.RequestRefresh();
        }

        private void SaveInstance_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.CreatePrompt = true;
            dlg.OverwritePrompt = true;
            dlg.DefaultExt = StaticData.InstanceExtension;
            dlg.FileName = StaticData.CurrentServerInstance!.Name + "." + StaticData.InstanceExtension;
            dlg.Filter = string.Format("Terraria Server Instance (*.{0})|*.{0}|XML Files (*.xml)|*.xml|Supported Server Instances (*.{0}; *.xml)|*.{0};*.xml|All Files (*.*)|*.*", StaticData.InstanceExtension);
            dlg.Title = "Select destination Terraria server instance file";
            var res = dlg.ShowDialog();

            if (res.HasValue && res.Value)
            {
                StaticData.CurrentServerInstance.SaveToFile(dlg.FileName);
                StaticData.CurrentServerInstance = ServerInstance.LoadFromFile(dlg.FileName);
                Settings.Default.LastInstanceFile = dlg.FileName;
                Settings.Default.Save();
                StaticData.RequestRefresh();
            }
        }
    }
}
