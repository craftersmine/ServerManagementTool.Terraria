using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace craftersmine.ServerManagementTool.Terraria
{
    public class ServerInfoEventArgs : EventArgs
    {
        public double CpuUsage { get; set; }
        public double MemUsage { get; set; }
        public ProcessPriorityClass ProcessPriority { get; set; }
        public ServerState ServerState { get; set; }

        public string CalculateMemUsageAsString()
        {
            if (MemUsage > 1024d)
            {
                if (MemUsage / 1024d > 1024d)
                {
                    if (MemUsage / 1024d / 1024d > 1024d)
                    {
                        return string.Format("{0:F2} GB", MemUsage / 1024d / 1024d / 1024d);
                    }

                    return string.Format("{0:F2} MB", MemUsage / 1024d / 1024d);
                }

                return string.Format("{0:F2} kB", MemUsage / 1024d);
            }

            return string.Format("{0:F2} B", MemUsage);
        }
    }
}
