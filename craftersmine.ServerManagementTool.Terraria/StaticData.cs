using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace craftersmine.ServerManagementTool.Terraria
{
    public sealed class StaticData
    {
        public const string InstanceExtension = "tsi";

        public static string AppDataRoot => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "craftersmine\\ServerManagementConsole\\Terraria");

        public static bool HasErrorWhileLoading { get; set; }

        public static ServerInstance? CurrentServerInstance { get; set; }

        public static event EventHandler<ServerRefreshRequestedEventArgs>? RefreshRequested;
        public static TerrariaServerProcess? ServerProcess { get; set; }

        public static void RequestRefresh(bool reinitializeProcInstance = false)
        {
            RefreshRequested?.Invoke(null, new ServerRefreshRequestedEventArgs(){ReinitializeProcessInstance = reinitializeProcInstance});
        }
    }

    public sealed class ServerRefreshRequestedEventArgs : EventArgs
    {
        public bool ReinitializeProcessInstance { get; set; }
    }
}
