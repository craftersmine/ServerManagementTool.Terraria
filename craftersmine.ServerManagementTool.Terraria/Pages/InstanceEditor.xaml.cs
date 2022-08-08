using System;
using System.Collections.Generic;
using System.IO;
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
using Microsoft.Win32;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;
using Wpf.Ui.Mvvm.Services;

namespace craftersmine.ServerManagementTool.Terraria.Pages
{
    public partial class InstanceEditor : Page
    {
        private bool _isCreatingInstance = false;

        public InstanceEditor(bool isCreatingInstance)
        {
            _isCreatingInstance = isCreatingInstance;
            InitializeComponent();
            if (!isCreatingInstance)
            {
                InstanceNameTextBox.Text = StaticData.CurrentServerInstance!.Name;
                ServerExecutablePathTextBox.Text = StaticData.CurrentServerInstance.ExecutablePath;
                if (!string.IsNullOrWhiteSpace(ServerExecutablePathTextBox.Text))
                    SaveButton.IsEnabled = true;
            }
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            NavigateBackToHome();
        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!File.Exists(ServerExecutablePathTextBox.Text))
            {
                var dlgHost = GetDialogHost();
                dlgHost!.ButtonLeftVisibility = Visibility.Collapsed;
                dlgHost.ButtonRightName = "Ok";
                dlgHost.ButtonRightAppearance = ControlAppearance.Primary;
                dlgHost.ButtonRightClick += DlgHost_ButtonRightClick;
                dlgHost.Show("Unable to locate executable.",
                    "Unable to locate selected server executable at specified path! Please check if path is valid and try again.");
                return;
            }

            if (string.IsNullOrWhiteSpace(InstanceNameTextBox.Text))
                InstanceNameTextBox.Text = "unnamed";

            if (!_isCreatingInstance)
            {
                StaticData.CurrentServerInstance!.ExecutablePath = ServerExecutablePathTextBox.Text;
                StaticData.CurrentServerInstance.Name = InstanceNameTextBox.Text;
                StaticData.CurrentServerInstance.SaveToFile(StaticData.CurrentServerInstance.InstanceFilePath);
                StaticData.RequestRefresh();
                NavigateBackToHome();
            }
            else
            {
                ServerInstance instance = new ServerInstance();
                instance.Name = InstanceNameTextBox.Text;
                instance.ExecutablePath = ServerExecutablePathTextBox.Text;

                SaveFileDialog dlg = new SaveFileDialog();
                dlg.OverwritePrompt = true;
                dlg.FileName = InstanceNameTextBox.Text + "." + StaticData.InstanceExtension;
                dlg.Filter = string.Format("Terraria Server Instance (*.{0})|*.{0}|XML Files (*.xml)|*.xml|Supported Server Instances (*.{0}; *.xml)|*.{0};*.xml|All Files (*.*)|*.*", StaticData.InstanceExtension);
                dlg.Title = "Select a destination for instance file";
                dlg.AddExtension = true;
                dlg.ValidateNames = true;

                var res = dlg.ShowDialog();
                if (res.HasValue && res.Value)
                {
                    instance.SaveToFile(dlg.FileName);
                    StaticData.CurrentServerInstance = ServerInstance.LoadFromFile(dlg.FileName);
                    StaticData.RequestRefresh();
                    NavigateBackToHome();
                }
            }
        }

        private void DlgHost_ButtonRightClick(object sender, RoutedEventArgs e)
        {
            (sender as Dialog)?.Hide();
        }

        private void NavigateBackToHome()
        {
            (Application.Current.MainWindow?.FindName("RootFrame") as Frame)?.Navigate(new Home());
        }

        private Dialog? GetDialogHost()
        {
            return (Application.Current.MainWindow?.FindName("DialogHost") as Dialog);
        }

        private void BrowseForExecutableButton_OnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Multiselect = false;
            dlg.Filter =
                "Applications (*.exe)|*.exe|Known server executables|TerrariaServer.exe;tModLoaderServer.exe|All Files (*.*)|*.*";
            dlg.Title = "Select server executable file";
            dlg.FilterIndex = 1;
            var result = dlg.ShowDialog();
            if (result.HasValue && result.Value)
            {
                ServerExecutablePathTextBox.Text = dlg.FileName;
            }
        }

        private void TextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (ServerExecutablePathTextBox.Text.Length > 0 && ConfigFilePathTextBox.Text.Length > 0)
                SaveButton.IsEnabled = true;
            else SaveButton.IsEnabled = false;
        }

        private void BrowseForConfigFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Multiselect = false;
            dlg.Filter =
                "Terraria Config Files (*.txt, *.cfg)|*.txt;*.cfg|Terraria Default Config File|serverconfig.txt|All Files (*.*)|*.*";
            dlg.Title = "Select server config file";
            dlg.FilterIndex = 1;
            var result = dlg.ShowDialog();
            if (result.HasValue && result.Value)
            {
                ConfigFilePathTextBox.Text = dlg.FileName;
            }
        }
    }
}
