using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace craftersmine.ServerManagementTool.Terraria
{
    public class ServerInstance
    {
        private static XmlSerializer _serializer = new XmlSerializer(typeof(ServerInstance));
        public string ExecutablePath { get; set; }
        public string Name { get; set; }

        public ServerConfig? Config { get; set; }

        public bool UseConfigFile { get; set; }
        public string ConfigFile { get; set; }

        [XmlIgnore]
        public string InstanceFilePath { get; private set; }

        public static ServerInstance LoadFromFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Unable to read instance data file.", filePath);
            }

            using (var reader = File.OpenRead(filePath))
            {
                var instance = _serializer.Deserialize(reader) as ServerInstance;
                instance.InstanceFilePath = filePath;

                if (instance.UseConfigFile)
                    instance.Config = ServerConfig.LoadFromFile(instance.ConfigFile);
                return instance;
            }
        }

        public static ServerInstance CreateDefault(string filePath)
        {
            ServerInstance instance = new ServerInstance();
            instance.Name = "Default";
            instance.InstanceFilePath = filePath;
            instance.UseConfigFile = false;
            instance.Config = ServerConfig.GenerateDefaultConfig();
            instance.SaveToFile(filePath);

            return instance;
        }

        public void SaveToFile(string filePath)
        {
            if (UseConfigFile)
                Config = null;
            using (var writer = File.Open(filePath, FileMode.Create, FileAccess.ReadWrite))
            {
                _serializer.Serialize(writer, this);
            }
            if (UseConfigFile)
                Config = ServerConfig.LoadFromFile(ConfigFile);
        }

        public ServerWorld[] LoadWorlds()
        {
            DirectoryInfo worldRoot = new DirectoryInfo(Config!.WorldRoot);
            string worldFileName = Path.GetFileNameWithoutExtension(Config.WorldFile);
            var files = worldRoot.GetFiles(worldFileName + ".*");
            List<ServerWorld> worlds = new List<ServerWorld>();
            foreach (var file in files)
            {
                worlds.Add(new ServerWorld()
                {
                    IsBackup = file.Extension.Contains("bak"),
                    IsRestoreBackup = file.Extension.Contains("wbk"),
                    WorldFilePath = file.FullName,
                    WorldName = worldFileName,
                    WorldSize = file.Length,
                    CreatedDate = file.CreationTime
                });
            }
            return worlds.ToArray();
        }
    }
}
