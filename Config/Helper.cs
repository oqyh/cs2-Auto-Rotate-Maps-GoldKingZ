using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Modules.Utils;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Text.Json;
using Auto_Rotate_Maps_GoldKingZ.Config;
using System.Text.Encodings.Web;
using System.Drawing;
using System.Text;

namespace Auto_Rotate_Maps_GoldKingZ;

public class Helper
{
    public static readonly HttpClient _httpClient = new HttpClient();
    public static readonly HttpClient httpClient = new HttpClient();
    public static void AdvancedPrintToChat(CCSPlayerController player, string message, params object[] args)
    {
        for (int i = 0; i < args.Length; i++)
        {
            message = message.Replace($"{{{i}}}", args[i].ToString());
        }
        if (Regex.IsMatch(message, "{nextline}", RegexOptions.IgnoreCase))
        {
            string[] parts = Regex.Split(message, "{nextline}", RegexOptions.IgnoreCase);
            foreach (string part in parts)
            {
                string messages = part.Trim();
                player.PrintToChat(" " + messages);
            }
        }else
        {
            player.PrintToChat(message);
        }
    }
    public static void AdvancedPrintToServer(string message, params object[] args)
    {
        for (int i = 0; i < args.Length; i++)
        {
            message = message.Replace($"{{{i}}}", args[i].ToString());
        }
        if (Regex.IsMatch(message, "{nextline}", RegexOptions.IgnoreCase))
        {
            string[] parts = Regex.Split(message, "{nextline}", RegexOptions.IgnoreCase);
            foreach (string part in parts)
            {
                string messages = part.Trim();
                Server.PrintToChatAll(" " + messages);
            }
        }else
        {
            Server.PrintToChatAll(message);
        }
    }
    
    public static bool IsPlayerInGroupPermission(CCSPlayerController player, string groups)
    {
        var excludedGroups = groups.Split(',');
        foreach (var group in excludedGroups)
        {
            if (group.StartsWith("#"))
            {
                if (AdminManager.PlayerInGroup(player, group))
                    return true;
            }
            else if (group.StartsWith("@"))
            {
                if (AdminManager.PlayerHasPermissions(player, group))
                    return true;
            }
        }
        return false;
    }
    public static List<CCSPlayerController> GetCounterTerroristController() 
    {
        var playerList = Utilities.FindAllEntitiesByDesignerName<CCSPlayerController>("cs_player_controller").Where(p => p != null && p.IsValid && !p.IsBot && !p.IsHLTV && p.Connected == PlayerConnectedState.PlayerConnected && p.Team == CsTeam.CounterTerrorist).ToList();
        return playerList;
    }
    public static List<CCSPlayerController> GetTerroristController() 
    {
        var playerList = Utilities.FindAllEntitiesByDesignerName<CCSPlayerController>("cs_player_controller").Where(p => p != null && p.IsValid && !p.IsBot && !p.IsHLTV && p.Connected == PlayerConnectedState.PlayerConnected && p.Team == CsTeam.Terrorist).ToList();
        return playerList;
    }
    public static List<CCSPlayerController> GetAllController() 
    {
        var playerList = Utilities.FindAllEntitiesByDesignerName<CCSPlayerController>("cs_player_controller").Where(p => p != null && p.IsValid && !p.IsBot && !p.IsHLTV && p.Connected == PlayerConnectedState.PlayerConnected).ToList();
        return playerList;
    }
    public static int GetCounterTerroristCount()
    {
        return Utilities.GetPlayers().Count(p => p != null && p.IsValid && !p.IsBot && !p.IsHLTV && p.Connected == PlayerConnectedState.PlayerConnected && p.TeamNum == (byte)CsTeam.CounterTerrorist);
    }
    public static int GetTerroristCount()
    {
        return Utilities.GetPlayers().Count(p => p != null && p.IsValid && !p.IsBot && !p.IsHLTV && p.Connected == PlayerConnectedState.PlayerConnected && p.TeamNum == (byte)CsTeam.Terrorist);
    }
    public static int GetAllCount()
    {
        return Utilities.GetPlayers().Count(p => p != null && p.IsValid && !p.IsBot && !p.IsHLTV && p.Connected == PlayerConnectedState.PlayerConnected);
    }
    public static void ClearVariables()
    {
        Globals.onetime = false;
        Globals.RotationTimer?.Kill();
        Globals.RotationTimer = null;
        Globals.RotationTimer2?.Kill();
        Globals.RotationTimer2 = null;
    }
    
    public static string ReplaceMessages(string Message, string time, string date, string map)
    {
        var replacedMessage = Message
                                    .Replace("{TIME}", time)
                                    .Replace("{DATE}", date)
                                    .Replace("{MAP}", map);
        return replacedMessage;
    }
    public static string RemoveLeadingSpaces(string content)
    {
        string[] lines = content.Split('\n');
        for (int i = 0; i < lines.Length; i++)
        {
            lines[i] = lines[i].TrimStart();
        }
        return string.Join("\n", lines);
    }
    private static CCSGameRules? GetGameRules()
    {
        try
        {
            var gameRulesEntities = Utilities.FindAllEntitiesByDesignerName<CCSGameRulesProxy>("cs_gamerules");
            return gameRulesEntities.First().GameRules;
        }
        catch
        {
            return null;
        }
    }
    public static bool IsWarmup()
    {
        return GetGameRules()?.WarmupPeriod ?? false;
    }
    public static void CreateDefaultWeaponsJson2(string filePath)
{
    var requiredHeaderLines = new List<string>
    {
        "//////////// Examples ///////////////////",
        "//Using ds: Means What map list in ds_workshop_listmaps (ex: ds:surf_boreas)",
        "//Using host: To Get Any Workshop Map example https://steamcommunity.com/sharedfiles/filedetails/?id=3112654794 (ex: host:3112654794)",
        "//Using Without any ds: or host: means what inside /../csgo/maps/  (ex: de_dust2)",
        "////VVVVVVVVVV ADD MAPS DOWN HERE VVVVVVVVVVVV////////////////"
    };

    var mapLines = new List<string>
    {
        "host:3112654794",
        "host:3070321829",
        "de_dust2",
        "de_mirage",
        "ds:surf_utopia_njv",
        "ds:surf_kitsune"
    };

    if (!File.Exists(filePath))
    {
        var defaultContent = new List<string>(requiredHeaderLines);
        defaultContent.AddRange(mapLines);
        File.WriteAllLines(filePath, defaultContent);
    }
    else
    {
        var existingLines = File.ReadAllLines(filePath).ToList();
        bool headerMissing = false;

        foreach (var headerLine in requiredHeaderLines)
        {
            if (!existingLines.Contains(headerLine))
            {
                headerMissing = true;
                int insertIndex = requiredHeaderLines.IndexOf(headerLine);
                existingLines.Insert(insertIndex, headerLine);
            }
        }

        if (headerMissing)
        {
            File.WriteAllLines(filePath, existingLines);
        }
    }
}
    public static string GetRandomMap()
    {
        if (Globals.availableMaps.Count == 0)
        {
            if (File.Exists(Globals.maplist))
            {
                string[] lines = File.ReadAllLines(Globals.maplist);
                Globals.allMaps = lines.Where(line => !line.Trim().StartsWith("//")).ToList();
                
                if (Globals.allMaps.Count > 0)
                {
                    Globals.availableMaps = new List<string>(Globals.allMaps);
                }
                else
                {
                    Console.WriteLine("|||||||||||||||||||||||||||||| E R R O R ||||||||||||||||||||||||||||||");
                    Console.WriteLine("[Auto-Rotate-Maps-GoldKingZ] RotationServerMapList.txt is empty or contains only comments");
                    Console.WriteLine("|||||||||||||||||||||||||||||| E R R O R ||||||||||||||||||||||||||||||");
                    return null!;
                }
            }
            else
            {
                Console.WriteLine("|||||||||||||||||||||||||||||| E R R O R ||||||||||||||||||||||||||||||");
                Console.WriteLine("[Auto-Rotate-Maps-GoldKingZ] RotationServerMapList.txt does not exist");
                Console.WriteLine("|||||||||||||||||||||||||||||| E R R O R ||||||||||||||||||||||||||||||");
                return null!;
            }
        }

        Random random = new Random();
        int randomIndex = random.Next(0, Globals.availableMaps.Count);
        string selectedMap = Globals.availableMaps[randomIndex];
        Globals.availableMaps.RemoveAt(randomIndex);

        return selectedMap;
    }

    public static string GetNextMap()
    {
        if (!File.Exists(Globals.maplist))
        {
            Console.WriteLine("|||||||||||||||||||||||||||||| E R R O R ||||||||||||||||||||||||||||||");
            Console.WriteLine("[Auto-Rotate-Maps-GoldKingZ] RotationServerMapList.txt does not exist");
            Console.WriteLine("|||||||||||||||||||||||||||||| E R R O R ||||||||||||||||||||||||||||||");
            return null!;
        }

        if (Globals._lines == null || Globals._currentIndex == Globals._lines.Length - 1)
        {
            string[] lines = File.ReadAllLines(Globals.maplist);
            Globals._lines = lines.Where(line => !line.Trim().StartsWith("//")).ToArray();
            Globals._currentIndex = -1;
        }

        if (Globals._lines.Length > 0)
        {
            Globals._currentIndex++;
            return Globals._lines[Globals._currentIndex];
        }
        else
        {
            Console.WriteLine("|||||||||||||||||||||||||||||| E R R O R ||||||||||||||||||||||||||||||");
            Console.WriteLine("[Auto-Rotate-Maps-GoldKingZ] RotationServerMapList.txt is empty or contains only comments");
            Console.WriteLine("|||||||||||||||||||||||||||||| E R R O R ||||||||||||||||||||||||||||||");
        }

        return null!;
    }
    public static async Task SendToDiscordWebhookNormal(string webhookUrl, string message)
    {
        try
        {
            var payload = new { content = message };
            var jsonPayload = Newtonsoft.Json.JsonConvert.SerializeObject(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(webhookUrl, content).ConfigureAwait(false);

            
        }
        catch
        {
        }
    }
    public static async Task SendToDiscordWebhookNameLinkWithPicture(string webhookUrl, string message)
    {
        try
        {
            int colorss = int.Parse(Configs.GetConfigData().DiscordLog_SideColor, System.Globalization.NumberStyles.HexNumber);
            Color color = Color.FromArgb(colorss >> 16, (colorss >> 8) & 0xFF, colorss & 0xFF);
            using (var httpClient = new HttpClient())
            {
                var embed = new
                {
                    type = "rich",
                    description = message,
                    color = color.ToArgb() & 0xFFFFFF,
                    author = new
                    {
                    }
                };

                var payload = new
                {
                    embeds = new[] { embed }
                };

                var jsonPayload = Newtonsoft.Json.JsonConvert.SerializeObject(payload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(webhookUrl, content).ConfigureAwait(false);
            }
        }
        catch
        {

        }
    }
    public static async Task SendToDiscordWebhookNameLinkWithPicture2(string webhookUrl, string message)
    {
        try
        {
            int colorss = int.Parse(Configs.GetConfigData().DiscordLog_SideColor, System.Globalization.NumberStyles.HexNumber);
            Color color = Color.FromArgb(colorss >> 16, (colorss >> 8) & 0xFF, colorss & 0xFF);

            var embed = new
            {
                type = "rich",
                color = color.ToArgb() & 0xFFFFFF,
                author = new
                {
                },
                fields = new[]
                {
                    new
                    {
                        name = "Date/Time",
                        value = DateTime.Now.ToString($"{Configs.GetConfigData().DiscordLog_DateFormat} {Configs.GetConfigData().DiscordLog_TimeFormat}"),
                        inline = false
                    },
                    new
                    {
                        name = "Message",
                        value = message,
                        inline = false
                    }
                }
            };

            var payload = new
            {
                embeds = new[] { embed }
            };

            var jsonPayload = Newtonsoft.Json.JsonConvert.SerializeObject(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(webhookUrl, content).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }
        catch
        {
            
        }
    }
    public static async Task SendToDiscordWebhookNameLinkWithPicture3(string webhookUrl, string message, string ipadress)
    {
        try
        {
            int colorss = int.Parse(Configs.GetConfigData().DiscordLog_SideColor, System.Globalization.NumberStyles.HexNumber);
            Color color = Color.FromArgb(colorss >> 16, (colorss >> 8) & 0xFF, colorss & 0xFF);

            var embed = new
            {
                type = "rich",
                color = color.ToArgb() & 0xFFFFFF,
                author = new
                {
                },
                fields = new[]
                {
                    new
                    {
                        name = "Date/Time",
                        value = DateTime.Now.ToString($"{Configs.GetConfigData().DiscordLog_DateFormat} {Configs.GetConfigData().DiscordLog_TimeFormat}"),
                        inline = false
                    },
                    new
                    {
                        name = "Message",
                        value = message,
                        inline = false
                    }
                },
                footer = new
                {
                    text = $"Server Ip: {ipadress}",
                    icon_url = $"{Configs.GetConfigData().DiscordLog_FooterImage}"
                }
            };

            var payload = new
            {
                embeds = new[] { embed }
            };

            var jsonPayload = Newtonsoft.Json.JsonConvert.SerializeObject(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(webhookUrl, content).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }
        catch
        {
            
        }
    }
    public static async Task<string> GetProfilePictureAsync(string steamId64, string defaultImage)
    {
        try
        {
            string apiUrl = $"https://steamcommunity.com/profiles/{steamId64}/?xml=1";

            HttpResponseMessage response = await httpClient.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                string xmlResponse = await response.Content.ReadAsStringAsync();
                int startIndex = xmlResponse.IndexOf("<avatarFull><![CDATA[") + "<avatarFull><![CDATA[".Length;
                int endIndex = xmlResponse.IndexOf("]]></avatarFull>", startIndex);

                if (endIndex >= 0)
                {
                    string profilePictureUrl = xmlResponse.Substring(startIndex, endIndex - startIndex);
                    return profilePictureUrl;
                }
                else
                {
                    return defaultImage;
                }
            }
            else
            {
                return null!;
            }
        }
        catch
        {
            return null!;
        }
    }
    public static string GetSteamProfileLink(string userId)
    {
        return $"https://steamcommunity.com/profiles/{userId}";
    }
    public static async Task<string> GetServerPublicIPAsync()
    {
        try
        {
            HttpResponseMessage response = await httpClient.GetAsync("https://api.ipify.org");
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            return responseBody.Trim();
        }
        catch (Exception ex)
        {
            return "Error retrieving public IP: " + ex.Message;
        }
    }
    public static void DeleteOldFiles(string folderPath, string searchPattern, TimeSpan maxAge)
    {
        try
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(folderPath);

            if (directoryInfo.Exists)
            {
                FileInfo[] files = directoryInfo.GetFiles(searchPattern);
                DateTime currentTime = DateTime.Now;
                
                foreach (FileInfo file in files)
                {
                    TimeSpan age = currentTime - file.LastWriteTime;

                    if (age > maxAge)
                    {
                        file.Delete();
                    }
                }
            }
            else
            {
                
            }
        }
        catch
        {
        }
    }
}