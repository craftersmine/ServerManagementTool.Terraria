using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wpf.Ui.Common;

namespace craftersmine.ServerManagementTool.Terraria
{
    public sealed class ServerWorld
    {
        public string WorldFilePath { get; set; }
        public string WorldName { get; set; }

        public string WorldSizeString
        {
            get
            {
                string str = WorldSize + " bytes";
                double size = WorldSize;
                if (WorldSize > 1024)
                {
                    size /= 1024;
                    str = string.Format("{0:F2} kB", size);
                    if (size > 1024)
                    {
                        size /= 1024;
                        str = string.Format("{0:F2} MB", size);
                        if (size > 1024)
                        {
                            size /= 1024;
                            str = string.Format("{0:F2} GB", size);
                        }
                    }
                }

                return str;
            }
        }

        public DateTime CreatedDate { get; set; }

        public long WorldSize { get; set; }
        public bool IsBackup { get; set; }
        public bool IsRestoreBackup { get; set; }

        public SymbolRegular Icon
        {
            get
            {
                if (IsBackup)
                    return SymbolRegular.GlobeClock16;
                if (IsRestoreBackup) 
                    return SymbolRegular.GlobeShield20;
                return SymbolRegular.Globe16;
            }
        }

        public void Delete()
        {
            if (!File.Exists(WorldFilePath))
                return;

            File.Delete(WorldFilePath);
        }

        public void Restore()
        {
            string worldsRoot = StaticData.CurrentServerInstance!.Config!.WorldRoot;
            string currentWorld = StaticData.CurrentServerInstance.Config.WorldFile;
            string curWorldBackup = Path.Combine(worldsRoot, WorldName + ".wld.wbk");

            if (File.Exists(curWorldBackup))
            {
                int lastCount = 2;
                var backupWorlds = StaticData.CurrentServerInstance.LoadWorlds().Where(w => w.IsRestoreBackup);
                foreach (var wld in backupWorlds)
                {
                    var ext = Path.GetExtension(wld.WorldFilePath);
                    if (char.IsDigit(ext[^1]))
                        lastCount = int.Parse(ext[^1].ToString());
                }

                curWorldBackup += lastCount;
            }

            if (File.Exists(currentWorld))
                File.Move(currentWorld, curWorldBackup);

            File.Move(WorldFilePath, currentWorld);
        }
    }
}
