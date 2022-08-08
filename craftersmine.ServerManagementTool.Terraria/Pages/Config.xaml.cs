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
using Microsoft.Win32;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;

namespace craftersmine.ServerManagementTool.Terraria.Pages
{
    /// <summary>
    /// Логика взаимодействия для Config.xaml
    /// </summary>
    public partial class Config : Page
    {
        public Config()
        {
            InitializeComponent();

            LanguageComboBox.Items.Add(ServerLanguage.English);
            LanguageComboBox.Items.Add(ServerLanguage.Russian);
            LanguageComboBox.Items.Add(ServerLanguage.German);
            LanguageComboBox.Items.Add(ServerLanguage.French);
            LanguageComboBox.Items.Add(ServerLanguage.Spanish);
            LanguageComboBox.Items.Add(ServerLanguage.Italian);
            LanguageComboBox.Items.Add(ServerLanguage.Polish);
            LanguageComboBox.Items.Add(ServerLanguage.Chinese);

            WorldSizeComboBox.SelectedIndex = (int)StaticData.CurrentServerInstance!.Config!.WorldSize;
            DifficultyComboBox.SelectedIndex = (int)StaticData.CurrentServerInstance.Config!.Difficulty;
            LanguageComboBox.SelectedItem = StaticData.CurrentServerInstance.Config!.Language;
            PriorityComboBox.SelectedIndex =
                ServerConfig.PriorityToInt(StaticData.CurrentServerInstance.Config.Priority);

            TimeSetFrozenPermComboBox.SelectedIndex =
                (int) StaticData.CurrentServerInstance.Config.TimeSetFrozenPermission;
            TimeSetSpeedPermComboBox.SelectedIndex =
                (int)StaticData.CurrentServerInstance.Config.TimeSetSpeedPermission;
            TimeSetDawnPermComboBox.SelectedIndex =
                (int)StaticData.CurrentServerInstance.Config.TimeSetDawnPermission;
            TimeSetNoonPermComboBox.SelectedIndex =
                (int)StaticData.CurrentServerInstance.Config.TimeSetNoonPermission;
            TimeSetDuskPermComboBox.SelectedIndex =
                (int)StaticData.CurrentServerInstance.Config.TimeSetDuskPermission;
            TimeSetMidnightPermComboBox.SelectedIndex =
                (int)StaticData.CurrentServerInstance.Config.TimeSetMidnightPermission;
            WindSetFrozenPermComboBox.SelectedIndex =
                (int)StaticData.CurrentServerInstance.Config.WindSetFrozenPermission;
            WindSetStrengthPermComboBox.SelectedIndex =
                (int)StaticData.CurrentServerInstance.Config.WindSetStrengthPermission;
            RainSetFrozenPermComboBox.SelectedIndex =
                (int)StaticData.CurrentServerInstance.Config.RainSetFrozenPermission;
            RainSetStrengthPermComboBox.SelectedIndex =
                (int)StaticData.CurrentServerInstance.Config.RainSetStrengthPermission;
            GodmodePermComboBox.SelectedIndex =
                (int)StaticData.CurrentServerInstance.Config.GodmodePermission;
            IncreasePlacementRangePermComboBox.SelectedIndex =
                (int)StaticData.CurrentServerInstance.Config.IncreasePlacementRangePermission;
            SetDifficultyPermComboBox.SelectedIndex =
                (int)StaticData.CurrentServerInstance.Config.SetDifficultyPermission;
            BiomeSpreadSetFrozenPermComboBox.SelectedIndex =
                (int)StaticData.CurrentServerInstance.Config.BiomeSpreadSetFrozenPermission;
            SetSpawnratePermComboBox.SelectedIndex =
                (int)StaticData.CurrentServerInstance.Config.SetSpawnrate;

            StaticData.ServerProcess!.ServerRefreshed -= ServerProcess_ServerRefreshed;
            StaticData.ServerProcess.ServerRefreshed += ServerProcess_ServerRefreshed;
            CheckServerStateAndBlockUi(StaticData.ServerProcess.ServerState);
        }

        protected override void OnInitialized(EventArgs e)
        {
            StaticData.ServerProcess!.ServerRefreshed -= ServerProcess_ServerRefreshed;
            StaticData.ServerProcess.ServerRefreshed += ServerProcess_ServerRefreshed;
            CheckServerStateAndBlockUi(StaticData.ServerProcess!.ServerState);
            base.OnInitialized(e);
        }

        private void CheckServerStateAndBlockUi(ServerState state)
        {
            switch (state)
            {
                case ServerState.Running:
                    ChangeUiState(false);
                    break;
                case ServerState.Stopping:
                    ChangeUiState(false);
                    break;
                case ServerState.Starting:
                    ChangeUiState(false);
                    break;
                case ServerState.Stopped:
                    ChangeUiState(true);
                    break;
            }
        }

        private void ChangeUiState(bool state)
        {
            Dispatcher.Invoke(() =>
            {
                SaveButton.IsEnabled = state;
                WorldFileTextBox.IsEnabled = state;
                WorldNameTextBox.IsEnabled = state;
                WorldSizeComboBox.IsEnabled = state;
                BrowseForWorldButton.IsEnabled = state;
                SeedTextBox.IsEnabled = state;
                GenerateSeedButton.IsEnabled = state;
                ServerPasswordBox.IsEnabled = state;
                DifficultyComboBox.IsEnabled = state;
                MaxPlayersNumBox.IsEnabled = state;
                PortNumBox.IsEnabled = state;
                MotdTextBox.IsEnabled = state;
                RollbacksToKeepNumBox.IsEnabled = state;
                BanlistFileTextBox.IsEnabled = state;
                BrowseForBanlistFileButton.IsEnabled = state;
                SecureCheckBox.IsEnabled = state;
                LanguageComboBox.IsEnabled = state;
                EnableUpnpCheckBox.IsEnabled = state;
                NpcStreamNumBox.IsEnabled = state;
                PriorityComboBox.IsEnabled = state;
                SlowLiquidsCheckBox.IsEnabled = state;
                TimeSetFrozenPermComboBox.IsEnabled = state;
                TimeSetSpeedPermComboBox.IsEnabled = state;
                TimeSetDawnPermComboBox.IsEnabled = state;
                TimeSetNoonPermComboBox.IsEnabled = state;
                TimeSetDuskPermComboBox.IsEnabled = state;
                TimeSetMidnightPermComboBox.IsEnabled = state;
                WindSetFrozenPermComboBox.IsEnabled = state;
                WindSetStrengthPermComboBox.IsEnabled = state;
                RainSetFrozenPermComboBox.IsEnabled = state;
                RainSetStrengthPermComboBox.IsEnabled = state;
                GodmodePermComboBox.IsEnabled = state;
                IncreasePlacementRangePermComboBox.IsEnabled = state;
                BiomeSpreadSetFrozenPermComboBox.IsEnabled = state;
                SetDifficultyPermComboBox.IsEnabled = state;
                SetSpawnratePermComboBox.IsEnabled = state;
            });
        }

        private void ServerProcess_ServerRefreshed(object? sender, ServerInfoEventArgs e)
        {
            CheckServerStateAndBlockUi(e.ServerState);
        }

        private void TextChangedCommonEventHandler(object sender, TextChangedEventArgs e)
        {
            if (WorldFileTextBox is null || WorldNameTextBox is null || SaveButton is null)
                return;

            if (WorldFileTextBox.Text.Length > 0 && WorldNameTextBox.Text.Length > 0)
                SaveButton.IsEnabled = true;
            else SaveButton.IsEnabled = false;
        }

        private void BrowseForWorldButtonClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Multiselect = false;
            dlg.Filter = "Terraria Worlds (*.wld)|*.wld|All Files (*.*)|*.*";
            dlg.Title = "Select world file";
            var res = dlg.ShowDialog();
            if (res.HasValue && res.Value)
                WorldFileTextBox.Text = dlg.FileName;
        }

        private void GenerateSeedClick(object sender, RoutedEventArgs e)
        {
            SeedTextBox.Text = ServerConfig.GenerateRandomSeed(16);
        }

        private void BrowseForBanlistFileButton_OnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Multiselect = false;
            dlg.Filter = "Terraria Banlist File (banlist.txt)|banlist.txt|Supported Files (*.txt)|*.txt|All Files (*.*)|*.*";
            dlg.Title = "Select banlist file";
            var res = dlg.ShowDialog();
            if (res.HasValue && res.Value)
                WorldFileTextBox.Text = dlg.FileName;
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            NavigateBackToHome();
        }

        private void NavigateBackToHome()
        {
            (Application.Current.MainWindow?.FindName("RootFrame") as Frame)?.Navigate(new Home());
        }

        private void ShowSnackbar(string message)
        {
            var snackbar = (Application.Current.MainWindow?.FindName("Snackbar") as Snackbar);
            snackbar!.Title = "Configuration manager";
            snackbar.Message = message;
            snackbar.Icon = SymbolRegular.Checkmark20;
            snackbar.Show();
        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            StaticData.CurrentServerInstance!.Config!.WorldFile = WorldFileTextBox.Text;
            StaticData.CurrentServerInstance.Config.WorldRoot =
                System.IO.Path.GetDirectoryName(StaticData.CurrentServerInstance.Config.WorldFile)!;
            StaticData.CurrentServerInstance.Config.WorldName = WorldNameTextBox.Text;
            StaticData.CurrentServerInstance.Config.WorldSize =
                (ServerWorldSize)(WorldSizeComboBox.SelectedItem as ComboBoxItem)!.Tag;
            StaticData.CurrentServerInstance.Config.Seed = SeedTextBox.Text;
            StaticData.CurrentServerInstance.Config.Password = ServerPasswordBox.Text;
            StaticData.CurrentServerInstance.Config.Difficulty =
                (ServerDifficulty) (DifficultyComboBox.SelectedItem as ComboBoxItem)!.Tag;
            StaticData.CurrentServerInstance.Config.MaxPlayers = (int)MaxPlayersNumBox.Value;
            StaticData.CurrentServerInstance.Config.Motd = MotdTextBox.Text;
            StaticData.CurrentServerInstance.Config.Port = (int)PortNumBox.Value;
            StaticData.CurrentServerInstance.Config.RollbacksToKeep = (int) RollbacksToKeepNumBox.Value;
            StaticData.CurrentServerInstance.Config.BanlistFile = BanlistFileTextBox.Text;
            StaticData.CurrentServerInstance.Config.Secure = SecureCheckBox.IsChecked!.Value;
            StaticData.CurrentServerInstance.Config.Language = LanguageComboBox.SelectedItem as ServerLanguage ?? ServerLanguage.English;
            StaticData.CurrentServerInstance.Config.EnableUpnp = EnableUpnpCheckBox.IsChecked!.Value;
            StaticData.CurrentServerInstance.Config.NpcStream = (int)NpcStreamNumBox.Value;
            StaticData.CurrentServerInstance.Config.Priority =
                (ProcessPriorityClass) (PriorityComboBox.SelectedItem as ComboBoxItem)!.Tag;
            StaticData.CurrentServerInstance.Config.SlowLiquids = SlowLiquidsCheckBox.IsChecked!.Value;

            StaticData.CurrentServerInstance.Config.TimeSetFrozenPermission =
                (ServerJourneyPermissionValue)(TimeSetFrozenPermComboBox.SelectedItem as ComboBoxItem)!.Tag;
            StaticData.CurrentServerInstance.Config.TimeSetDawnPermission =
                (ServerJourneyPermissionValue)(TimeSetDawnPermComboBox.SelectedItem as ComboBoxItem)!.Tag;
            StaticData.CurrentServerInstance.Config.TimeSetNoonPermission =
                (ServerJourneyPermissionValue)(TimeSetNoonPermComboBox.SelectedItem as ComboBoxItem)!.Tag;
            StaticData.CurrentServerInstance.Config.TimeSetDuskPermission =
                (ServerJourneyPermissionValue)(TimeSetDuskPermComboBox.SelectedItem as ComboBoxItem)!.Tag;
            StaticData.CurrentServerInstance.Config.TimeSetMidnightPermission =
                (ServerJourneyPermissionValue)(TimeSetMidnightPermComboBox.SelectedItem as ComboBoxItem)!.Tag;
            StaticData.CurrentServerInstance.Config.TimeSetSpeedPermission =
                (ServerJourneyPermissionValue)(TimeSetSpeedPermComboBox.SelectedItem as ComboBoxItem)!.Tag;
            StaticData.CurrentServerInstance.Config.WindSetFrozenPermission =
                (ServerJourneyPermissionValue)(WindSetFrozenPermComboBox.SelectedItem as ComboBoxItem)!.Tag;
            StaticData.CurrentServerInstance.Config.WindSetStrengthPermission =
                (ServerJourneyPermissionValue)(WindSetStrengthPermComboBox.SelectedItem as ComboBoxItem)!.Tag;
            StaticData.CurrentServerInstance.Config.RainSetFrozenPermission =
                (ServerJourneyPermissionValue)(RainSetFrozenPermComboBox.SelectedItem as ComboBoxItem)!.Tag;
            StaticData.CurrentServerInstance.Config.RainSetStrengthPermission =
                (ServerJourneyPermissionValue)(RainSetStrengthPermComboBox.SelectedItem as ComboBoxItem)!.Tag;
            StaticData.CurrentServerInstance.Config.GodmodePermission =
                (ServerJourneyPermissionValue)(GodmodePermComboBox.SelectedItem as ComboBoxItem)!.Tag;
            StaticData.CurrentServerInstance.Config.IncreasePlacementRangePermission =
                (ServerJourneyPermissionValue)(IncreasePlacementRangePermComboBox.SelectedItem as ComboBoxItem)!.Tag;
            StaticData.CurrentServerInstance.Config.SetDifficultyPermission =
                (ServerJourneyPermissionValue)(SetDifficultyPermComboBox.SelectedItem as ComboBoxItem)!.Tag;
            StaticData.CurrentServerInstance.Config.BiomeSpreadSetFrozenPermission =
                (ServerJourneyPermissionValue)(BiomeSpreadSetFrozenPermComboBox.SelectedItem as ComboBoxItem)!.Tag;
            StaticData.CurrentServerInstance.Config.SetSpawnrate =
                (ServerJourneyPermissionValue)(SetSpawnratePermComboBox.SelectedItem as ComboBoxItem)!.Tag;

            if (StaticData.CurrentServerInstance.UseConfigFile)
                StaticData.CurrentServerInstance.Config.SaveToFile(StaticData.CurrentServerInstance.ConfigFile);
            else
            {
                StaticData.CurrentServerInstance.Config.SaveToFile(System.IO.Path.Combine(StaticData.AppDataRoot,
                    "tempConfig.cfg"));
                StaticData.CurrentServerInstance.SaveToFile(StaticData.CurrentServerInstance.InstanceFilePath);
            }

            ShowSnackbar("Server Configuration has been saved!");
        }
    }
}
