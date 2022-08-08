using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Documents;
using System.Windows.Media.Animation;

namespace craftersmine.ServerManagementTool.Terraria
{
    public sealed class ServerConfig
    {
        public string WorldFile { get; set; }
        public string WorldName { get; set; }
        public string WorldRoot { get; set; }
        public string Seed { get; set; }
        public string Password { get; set; }
        public string Motd { get; set; }
        public string BanlistFile { get; set; }
        public ServerWorldSize WorldSize { get; set; }
        public ServerDifficulty Difficulty { get; set; }
        public int Port { get; set; }
        public int RollbacksToKeep { get; set; }
        public int NpcStream { get; set; }
        public int MaxPlayers { get; set; }
        public bool Secure { get; set; }
        public bool EnableUpnp { get; set; }
        public bool SlowLiquids { get; set; }
        public ServerLanguage Language { get; set; }
        public ProcessPriorityClass Priority { get; set; }

        public ServerJourneyPermissionValue TimeSetSpeedPermission { get; set; }
        public ServerJourneyPermissionValue TimeSetFrozenPermission { get; set; }
        public ServerJourneyPermissionValue TimeSetDawnPermission { get; set; }
        public ServerJourneyPermissionValue TimeSetNoonPermission { get; set; }
        public ServerJourneyPermissionValue TimeSetDuskPermission { get; set; }
        public ServerJourneyPermissionValue TimeSetMidnightPermission { get; set; }
        public ServerJourneyPermissionValue GodmodePermission { get; set; }
        public ServerJourneyPermissionValue WindSetStrengthPermission { get; set; }
        public ServerJourneyPermissionValue WindSetFrozenPermission { get; set; }
        public ServerJourneyPermissionValue RainSetStrengthPermission { get; set; }
        public ServerJourneyPermissionValue RainSetFrozenPermission { get; set; }
        public ServerJourneyPermissionValue IncreasePlacementRangePermission { get; set; }
        public ServerJourneyPermissionValue SetDifficultyPermission { get; set; }
        public ServerJourneyPermissionValue BiomeSpreadSetFrozenPermission { get; set; }
        public ServerJourneyPermissionValue SetSpawnrate { get; set; }

        public void SaveToFile(string filepath)
        {
            List<string> _file = new List<string>();

            _file.Add(string.Join("=", "world", WorldFile));
            _file.Add(string.Join("=", "worldname", WorldName));
            _file.Add(string.Join("=", "worldpath", WorldRoot));
            _file.Add(string.Join("=", "autocreate", (int)WorldSize));
            _file.Add(string.Join("=", "seed", Seed));
            _file.Add(string.Join("=", "password", Password));
            _file.Add(string.Join("=", "difficulty", (int)Difficulty));
            _file.Add(string.Join("=", "maxplayers", MaxPlayers));
            _file.Add(string.Join("=", "port", Port));
            _file.Add(string.Join("=", "motd", Motd));
            _file.Add(string.Join("=", "worldrollbackstokeep", RollbacksToKeep));
            _file.Add(string.Join("=", "banlist", BanlistFile));
            _file.Add(string.Join("=", "secure", Secure ? 1 : 0));
            _file.Add(string.Join("=", "language", Language.Code));
            _file.Add(string.Join("=", "upnp", EnableUpnp ? 1 : 0));
            _file.Add(string.Join("=", "npcstream", NpcStream));
            _file.Add(string.Join("=", "priority", PriorityToInt(Priority)));
            _file.Add(string.Join("=", "slowliquids", SlowLiquids ? 1 : 0));

            _file.Add(string.Join("=", "journeypermission_time_setfrozen", (int)TimeSetFrozenPermission));
            _file.Add(string.Join("=", "journeypermission_time_setspeed", (int)TimeSetSpeedPermission));
            _file.Add(string.Join("=", "journeypermission_time_setdawn", (int)TimeSetDawnPermission));
            _file.Add(string.Join("=", "journeypermission_time_setnoon", (int)TimeSetNoonPermission));
            _file.Add(string.Join("=", "journeypermission_time_setdusk", (int)TimeSetDuskPermission));
            _file.Add(string.Join("=", "journeypermission_time_setmidnight", (int)TimeSetMidnightPermission));
            _file.Add(string.Join("=", "journeypermission_godmode", (int)GodmodePermission));
            _file.Add(string.Join("=", "journeypermission_wind_setstrength", (int)WindSetStrengthPermission));
            _file.Add(string.Join("=", "journeypermission_wind_setfrozen", (int)WindSetFrozenPermission));
            _file.Add(string.Join("=", "journeypermission_rain_setstrength", (int)RainSetStrengthPermission));
            _file.Add(string.Join("=", "journeypermission_rain_setfrozen", (int)RainSetFrozenPermission));
            _file.Add(string.Join("=", "journeypermission_increaseplacementrange", (int)IncreasePlacementRangePermission));
            _file.Add(string.Join("=", "journeypermission_setdifficulty", (int)SetDifficultyPermission));
            _file.Add(string.Join("=", "journeypermission_biomespread_setfrozen", (int)BiomeSpreadSetFrozenPermission));
            _file.Add(string.Join("=", "journeypermission_setspawnrate", (int)SetSpawnrate));

            File.WriteAllLines(filepath, _file);
        }

        public static ServerConfig LoadFromFile(string filepath)
        {
            ServerConfig cfg = new ServerConfig();

            var lines = File.ReadAllLines(filepath);
            foreach (var line in lines)
            {
                var splitLine = line.Split('=', 2, StringSplitOptions.None);

                switch (splitLine[0].ToLower())
                {
                    case "world":
                        cfg.WorldFile = splitLine[1];
                        break;
                    case "worldname":
                        cfg.WorldName = splitLine[1];
                        break;
                    case "worldpath":
                        cfg.WorldRoot = splitLine[1];
                        break;
                    case "autocreate":
                        cfg.WorldSize = (ServerWorldSize) int.Parse(splitLine[1]);
                        break;
                    case "seed":
                        cfg.Seed = splitLine[1];
                        break;
                    case "password":
                        cfg.Password = splitLine[1];
                        break;
                    case "difficulty":
                        cfg.Difficulty = (ServerDifficulty) int.Parse(splitLine[1]);
                        break;
                    case "maxplayers":
                        cfg.MaxPlayers = int.Parse(splitLine[1]);
                        break;
                    case "port":
                        cfg.Port = int.Parse(splitLine[1]);
                        break;
                    case "motd":
                        cfg.Motd = splitLine[1];
                        break;
                    case "worldrollbackstokeep":
                        cfg.RollbacksToKeep = int.Parse(splitLine[1]);
                        break;
                    case "banlist":
                        cfg.BanlistFile = splitLine[1];
                        break;
                    case "language":
                        cfg.Language = new ServerLanguage(splitLine[1]);
                        break;
                    case "upnp":
                        cfg.EnableUpnp = Convert.ToBoolean(int.Parse(splitLine[1]));
                        break;
                    case "npcstream":
                        cfg.NpcStream = int.Parse(splitLine[1]);
                        break;
                    case "priority":
                        cfg.Priority = PriorityFromInt(int.Parse(splitLine[1]));
                        break;
                    case "slowliquids":
                        cfg.SlowLiquids = Convert.ToBoolean(int.Parse(splitLine[1]));
                        break;
                    case "journeypermission_time_setfrozen":
                        cfg.TimeSetFrozenPermission = (ServerJourneyPermissionValue) int.Parse(splitLine[1]);
                        break;
                    case "journeypermission_time_setdawn":
                        cfg.TimeSetFrozenPermission = (ServerJourneyPermissionValue)int.Parse(splitLine[1]);
                        break;
                    case "journeypermission_time_setnoon":
                        cfg.TimeSetFrozenPermission = (ServerJourneyPermissionValue)int.Parse(splitLine[1]);
                        break;
                    case "journeypermission_time_setdusk":
                        cfg.TimeSetFrozenPermission = (ServerJourneyPermissionValue)int.Parse(splitLine[1]);
                        break;
                    case "journeypermission_time_setmidnight":
                        cfg.TimeSetFrozenPermission = (ServerJourneyPermissionValue)int.Parse(splitLine[1]);
                        break;
                    case "journeypermission_time_setspeed":
                        cfg.TimeSetFrozenPermission = (ServerJourneyPermissionValue)int.Parse(splitLine[1]);
                        break;
                    case "journeypermission_wind_setfrozen":
                        cfg.TimeSetFrozenPermission = (ServerJourneyPermissionValue)int.Parse(splitLine[1]);
                        break;
                    case "journeypermission_wind_setspeed":
                        cfg.TimeSetFrozenPermission = (ServerJourneyPermissionValue)int.Parse(splitLine[1]);
                        break;
                    case "journeypermission_rain_setfrozen":
                        cfg.TimeSetFrozenPermission = (ServerJourneyPermissionValue)int.Parse(splitLine[1]);
                        break;
                    case "journeypermission_rain_setspeed":
                        cfg.TimeSetFrozenPermission = (ServerJourneyPermissionValue)int.Parse(splitLine[1]);
                        break;
                    case "journeypermission_biomespread_setfrozen":
                        cfg.TimeSetFrozenPermission = (ServerJourneyPermissionValue)int.Parse(splitLine[1]);
                        break;
                    case "journeypermission_godmode":
                        cfg.TimeSetFrozenPermission = (ServerJourneyPermissionValue)int.Parse(splitLine[1]);
                        break;
                    case "journeypermission_increaseplacementrange":
                        cfg.TimeSetFrozenPermission = (ServerJourneyPermissionValue)int.Parse(splitLine[1]);
                        break;
                    case "journeypermission_setdifficulty":
                        cfg.TimeSetFrozenPermission = (ServerJourneyPermissionValue)int.Parse(splitLine[1]);
                        break;
                    case "journeypermission_setspawnrate":
                        cfg.TimeSetFrozenPermission = (ServerJourneyPermissionValue)int.Parse(splitLine[1]);
                        break;
                }
            }

            return cfg;
        }

        public static int PriorityToInt(ProcessPriorityClass priority)
        {
            return priority switch
            {
                ProcessPriorityClass.Idle => 5,
                ProcessPriorityClass.BelowNormal => 4,
                ProcessPriorityClass.Normal => 3,
                ProcessPriorityClass.AboveNormal => 2,
                ProcessPriorityClass.High => 1,
                ProcessPriorityClass.RealTime => 0,
                _ => 3
            };
        }

        public static ProcessPriorityClass PriorityFromInt(int val)
        {
            return val switch
            {
                5 => ProcessPriorityClass.Idle,
                4 => ProcessPriorityClass.BelowNormal,
                3 => ProcessPriorityClass.Normal,
                2 => ProcessPriorityClass.AboveNormal,
                1 => ProcessPriorityClass.High,
                0 => ProcessPriorityClass.RealTime,
                _ => ProcessPriorityClass.Normal
            };
        }

        public static ServerConfig GenerateDefaultConfig()
        {
            ServerConfig cfg = new ServerConfig();
            string worldRoot = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "My Games",
                "Terraria", "Server", "Worlds");
            cfg.WorldFile = Path.Combine(worldRoot, "world.wld");
            cfg.WorldSize = ServerWorldSize.Small;
            cfg.Seed = GenerateRandomSeed(16);
            cfg.WorldName = "Terraria";
            cfg.Difficulty = ServerDifficulty.Classic;
            cfg.MaxPlayers = 8;
            cfg.Port = 7777;
            cfg.Password = "";
            cfg.Motd = "Please don't cut the purple trees!";
            cfg.WorldRoot = worldRoot;
            cfg.RollbacksToKeep = 2;
            cfg.BanlistFile = "banlist.txt";
            cfg.Secure = true;
            cfg.Language = new ServerLanguage("en-US");
            cfg.EnableUpnp = true;
            cfg.NpcStream = 60;
            cfg.Priority = ProcessPriorityClass.High;
            cfg.SlowLiquids = false;
            cfg.TimeSetFrozenPermission = ServerJourneyPermissionValue.Everyone;
            cfg.TimeSetSpeedPermission = ServerJourneyPermissionValue.Everyone;
            cfg.TimeSetDawnPermission = ServerJourneyPermissionValue.Everyone;
            cfg.TimeSetNoonPermission = ServerJourneyPermissionValue.Everyone;
            cfg.TimeSetDuskPermission = ServerJourneyPermissionValue.Everyone;
            cfg.TimeSetMidnightPermission = ServerJourneyPermissionValue.Everyone;
            cfg.GodmodePermission = ServerJourneyPermissionValue.Everyone;
            cfg.WindSetFrozenPermission = ServerJourneyPermissionValue.Everyone;
            cfg.WindSetStrengthPermission = ServerJourneyPermissionValue.Everyone;
            cfg.RainSetFrozenPermission = ServerJourneyPermissionValue.Everyone;
            cfg.RainSetStrengthPermission = ServerJourneyPermissionValue.Everyone;
            cfg.IncreasePlacementRangePermission = ServerJourneyPermissionValue.Everyone;
            cfg.SetDifficultyPermission = ServerJourneyPermissionValue.Everyone;
            cfg.BiomeSpreadSetFrozenPermission = ServerJourneyPermissionValue.Everyone;
            cfg.SetSpawnrate = ServerJourneyPermissionValue.Everyone;
            return cfg;
        }

        private const string Symbols = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";

        public static string GenerateRandomSeed(int seedLength)
        {
            string seed = "";
            for (int i = 0; i < seedLength; i++)
            {
                seed += Symbols[Random.Shared.Next(Symbols.Length)];
            }

            return seed;
        }
    }

    public sealed class ServerLanguage
    {
        public string Code { get; set; }
        public string Name { get; set; }

        public static readonly ServerLanguage English = new ServerLanguage("en-US");
        public static readonly ServerLanguage French = new ServerLanguage("fr-FR");
        public static readonly ServerLanguage German = new ServerLanguage("de-DE");
        public static readonly ServerLanguage Italian = new ServerLanguage("it-IT");
        public static readonly ServerLanguage Spanish = new ServerLanguage("es-ES");
        public static readonly ServerLanguage Russian = new ServerLanguage("ru-RU");
        public static readonly ServerLanguage Chinese = new ServerLanguage("zh-Hans");
        public static readonly ServerLanguage Polish = new ServerLanguage("pl-PL");

        public ServerLanguage() {}

        public ServerLanguage(string code)
        {
            switch (code)
            {
                case "de-DE":
                    Code = "de-DE";
                    Name = "German";
                    break;
                case "it-IT":
                    Code = "it-IT";
                    Name = "Italian";
                    break;
                case "fr-FR":
                    Code = "fr-FR";
                    Name = "French";
                    break;
                case "es-ES":
                    Code = "es-ES";
                    Name = "Spanish";
                    break;
                case "ru-RU":
                    Code = "ru-RU";
                    Name = "Russian";
                    break;
                case "zh-Hans":
                    Code = "zh-Hans";
                    Name = "Chinese";
                    break;
                case "pt-PT":
                    Code = "pt-PT";
                    Name = "Portuguese";
                    break;
                case "pl-PL":
                    Code = "pl-PL";
                    Name = "Polish";
                    break;
                case "en-US":
                default:
                    Code = "en-US";
                    Name = "English";
                    break;
            }
        }

        public override string ToString()
        {
            return Name + " (" + Code + ")";
        }
    }

    public enum ServerWorldSize
    {
        Small = 1,
        Medium = 2,
        Large = 3
    }

    public enum ServerJourneyPermissionValue
    {
        LockedForEveryone = 0,
        OnlyHost = 1,
        Everyone = 2
    }

    public enum ServerDifficulty
    {
        Classic = 0,
        Expert = 1,
        Master = 2,
        Journey = 3
    }
}
