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
using Wpf.Ui.Controls;

namespace craftersmine.ServerManagementTool.Terraria.Pages
{
    public partial class Home : UiPage
    {
        public Home()
        {
            InitializeComponent();
            StaticData.ServerProcess!.ServerRefreshed -= ServerProcess_ServerRefreshed;
            StaticData.ServerProcess.ServerRefreshed += ServerProcess_ServerRefreshed;
            InstanceStateTextBlock.Text = StaticData.ServerProcess.ServerState.ToString();
            InstanceNameTextBlock.Text = StaticData.CurrentServerInstance!.Name;
            InstanceExecutableTextBlock.Text = StaticData.CurrentServerInstance.ExecutablePath;
            CpuRing.Progress = 0;
            CpuUsageTextBlock.Text = $"CPU: 0.00%";
            MemUsageTextBlock.Text = "0 B";
            ProcessPriorityTextBlock.Text = "Idle";
            StaticData.RefreshRequested -= StaticData_RefreshRequested;
            StaticData.RefreshRequested += StaticData_RefreshRequested;
        }

        private void StaticData_RefreshRequested(object? sender, ServerRefreshRequestedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                InstanceStateTextBlock.Text = StaticData.ServerProcess!.ServerState.ToString();
                InstanceNameTextBlock.Text = StaticData.CurrentServerInstance!.Name;
                InstanceExecutableTextBlock.Text = StaticData.CurrentServerInstance.ExecutablePath;
                CpuRing.Progress = 0;
                CpuUsageTextBlock.Text = $"CPU: 0.00%";
                MemUsageTextBlock.Text = "0 B";
                ProcessPriorityTextBlock.Text = "Idle";
            });
        }

        private void ServerProcess_ServerRefreshed(object? sender, ServerInfoEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                CpuRing.Progress = e.CpuUsage;
                CpuUsageTextBlock.Text = $"CPU: {e.CpuUsage:F2} %";
                MemUsageTextBlock.Text = e.CalculateMemUsageAsString();
                ProcessPriorityTextBlock.Text = e.ProcessPriority.ToString();
                InstanceStateTextBlock.Text = e.ServerState.ToString();
                InstanceNameTextBlock.Text = StaticData.CurrentServerInstance!.Name;
                InstanceExecutableTextBlock.Text = StaticData.CurrentServerInstance.ExecutablePath;

                switch (e.ServerState)
                {
                    case ServerState.Running:
                        LaunchButton.IsEnabled = false;
                        KillButton.IsEnabled = true;
                        StopButton.IsEnabled = true;
                        break;
                    case ServerState.Starting:
                        LaunchButton.IsEnabled = false;
                        KillButton.IsEnabled = true;
                        StopButton.IsEnabled = false;
                        break;
                    case ServerState.Stopped:
                        LaunchButton.IsEnabled = true;
                        KillButton.IsEnabled = false;
                        StopButton.IsEnabled = false;
                        break;
                    case ServerState.Stopping:
                        LaunchButton.IsEnabled = false;
                        KillButton.IsEnabled = true;
                        StopButton.IsEnabled = false;
                        break;
                }
            });
        }

        private void LaunchInstance_Click(object sender, RoutedEventArgs e)
        {
            StaticData.ServerProcess?.Run();
        }

        private void StopInstance_Click(object sender, RoutedEventArgs e)
        {
            StaticData.ServerProcess?.Stop(true);
        }

        private void KillButton_OnClick(object sender, RoutedEventArgs e)
        {
            StaticData.ServerProcess?.Kill();
        }
    }
}
