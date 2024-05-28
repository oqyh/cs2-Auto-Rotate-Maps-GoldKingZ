using System.Drawing;
using Newtonsoft.Json;
using Auto_Rotate_Maps_GoldKingZ.Config;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using Microsoft.Extensions.Localization;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Core.Attributes;

namespace Auto_Rotate_Maps_GoldKingZ;

[MinimumApiVersion(234)]
public class AutoRotateMapsGoldKingZ : BasePlugin
{
    public override string ModuleName => "Auto Rotate Maps (Auto Rotate Maps Depend Players In Server)";
    public override string ModuleVersion => "1.0.0";
    public override string ModuleAuthor => "Gold KingZ";
    public override string ModuleDescription => "https://github.com/oqyh";

    public override void Load(bool hotReload)
    {
        Globals.maplist = Path.Combine(ModuleDirectory, "config/RotationServerMapList.txt");

        Configs.Load(ModuleDirectory);
        Configs.Shared.CookiesModule = ModuleDirectory;
        RegisterListener<Listeners.OnClientConnected>(OnClientConnected);
        RegisterListener<Listeners.OnClientDisconnectPost>(OnClientDisconnectPost);
        RegisterListener<Listeners.OnMapStart>(OnMapStart);
        RegisterListener<Listeners.OnMapEnd>(OnMapEnd);

        if(Configs.GetConfigData().RotateMode == 1 || Configs.GetConfigData().RotateMode == 2)
        {
            Globals.onetime = false;
            Globals.RotationTimer?.Kill();
            Globals.RotationTimer = null;
            Globals.RotationTimer2?.Kill();
            Globals.RotationTimer2 = null;
            Globals.RotationTimer = AddTimer(0.1f, RotationTimer_Callback, TimerFlags.REPEAT | TimerFlags.STOP_ON_MAPCHANGE);
        }
    }
    private void OnMapStart(string Map)
    {
        if(Configs.GetConfigData().TextLog_AutoDeleteLogsMoreThanXdaysOld > 0)
        {
            string Fpath = Path.Combine(ModuleDirectory,"../../plugins/Auto-Rotate-Maps-GoldKingZ/logs");
            Helper.DeleteOldFiles(Fpath, "*" + ".txt", TimeSpan.FromDays(Configs.GetConfigData().TextLog_AutoDeleteLogsMoreThanXdaysOld));
        }

        if(Configs.GetConfigData().RotateMode == 1 || Configs.GetConfigData().RotateMode == 2)
        {
            Globals.onetime = false;
            Globals.RotationTimer?.Kill();
            Globals.RotationTimer = null;
            Globals.RotationTimer2?.Kill();
            Globals.RotationTimer2 = null;
            Globals.RotationTimer = AddTimer(0.1f, RotationTimer_Callback, TimerFlags.REPEAT | TimerFlags.STOP_ON_MAPCHANGE);

            if (ConVar.Find("sv_hibernate_when_empty")!.GetPrimitiveValue<bool>())
            {
                Console.WriteLine("|||||||||||||||||||||||||||||| E R R O R ||||||||||||||||||||||||||||||");
                Console.WriteLine("[Auto-Rotate-Maps-GoldKingZ] Found (sv_hibernate_when_empty = true)");
                Console.WriteLine("[Auto-Rotate-Maps-GoldKingZ] Plugin Will Not Work Properly");
                Console.WriteLine("[Auto-Rotate-Maps-GoldKingZ] Please Make (((sv_hibernate_when_empty = false)))");
                Console.WriteLine("|||||||||||||||||||||||||||||| E R R O R ||||||||||||||||||||||||||||||");
            }
        }
    }
    private void OnClientConnected(int playerSlot)
    {
        var playersCount = Helper.GetAllCount();

        if(Configs.GetConfigData().RotateMode == 1 || Configs.GetConfigData().RotateMode == 2)
        {
            if(playersCount <= Configs.GetConfigData().RotateWhenXPlayersInServerORLess)
            {
                Globals.onetime = false;
                Globals.RotationTimer?.Kill();
                Globals.RotationTimer = null;
                Globals.RotationTimer2?.Kill();
                Globals.RotationTimer2 = null;
                Globals.RotationTimer = AddTimer(0.1f, RotationTimer_Callback, TimerFlags.REPEAT | TimerFlags.STOP_ON_MAPCHANGE);
            }else if(playersCount > Configs.GetConfigData().RotateWhenXPlayersInServerORLess)
            {
                Globals.onetime = false;
                Globals.RotationTimer?.Kill();
                Globals.RotationTimer = null;
                Globals.RotationTimer2?.Kill();
                Globals.RotationTimer2 = null;
            }
        }

    }
    private void OnClientDisconnectPost(int playerSlot)
    {
        var playersCount = Helper.GetAllCount();

        if(Configs.GetConfigData().RotateMode == 1 || Configs.GetConfigData().RotateMode == 2)
        {
            if(playersCount <= Configs.GetConfigData().RotateWhenXPlayersInServerORLess)
            {
                Globals.onetime = false;
                Globals.RotationTimer?.Kill();
                Globals.RotationTimer = null;
                Globals.RotationTimer2?.Kill();
                Globals.RotationTimer2 = null;
                Globals.RotationTimer = AddTimer(0.1f, RotationTimer_Callback, TimerFlags.REPEAT | TimerFlags.STOP_ON_MAPCHANGE);
            }else if(playersCount > Configs.GetConfigData().RotateWhenXPlayersInServerORLess)
            {
                Globals.onetime = false;
                Globals.RotationTimer?.Kill();
                Globals.RotationTimer = null;
                Globals.RotationTimer2?.Kill();
                Globals.RotationTimer2 = null;
            }
        }

    }
    private void RotationTimer_Callback()
    {
        var playersCount = Helper.GetAllCount();

        if(playersCount <= Configs.GetConfigData().RotateWhenXPlayersInServerORLess && Globals.onetime == false)
        {
            Globals.RotationTimer2 = AddTimer(Configs.GetConfigData().RotateXTimerInMins * 60, RotationTimer_Callback2, TimerFlags.STOP_ON_MAPCHANGE);
            Globals.onetime = true;
        }else if(playersCount > Configs.GetConfigData().RotateWhenXPlayersInServerORLess)
        {
            Globals.onetime = false;
            Globals.RotationTimer?.Kill();
            Globals.RotationTimer = null;
            Globals.RotationTimer2?.Kill();
            Globals.RotationTimer2 = null;
        }
    }
    private void RotationTimer_Callback2()
    {
        var playersCount = Helper.GetAllCount();

        if(playersCount <= Configs.GetConfigData().RotateWhenXPlayersInServerORLess)
        {
            if(Configs.GetConfigData().RotateMode == 1)
            {
                Globals.GmapName = Helper.GetNextMap();
            }else if(Configs.GetConfigData().RotateMode == 2)
            {
                Globals.GmapName = Helper.GetRandomMap();
            }

            if(Configs.GetConfigData().TextLog_Enable && Configs.GetConfigData().RotateMode != 0)
            {
                string Time = DateTime.Now.ToString(Configs.GetConfigData().TextLog_TimeFormat);
                string Date = DateTime.Now.ToString(Configs.GetConfigData().TextLog_DateFormat);
                var replacerlog = !string.IsNullOrEmpty(Configs.GetConfigData().TextLog_MessageFormat)
                ? Helper.ReplaceMessages(
                    Configs.GetConfigData().TextLog_MessageFormat,  
                    Time,  
                    Date,  
                    Globals.GmapName
                ): string.Empty;

                if(!string.IsNullOrEmpty(Configs.GetConfigData().TextLog_MessageFormat))
                {
                    string Fpath = Path.Combine(ModuleDirectory,"../../plugins/Auto-Rotate-Maps-GoldKingZ/logs/");
                    string fileName = DateTime.Now.ToString(Configs.GetConfigData().TextLog_DateFormat) + ".txt";
                    string Tpath = Path.Combine(ModuleDirectory,"../../plugins/Auto-Rotate-Maps-GoldKingZ/logs/") + $"{fileName}";
                    if(!Directory.Exists(Fpath))
                    {
                        Directory.CreateDirectory(Fpath);
                    }

                    if(!File.Exists(Tpath))
                    {
                        using (File.Create(Tpath)) { }
                    }
                    try
                    {
                        File.AppendAllLines(Tpath, new[]{replacerlog});
                    }catch
                    {

                    }
                }
            }

            if(Configs.GetConfigData().DiscordLog_EnableMode != 0 && Configs.GetConfigData().RotateMode != 0)
            {
                
                string Time = DateTime.Now.ToString(Configs.GetConfigData().DiscordLog_TimeFormat);
                string Date = DateTime.Now.ToString(Configs.GetConfigData().DiscordLog_DateFormat);
                int hostPort = ConVar.Find("hostport")!.GetPrimitiveValue<int>();
                var replacerDiscord = !string.IsNullOrEmpty(Configs.GetConfigData().DiscordLog_MessageFormat)
                ? Helper.ReplaceMessages(
                    Configs.GetConfigData().DiscordLog_MessageFormat,  
                    Time,  
                    Date,  
                    Globals.GmapName
                ): string.Empty;
                
                if(!string.IsNullOrEmpty(replacerDiscord))
                {
                    if(Configs.GetConfigData().DiscordLog_EnableMode == 1)
                    {
                        Task.Run(() =>
                        {
                            _ = Helper.SendToDiscordWebhookNormal(Configs.GetConfigData().DiscordLog_WebHookURL, replacerDiscord);
                        });
                    }else if(Configs.GetConfigData().DiscordLog_EnableMode == 2)
                    {
                        Task.Run(() =>
                        {
                            _ = Helper.SendToDiscordWebhookNameLinkWithPicture2(Configs.GetConfigData().DiscordLog_WebHookURL, replacerDiscord);
                        });
                    }else if(Configs.GetConfigData().DiscordLog_EnableMode == 3)
                    {
                        Task.Run(() =>
                        {
                            string serverIp = Helper.GetServerPublicIPAsync().Result;
                            _ = Helper.SendToDiscordWebhookNameLinkWithPicture3(Configs.GetConfigData().DiscordLog_WebHookURL, replacerDiscord, $"{serverIp}:{hostPort}");
                        });
                    }
                }
                
            }

            if (Globals.GmapName.StartsWith("ds:") )
            {
                Server.NextFrame(() =>
                {
                    AddTimer(2.00f, () =>
                    {
                        string dsworkshop = Globals.GmapName.TrimStart().Substring("ds:".Length).Trim();
                        Server.ExecuteCommand($"ds_workshop_changelevel {dsworkshop}");
                    }, TimerFlags.STOP_ON_MAPCHANGE);
                });
                
            }else if (Globals.GmapName.StartsWith("host:"))
            {
                Server.NextFrame(() =>
                {
                    AddTimer(2.00f, () =>
                    {
                        string hostworkshop = Globals.GmapName.TrimStart().Substring("host:".Length).Trim();
                        Server.ExecuteCommand($"host_workshop_map {hostworkshop}");
                    }, TimerFlags.STOP_ON_MAPCHANGE);
                });
            }else if (!(Globals.GmapName.StartsWith("ds:") || Globals.GmapName.StartsWith("host:")))
            {
                Server.NextFrame(() =>
                {
                    AddTimer(2.00f, () =>
                    {
                        Server.ExecuteCommand($"changelevel {Globals.GmapName}");
                    }, TimerFlags.STOP_ON_MAPCHANGE);
                });
                
            }

        }else if(playersCount > Configs.GetConfigData().RotateWhenXPlayersInServerORLess)
        {
            Globals.onetime = false;
            Globals.RotationTimer?.Kill();
            Globals.RotationTimer = null;
            Globals.RotationTimer2?.Kill();
            Globals.RotationTimer2 = null;
        }
    }
    private void OnMapEnd()
    {
        Helper.ClearVariables();
    }
    public override void Unload(bool hotReload)
    {
        Helper.ClearVariables();
    }
}