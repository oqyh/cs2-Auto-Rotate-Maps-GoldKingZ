using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Localization;

namespace Auto_Rotate_Maps_GoldKingZ.Config
{
    public static class Configs
    {
        public static class Shared {
            public static string? CookiesModule { get; set; }
            public static IStringLocalizer? StringLocalizer { get; set; }
        }
        
        private static readonly string ConfigDirectoryName = "config";
        private static readonly string ConfigFileName = "config.json";
        private static readonly string FilePath = "RotationServerMapList.txt";
        private static readonly string FilePath2 = "RotationServerMapListSchedule.txt";
        private static string? _FilePath;
        private static string? _FilePath2;
        private static string? _configFilePath;
        private static ConfigData? _configData;

        private static readonly JsonSerializerOptions SerializationOptions = new()
        {
            Converters =
            {
                new JsonStringEnumConverter()
            },
            WriteIndented = true,
            AllowTrailingCommas = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
        };

        public static bool IsLoaded()
        {
            return _configData is not null;
        }

        public static ConfigData GetConfigData()
        {
            if (_configData is null)
            {
                throw new Exception("Config not yet loaded.");
            }
            
            return _configData;
        }

        public static ConfigData Load(string modulePath)
        {
            var configFileDirectory = Path.Combine(modulePath, ConfigDirectoryName);
            if(!Directory.Exists(configFileDirectory))
            {
                Directory.CreateDirectory(configFileDirectory);
            }

            _FilePath = Path.Combine(configFileDirectory, FilePath);
            Helper.CreateDefaultWeaponsJson(_FilePath);

            _FilePath2 = Path.Combine(configFileDirectory, FilePath2);
            Helper.CreateDefaultWeaponsJson2(_FilePath2);
            
            _configFilePath = Path.Combine(configFileDirectory, ConfigFileName);
            if (File.Exists(_configFilePath))
            {
                _configData = JsonSerializer.Deserialize<ConfigData>(File.ReadAllText(_configFilePath), SerializationOptions);
            }
            else
            {
                _configData = new ConfigData();
            }

            if (_configData is null)
            {
                throw new Exception("Failed to load configs.");
            }

            SaveConfigData(_configData);
            
            return _configData;
        }

        private static void SaveConfigData(ConfigData configData)
        {
            if (_configFilePath is null)
            {
                throw new Exception("Config not yet loaded.");
            }
            string json = JsonSerializer.Serialize(configData, SerializationOptions);


            File.WriteAllText(_configFilePath, json);
        }

        public class ConfigData
        {
            public string Load_MapList_Path { get; set; }
            private string? _Prefix_For_Ds_Workshop_Changelevel;

            public string Prefix_For_Ds_Workshop_Changelevel
            {
                get => _Prefix_For_Ds_Workshop_Changelevel!;
                set
                {
                    _Prefix_For_Ds_Workshop_Changelevel = value;
                    if (value.Contains(":"))
                    {
                        _Prefix_For_Ds_Workshop_Changelevel = value.Replace(":", "");
                    }
                }
            }

            private string? _Prefix_For_Host_Workshop_Map;

            public string Prefix_For_Host_Workshop_Map
            {
                get => _Prefix_For_Host_Workshop_Map!;
                set
                {
                    _Prefix_For_Host_Workshop_Map = value;
                    if (value.Contains(":"))
                    {
                        _Prefix_For_Host_Workshop_Map = value.Replace(":", "");
                    }
                }
            }

            
            private int _RotateMode;
            public int RotateMode
            {
                get => _RotateMode;
                set
                {
                    _RotateMode = value;
                    if (_RotateMode < 0 || _RotateMode > 2)
                    {
                        RotateMode = 1;
                        Console.WriteLine("|||||||||||||||||||||||||||||||||||||||||||||||| I N V A L I D ||||||||||||||||||||||||||||||||||||||||||||||||");
                        Console.WriteLine("[Auto-Rotate-Maps-GoldKingZ] RotateMode: is invalid, setting to default value (1) Please Choose 0 or 1 or 2 or 3.");
                        Console.WriteLine("[Auto-Rotate-Maps-GoldKingZ] RotateMode (0) = Disable");
                        Console.WriteLine("[Auto-Rotate-Maps-GoldKingZ] RotateMode (1) = Get Maps From Top To Bottom In RotationServerMapList.txt");
                        Console.WriteLine("[Auto-Rotate-Maps-GoldKingZ] RotateMode (2) = Get Random Maps In RotationServerMapList.txt");
                        Console.WriteLine("|||||||||||||||||||||||||||||||||||||||||||||||| I N V A L I D ||||||||||||||||||||||||||||||||||||||||||||||||");
                    }
                }
            }
            public float RotateXTimerInMins { get; set; }
            public int RotateWhenXPlayersInServerORLess { get; set; }
            public bool ForceRotateMapsOnTimelimitEndOrMaxRoundEnd { get; set; }
            public float DelayXInSecsChangeMapOnTimelimitEnd { get; set; }
            public float DelayXInSecsChangeMapOnRoundEnd { get; set; }
            public bool EnableSchedule { get; set; }
            public string ScheduleFromTime { get; set; }
            public string ScheduleToTime { get; set; }
            public string Schedule_MapList_Path { get; set; }

            public string empty { get; set; }
            public bool TextLog_Enable { get; set; }
            public string TextLog_MessageFormat { get; set; }
            public string TextLog_DateFormat { get; set; }
            public string TextLog_TimeFormat { get; set; }
            public int TextLog_AutoDeleteLogsMoreThanXdaysOld { get; set; }
            public string empty2 { get; set; }
            private int _DiscordLog_EnableMode;
            public int DiscordLog_EnableMode
            {
                get => _DiscordLog_EnableMode;
                set
                {
                    _DiscordLog_EnableMode = value;
                    if (_DiscordLog_EnableMode < 0 || _DiscordLog_EnableMode > 3)
                    {
                        DiscordLog_EnableMode = 0;
                        Console.WriteLine("|||||||||||||||||||||||||||||||||||||||||||||||| I N V A L I D ||||||||||||||||||||||||||||||||||||||||||||||||");
                        Console.WriteLine("[Auto-Rotate-Maps-GoldKingZ] DiscordLog_EnableMode: is invalid, setting to default value (0) Please Choose 0 or 1 or 2 or 3.");
                        Console.WriteLine("[Auto-Rotate-Maps-GoldKingZ] DiscordLog_EnableMode (0) = Disable");
                        Console.WriteLine("[Auto-Rotate-Maps-GoldKingZ] DiscordLog_EnableMode (1) = Text Only");
                        Console.WriteLine("[Auto-Rotate-Maps-GoldKingZ] DiscordLog_EnableMode (2) = Text With Saparate Date And Time From Message");
                        Console.WriteLine("[Auto-Rotate-Maps-GoldKingZ] DiscordLog_EnableMode (3) = Text With Saparate Date And Time From Message + Server Ip In Footer");
                        Console.WriteLine("|||||||||||||||||||||||||||||||||||||||||||||||| I N V A L I D ||||||||||||||||||||||||||||||||||||||||||||||||");
                    }
                }
            }
            public string DiscordLog_MessageFormat { get; set; }
            public string DiscordLog_DateFormat { get; set; }
            public string DiscordLog_TimeFormat { get; set; }
            private string? _DiscordLog_SideColor;
            public string DiscordLog_SideColor
            {
                get => _DiscordLog_SideColor!;
                set
                {
                    _DiscordLog_SideColor = value;
                    if (_DiscordLog_SideColor.StartsWith("#"))
                    {
                        DiscordLog_SideColor = _DiscordLog_SideColor.Substring(1);
                    }
                }
            }
            public string DiscordLog_WebHookURL { get; set; }
            public string DiscordLog_FooterImage { get; set; }
            public string empty3 { get; set; }
            public string Information_For_You_Dont_Delete_it { get; set; }
            
            public ConfigData()
            {
                Load_MapList_Path = "csgo/addons/counterstrikesharp/plugins/Auto-Rotate-Maps-GoldKingZ/config/RotationServerMapList.txt";
                Prefix_For_Ds_Workshop_Changelevel = "ds";
                Prefix_For_Host_Workshop_Map = "host";
                RotateMode = 1;
                RotateXTimerInMins = 5.0f;
                RotateWhenXPlayersInServerORLess = 0;
                ForceRotateMapsOnTimelimitEndOrMaxRoundEnd = false;
                DelayXInSecsChangeMapOnTimelimitEnd = 2.0f;
                DelayXInSecsChangeMapOnRoundEnd = 0.0f;
                EnableSchedule = false;
                ScheduleFromTime = "01:00";
                ScheduleToTime = "06:00";
                Schedule_MapList_Path = "csgo/addons/counterstrikesharp/plugins/Auto-Rotate-Maps-GoldKingZ/config/RotationServerMapListSchedule.txt";
                empty = "-----------------------------------------------------------------------------------";
                TextLog_Enable = false;
                TextLog_MessageFormat = "[{DATE} - {TIME}] Server Has Less Players Changing Map To [{MAP}]";
                TextLog_DateFormat = "MM-dd-yyyy";
                TextLog_TimeFormat = "HH:mm:ss";
                TextLog_AutoDeleteLogsMoreThanXdaysOld = 0;
                empty2 = "-----------------------------------------------------------------------------------";
                DiscordLog_EnableMode = 0;
                DiscordLog_MessageFormat = "[{DATE} - {TIME}] Server Has Less Players Changing Map To [{MAP}]";
                DiscordLog_DateFormat = "MM-dd-yyyy";
                DiscordLog_TimeFormat = "HH:mm:ss";
                DiscordLog_SideColor = "00FFFF";
                DiscordLog_WebHookURL = "https://discord.com/api/webhooks/XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX";
                DiscordLog_FooterImage = "https://github.com/oqyh/cs2-Auto-Rotate-Maps-GoldKingZ/blob/main/Resources/serverip.png?raw=true";
                empty3 = "-----------------------------------------------------------------------------------";
                Information_For_You_Dont_Delete_it = " Vist  [https://github.com/oqyh/cs2-Auto-Rotate-Maps-GoldKingZ/tree/main?tab=readme-ov-file#-configuration-] To Understand All Above";
            }
        }
    }
}