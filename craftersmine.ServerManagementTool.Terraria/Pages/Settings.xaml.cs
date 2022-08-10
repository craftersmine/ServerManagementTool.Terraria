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
using Microsoft.Win32;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;

namespace craftersmine.ServerManagementTool.Terraria.Pages
{
    /// <summary>
    /// Логика взаимодействия для Settings.xaml
    /// </summary>
    public partial class Settings : Page
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void TextChangedCommonEventHandler(object sender, TextChangedEventArgs e)
        {
            if (ExecutablePathTextBox.Text.Length > 0)
                SaveButton.IsEnabled = true;
            else
                SaveButton.IsEnabled = false;
            
        }

        protected override void OnInitialized(EventArgs e)
        {
            ExecutablePathTextBox.Text = Properties.Settings.Default
                .ServerExecutablePath;

            base.OnInitialized(e);
        }

        private void BrowseForWorldButtonClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Multiselect = false;
            dlg.Filter = "Applications (*.exe)|*.exe|All Files (*.*)|*.*";
            dlg.FileName = "TerrariaServer.exe";
            var ok = dlg.ShowDialog();
            if (ok.HasValue && ok.Value)
            {
                Properties.Settings.Default.ServerExecutablePath = dlg.FileName;
            }
        }

        private void ShowSnackbar(string message)
        {
            var snackbar = (Application.Current.MainWindow?.FindName("Snackbar") as Snackbar);
            snackbar!.Title = "Settings";
            snackbar.Message = message;
            snackbar.Icon = SymbolRegular.Checkmark20;
            snackbar.Show();
        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.ServerExecutablePath = ExecutablePathTextBox.Text;
            Properties.Settings.Default.Save();
            ShowSnackbar("Settings saved successfully!");
        }
    }
}
