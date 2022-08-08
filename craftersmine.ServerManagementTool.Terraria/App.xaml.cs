using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using craftersmine.ServerManagementTool.Terraria.Properties;
using Wpf.Ui.Controls;
using MessageBox = System.Windows.MessageBox;

namespace craftersmine.ServerManagementTool.Terraria
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            if (!Directory.Exists(StaticData.AppDataRoot))
                Directory.CreateDirectory(StaticData.AppDataRoot);

            try
            {
                StaticData.CurrentServerInstance = ServerInstance.LoadFromFile(Settings.Default.LastInstanceFile);
            }
            catch
            {
                string defaultInstancePath = Path.Combine(StaticData.AppDataRoot, "defaultInstance." + StaticData.InstanceExtension);
                StaticData.HasErrorWhileLoading = true;
                try
                {
                    StaticData.CurrentServerInstance =
                        ServerInstance.LoadFromFile(defaultInstancePath);
                }
                catch
                {
                    StaticData.CurrentServerInstance = ServerInstance.CreateDefault(defaultInstancePath);
                    Settings.Default.LastInstanceFile = defaultInstancePath;
                    Settings.Default.Save();
                    MessageBox.Show(
                        "Unable to load selected server instance. We will create a new default instance to use.",
                        "Unable to load instance!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            base.OnStartup(e);
        }
    }
}
