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

namespace craftersmine.ServerManagementTool.Terraria.Pages
{
    /// <summary>
    /// Логика взаимодействия для Console.xaml
    /// </summary>
    public partial class Console : Page
    {
        public Console()
        {
            InitializeComponent();
            ConsoleBox.ItemsSource = StaticData.ServerProcess?.CurrentConsole.ConsoleEntries;
            StaticData.ServerProcess!.CurrentConsole.EntryAdded += CurrentConsole_EntryAdded;
            StaticData.ServerProcess.ServerRefreshed -= ServerProcess_ServerRefreshed;
            StaticData.ServerProcess.ServerRefreshed += ServerProcess_ServerRefreshed;
            CheckServerStateAndBlockUi(StaticData.ServerProcess.ServerState);
        }

        private void ServerProcess_ServerRefreshed(object? sender, ServerInfoEventArgs e)
        {
            CheckServerStateAndBlockUi(e.ServerState);
        }

        protected override void OnInitialized(EventArgs e)
        {
            ConsoleBox.ItemsSource = StaticData.ServerProcess?.CurrentConsole.ConsoleEntries;
            StaticData.ServerProcess!.CurrentConsole.EntryAdded += CurrentConsole_EntryAdded;
            StaticData.ServerProcess.ServerRefreshed -= ServerProcess_ServerRefreshed;
            StaticData.ServerProcess.ServerRefreshed += ServerProcess_ServerRefreshed;
            CheckServerStateAndBlockUi(StaticData.ServerProcess!.ServerState);
            base.OnInitialized(e);
        }

        private void CheckServerStateAndBlockUi(ServerState state)
        {
            switch (state)
            {
                case ServerState.Running:
                    ChangeUiState(true);
                    break;
                case ServerState.Stopping:
                case ServerState.Starting:
                case ServerState.Stopped:
                    ChangeUiState(false);
                    break;
            }
        }

        private void ChangeUiState(bool state)
        {
            Dispatcher.Invoke(() =>
            {
                CommandTextBox.IsEnabled = state;
                ExecuteButton.IsEnabled = state;
            });
        }

        private void CurrentConsole_EntryAdded(object? sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                if (StaticData.ServerProcess!.CurrentConsole.ConsoleEntries.Any())
                    ConsoleBox.ScrollIntoView(StaticData.ServerProcess.CurrentConsole.ConsoleEntries[^1]);
            });
        }

        private void ExecuteButton_Click(object sender, RoutedEventArgs e)
        {
            SendCommand();
        }

        private void ScrollToBottom_Click(object sender, RoutedEventArgs e)
        {
            if (StaticData.ServerProcess!.CurrentConsole.ConsoleEntries.Any())
                ConsoleBox.ScrollIntoView(StaticData.ServerProcess.CurrentConsole.ConsoleEntries[^1]);
            ConsoleBox.SelectedItem = null;
        }

        private void CommandTextBox_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                SendCommand();
        }

        private void SendCommand()
        {
            StaticData.ServerProcess!.SendInput(CommandTextBox.Text);
            CommandTextBox.Text = "";
        }
    }
}
