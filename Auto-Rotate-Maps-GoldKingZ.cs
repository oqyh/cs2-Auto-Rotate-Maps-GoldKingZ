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
using System.Diagnostics;
using CounterStrikeSharp.API.Modules.Utils;

namespace Auto_Rotate_Maps_GoldKingZ;

[MinimumApiVersion(234)]
public class AutoRotateMapsGoldKingZ : BasePlugin
{
    public override string ModuleName => "Auto Rotate Maps (Auto Rotate Maps Depend Players In Server)";
    public override string ModuleVersion => "1.0.2";
    public override string ModuleAuthor => "Gold KingZ";
    public override string ModuleDescription => "https://github.com/oqyh";
    internal static IStringLocalizer? Stringlocalizer;
    

    public override void Load(bool hotReload)
    {
        Configs.Load(ModuleDirectory);
        Stringlocalizer = Localizer;
        Configs.Shared.CookiesModule = ModuleDirectory;
        Configs.Shared.StringLocalizer = Localizer;
        
        RegisterListener<Listeners.OnClientConnected>(OnClientConnected);
        RegisterListener<Listeners.OnClientDisconnectPost>(OnClientDisconnectPost);
        RegisterListener<Listeners.OnMapStart>(OnMapStart);
        RegisterListener<Listeners.OnMapEnd>(OnMapEnd);
        RegisterEventHandler<EventRoundStart>(OnEventRoundStart);
        RegisterEventHandler<EventRoundAnnounceWarmup>(OnEventRoundAnnounceWarmup);
        RegisterEventHandler<EventRoundAnnounceMatchStart>(OnEventRoundAnnounceMatchStart);
        RegisterEventHandler<EventCsPreRestart>(OnEventCsPreRestart);
        RegisterEventHandler<EventRoundEnd>(OnRoundEnd);

        if(Configs.GetConfigData().RotateMode == 1 || Configs.GetConfigData().RotateMode == 2)
        {
            Globals.onetime = false;
            Globals.RotationTimer?.Kill();
            Globals.RotationTimer = null;
            Globals.RotationTimer2?.Kill();
            Globals.RotationTimer2 = null;
            Globals.RotationTimer = AddTimer(0.1f, RotationTimer_Callback, TimerFlags.REPEAT);
        }

        if(Configs.GetConfigData().ForceRotateMapsOnTimelimitEndOrMaxRoundEnd)
        {
            float timeLimitInMinutes = ConVar.Find("mp_timelimit")!.GetPrimitiveValue<float>();
            Globals.mp_timelimit = (int)(timeLimitInMinutes * 60);
            Globals.ForceEndTimer?.Kill();
            Globals.ForceEndTimer = null;
            Globals.ForceEndTimer = AddTimer(0.1f, ForceEndTimer_Callback, TimerFlags.REPEAT);
            Server.ExecuteCommand($"sv_hibernate_when_empty 0");
        }
    }
    public HookResult OnEventCsPreRestart(EventCsPreRestart @event, GameEventInfo info)
    {
        if(!Configs.GetConfigData().ForceRotateMapsOnTimelimitEndOrMaxRoundEnd || @event == null)return HookResult.Continue;
        if(Globals.CTWins == 0 && Globals.TWins == 0 && !Globals.WinTeamDraw)
        {
            float timeLimitInMinutes = ConVar.Find("mp_timelimit")!.GetPrimitiveValue<float>();
            Globals.mp_timelimit = (int)(timeLimitInMinutes * 60);
            Globals.stopwatch.Start();
        }
        return HookResult.Continue;
    }

    public HookResult OnEventRoundAnnounceMatchStart(EventRoundAnnounceMatchStart @event, GameEventInfo info)
    {
        if(!Configs.GetConfigData().ForceRotateMapsOnTimelimitEndOrMaxRoundEnd || @event == null)return HookResult.Continue;
        Globals.CTWins = 0;
        Globals.TWins = 0;

        return HookResult.Continue;
    }
    public HookResult OnEventRoundAnnounceWarmup(EventRoundAnnounceWarmup @event, GameEventInfo info)
    {
        if(!Configs.GetConfigData().ForceRotateMapsOnTimelimitEndOrMaxRoundEnd || @event == null)return HookResult.Continue;
        float timeLimitInMinutes = ConVar.Find("mp_timelimit")!.GetPrimitiveValue<float>();
        Globals.mp_timelimit = (int)(timeLimitInMinutes * 60);
        Globals.stopwatch.Start();
        Globals.getvalues = false;
        return HookResult.Continue;
    }
    public HookResult OnEventRoundStart(EventRoundStart @event, GameEventInfo info)
    {
        if(!Configs.GetConfigData().ForceRotateMapsOnTimelimitEndOrMaxRoundEnd || @event == null)return HookResult.Continue;

        if(!Globals.getvalues)
        {
            float timeLimitInMinutes = ConVar.Find("mp_timelimit")!.GetPrimitiveValue<float>();
            Globals.mp_timelimit = (int)(timeLimitInMinutes * 60);
            Globals.stopwatch.Start();
            Globals.getvalues = true;
        }
        return HookResult.Continue;
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
            Globals.RotationTimer = AddTimer(0.1f, RotationTimer_Callback, TimerFlags.REPEAT);
            Server.ExecuteCommand($"sv_hibernate_when_empty 0");
        }

        if(Configs.GetConfigData().ForceRotateMapsOnTimelimitEndOrMaxRoundEnd)
        {
            float timeLimitInMinutes = ConVar.Find("mp_timelimit")!.GetPrimitiveValue<float>();
            Globals.mp_timelimit = (int)(timeLimitInMinutes * 60);
            Globals.ForceEndTimer?.Kill();
            Globals.ForceEndTimer = null;
            Globals.ForceEndTimer = AddTimer(0.1f, ForceEndTimer_Callback, TimerFlags.REPEAT);
            Server.ExecuteCommand($"sv_hibernate_when_empty 0");
        }
    }
    private void OnClientConnected(int playerSlot)
    {
        var playersCount = Helper.GetAllCount();

        if(Configs.GetConfigData().RotateMode == 1 || Configs.GetConfigData().RotateMode == 2)
        {
            Server.ExecuteCommand("sv_hibernate_when_empty 0");
            if(playersCount <= Configs.GetConfigData().RotateWhenXPlayersInServerORLess)
            {
                if(Globals.RotationTimer == null)
                {
                    Globals.onetime = false;
                    Globals.RotationTimer?.Kill();
                    Globals.RotationTimer = null;
                    Globals.RotationTimer2?.Kill();
                    Globals.RotationTimer2 = null;
                    Globals.RotationTimer = AddTimer(0.1f, RotationTimer_Callback, TimerFlags.REPEAT | TimerFlags.STOP_ON_MAPCHANGE);
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

    }
    private void OnClientDisconnectPost(int playerSlot)
    {
        var playersCount = Helper.GetAllCount();

        if(Configs.GetConfigData().RotateMode == 1 || Configs.GetConfigData().RotateMode == 2)
        {
            Server.ExecuteCommand("sv_hibernate_when_empty 0");
            if(playersCount <= Configs.GetConfigData().RotateWhenXPlayersInServerORLess)
            {
                if(Globals.RotationTimer == null)
                {
                    Globals.onetime = false;
                    Globals.RotationTimer?.Kill();
                    Globals.RotationTimer = null;
                    Globals.RotationTimer2?.Kill();
                    Globals.RotationTimer2 = null;
                    Globals.RotationTimer = AddTimer(0.1f, RotationTimer_Callback, TimerFlags.REPEAT | TimerFlags.STOP_ON_MAPCHANGE);
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
    private void ForceEndTimer_Callback()
    {
        if (Globals.mp_timelimit > 0 && !Helper.IsWarmup())
        {
            int buffer = 10;
            if (Globals.stopwatch.ElapsedMilliseconds >= 1000 - buffer)
            {
                Globals.mp_timelimit--;
                Globals.stopwatch.Restart();
            }
            
            switch (Globals.mp_timelimit)
            {
                case 300:
                    if (!Globals.fiveMinsLeftPrinted)
                    {
                        if (!string.IsNullOrEmpty(Localizer["timeleft.5mins"]))
                        {
                            Helper.AdvancedPrintToServer(Localizer["timeleft.5mins"]);
                        }
                        Globals.fiveMinsLeftPrinted = true;
                    }
                break;
                case 120:
                    if (!Globals.twoMinsLeftPrinted)
                    {
                        if (!string.IsNullOrEmpty(Localizer["timeleft.2mins"]))
                        {
                            Helper.AdvancedPrintToServer(Localizer["timeleft.2mins"]);
                        }
                        Globals.twoMinsLeftPrinted = true;
                    }
                break;
                case 60:
                    if (!Globals.oneMinLeftPrinted)
                    {
                        if (!string.IsNullOrEmpty(Localizer["timeleft.1min"]))
                        {
                            Helper.AdvancedPrintToServer(Localizer["timeleft.1min"]);
                        }
                        Globals.oneMinLeftPrinted = true;
                    }
                break;
                case 30:
                    if (!Globals.thirtySecsLeftPrinted)
                    {
                        if (!string.IsNullOrEmpty(Localizer["timeleft.30secs"]))
                        {
                            Helper.AdvancedPrintToServer(Localizer["timeleft.30secs"]);
                        }
                        Globals.thirtySecsLeftPrinted = true;
                    }
                break;
                case 15:
                    if (!Globals.fifteenSecsLeftPrinted)
                    {
                        if (!string.IsNullOrEmpty(Localizer["timeleft.15secs"]))
                        {
                            Helper.AdvancedPrintToServer(Localizer["timeleft.15secs"]);
                        }
                        Globals.fifteenSecsLeftPrinted = true;
                    }
                break;
                case 3:
                    if (!Globals.threeSecsLeftPrinted)
                    {
                        if (!string.IsNullOrEmpty(Localizer["timeleft.3secs"]))
                        {
                            Helper.AdvancedPrintToServer(Localizer["timeleft.3secs"]);
                        }
                        Globals.threeSecsLeftPrinted = true;
                    }
                break;
                case 2:
                    if (!Globals.twoSecsLeftPrinted)
                    {
                        if (!string.IsNullOrEmpty(Localizer["timeleft.2secs"]))
                        {
                            Helper.AdvancedPrintToServer(Localizer["timeleft.2secs"]);
                        }
                        Globals.twoSecsLeftPrinted = true;
                    }
                break;
                case 1:
                    if (!Globals.oneSecLeftPrinted)
                    {
                        if (!string.IsNullOrEmpty(Localizer["timeleft.1sec"]))
                        {
                            Helper.AdvancedPrintToServer(Localizer["timeleft.1sec"]);
                        }
                        if(Globals.timeisup == false)
                        {
                            Server.NextFrame(() =>
                            {
                                Globals.oneSecLeftPrinted = true;
                                Globals.timeisup = true;
                                Globals.timeisupTime = true;
                                Globals.timeisupEnd = false;
                                Globals.RotationTimer?.Kill();
                                Globals.RotationTimer = null;
                                Globals.RotationTimer2?.Kill();
                                Globals.RotationTimer2 = null;
                                Globals.RotationTimer2 = AddTimer(1.0f, RotationTimer_Callback2);
                                Globals.onetime = true;
                            });
                        }
                    }
                break;
            }
        }
    }
    private void RotationTimer_Callback2()
    {
        var playersCount = Helper.GetAllCount();

        if(playersCount <= Configs.GetConfigData().RotateWhenXPlayersInServerORLess || Globals.timeisup)
        {
            if(Configs.GetConfigData().RotateMode == 1)
            {
                if (Configs.GetConfigData().EnableSchedule)
                {
                    DateTime now = DateTime.Now;
                    string currentTime = now.ToString("HH:mm");
                    if (String.Compare(currentTime, Configs.GetConfigData().ScheduleFromTime) >= 0 &&
                        String.Compare(currentTime, Configs.GetConfigData().ScheduleToTime) < 0)
                    {
                        Globals.GmapName = Helper.GetNextMaps();
                    }else
                    {
                        Globals.GmapName = Helper.GetNextMap();
                    }
                }else
                {
                    Globals.GmapName = Helper.GetNextMap();
                }
                        
            }else if(Configs.GetConfigData().RotateMode == 2)
            {
                Globals.GmapName = Helper.GetRandomMap();

                if (Configs.GetConfigData().EnableSchedule)
                {
                    DateTime now = DateTime.Now;
                    string currentTime = now.ToString("HH:mm");
                    if (String.Compare(currentTime, Configs.GetConfigData().ScheduleFromTime) >= 0 &&
                        String.Compare(currentTime, Configs.GetConfigData().ScheduleToTime) < 0)
                    {
                        Globals.GmapName = Helper.GetRandomMaps();
                    }else
                    {
                        Globals.GmapName = Helper.GetRandomMap();
                    }
                }else
                {
                    Globals.GmapName = Helper.GetRandomMap();
                }
            }

            if(Configs.GetConfigData().TextLog_Enable && Configs.GetConfigData().RotateMode != 0 && !Globals.timeisup)
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

            if(Configs.GetConfigData().DiscordLog_EnableMode != 0 && Configs.GetConfigData().RotateMode != 0 && !Globals.timeisup)
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

            if (Globals.GmapName.StartsWith(Configs.GetConfigData().Prefix_For_Ds_Workshop_Changelevel + ":"))
            {
                string getmapname = Globals.GmapName;
                string mapName = getmapname.Split(':')[^1].Contains(":") ?getmapname.Split(':')[^1].Split(':')[2] : getmapname.Split(':')[^1];
                string map = getmapname.Contains(":") ? getmapname.Split(':')[1] : getmapname;
                if (!string.IsNullOrEmpty(Localizer["mapchange"]))
                {
                    Helper.AdvancedPrintToServer(Localizer["mapchange"], mapName);
                }
                Server.NextFrame(() =>
                {
                    if(Globals.timeisup)
                    {
                        if(Globals.timeisupTime)
                        {
                            AddTimer(Configs.GetConfigData().DelayXInSecsChangeMapOnTimelimitEnd, () =>
                            {
                                Server.ExecuteCommand($"ds_workshop_changelevel {map}");
                            });
                        }else if(Globals.timeisupEnd)
                        {
                            AddTimer(Configs.GetConfigData().DelayXInSecsChangeMapOnRoundEnd, () =>
                            {
                                Server.ExecuteCommand($"ds_workshop_changelevel {map}");
                            });
                        }
                    }else
                    {
                        AddTimer(2.0f, () =>
                        {
                            Server.ExecuteCommand($"ds_workshop_changelevel {map}");
                        });
                    }
                });
                
            }else if (Globals.GmapName.StartsWith(Configs.GetConfigData().Prefix_For_Host_Workshop_Map + ":"))
            {
                string getmapname = Globals.GmapName;
                string mapName = getmapname.Split(':')[^1].Contains(":") ?getmapname.Split(':')[^1].Split(':')[2] : getmapname.Split(':')[^1];
                string map = getmapname.Contains(":") ? getmapname.Split(':')[1] : getmapname;
                if (!string.IsNullOrEmpty(Localizer["mapchange"]))
                {
                    Helper.AdvancedPrintToServer(Localizer["mapchange"], mapName);
                }
                Server.NextFrame(() =>
                {
                    if(Globals.timeisup)
                    {
                        if(Globals.timeisupTime)
                        {
                            AddTimer(Configs.GetConfigData().DelayXInSecsChangeMapOnTimelimitEnd, () =>
                            {
                                Server.ExecuteCommand($"host_workshop_map {map}");
                            });
                        }else if(Globals.timeisupEnd)
                        {
                            AddTimer(Configs.GetConfigData().DelayXInSecsChangeMapOnRoundEnd, () =>
                            {
                                Server.ExecuteCommand($"host_workshop_map {map}");
                            });
                        }
                    }else
                    {
                        AddTimer(2.0f, () =>
                        {
                            Server.ExecuteCommand($"host_workshop_map {map}");
                        });
                    }
                });
            }else if (!(Globals.GmapName.StartsWith(Configs.GetConfigData().Prefix_For_Ds_Workshop_Changelevel + ":") || Globals.GmapName.StartsWith(Configs.GetConfigData().Prefix_For_Host_Workshop_Map + ":")))
            {
                string getmapname = Globals.GmapName;
                string mapName = getmapname.Contains(":") ? getmapname.Split(':')[^1] : getmapname;
                string map = getmapname.Contains(":") ? getmapname.Split(':')[0] : getmapname;
                if (!string.IsNullOrEmpty(Localizer["mapchange"]))
                {
                    Helper.AdvancedPrintToServer(Localizer["mapchange"], mapName);
                }
                Server.NextFrame(() =>
                {
                    if(Globals.timeisup)
                    {
                        if(Globals.timeisupTime)
                        {
                            AddTimer(Configs.GetConfigData().DelayXInSecsChangeMapOnTimelimitEnd, () =>
                            {
                                Server.ExecuteCommand($"changelevel {map}");
                            });
                        }else if(Globals.timeisupEnd)
                        {
                            AddTimer(Configs.GetConfigData().DelayXInSecsChangeMapOnRoundEnd, () =>
                            {
                                Server.ExecuteCommand($"changelevel {map}");
                            });
                        }
                    }else
                    {
                        AddTimer(2.0f, () =>
                        {
                            Server.ExecuteCommand($"changelevel {map}");
                        });
                    }
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

    private HookResult OnRoundEnd(EventRoundEnd @event, GameEventInfo info)
    {
        if(!Configs.GetConfigData().ForceRotateMapsOnTimelimitEndOrMaxRoundEnd || @event == null)return HookResult.Continue;

        var WinnerTEAM = @event.Winner;
        int MaxRounds = ConVar.Find("mp_maxrounds")!.GetPrimitiveValue<int>();
        bool canclitch = ConVar.Find("mp_match_can_clinch")!.GetPrimitiveValue<bool>();
        
        Globals.WinTeamDraw = (WinnerTEAM == 1)? true: false;
        if(WinnerTEAM == (byte)CsTeam.CounterTerrorist)
        {
            Globals.CTWins++;

        }else if(WinnerTEAM == (byte)CsTeam.Terrorist)
        {
            Globals.TWins++;
        }

        if(!canclitch && MaxRounds != 0  && (Globals.TWins + Globals.CTWins >= MaxRounds) || canclitch && MaxRounds != 0  && (Globals.TWins >= (MaxRounds/2) + 1 || Globals.CTWins >= (MaxRounds/2) + 1))
        {
            if(Globals.timeisup == false)
            {
                Server.NextFrame(() =>
                {
                    Globals.timeisup = true;
                    Globals.timeisupTime = false;
                    Globals.timeisupEnd = true;
                    Globals.onetime = true;
                    Globals.RotationTimer?.Kill();
                    Globals.RotationTimer = null;
                    Globals.RotationTimer2?.Kill();
                    Globals.RotationTimer2 = null;
                    
                    if(Configs.GetConfigData().RotateMode == 1)
                    {
                        if (Configs.GetConfigData().EnableSchedule)
                        {
                            DateTime now = DateTime.Now;
                            string currentTime = now.ToString("HH:mm");
                            if (String.Compare(currentTime, Configs.GetConfigData().ScheduleFromTime) >= 0 &&
                                String.Compare(currentTime, Configs.GetConfigData().ScheduleToTime) < 0)
                            {
                                Globals.GmapName = Helper.GetNextMaps();
                            }else
                            {
                                Globals.GmapName = Helper.GetNextMap();
                            }
                        }else
                        {
                            Globals.GmapName = Helper.GetNextMap();
                        }
                                
                    }else if(Configs.GetConfigData().RotateMode == 2)
                    {
                        Globals.GmapName = Helper.GetRandomMap();

                        if (Configs.GetConfigData().EnableSchedule)
                        {
                            DateTime now = DateTime.Now;
                            string currentTime = now.ToString("HH:mm");
                            if (String.Compare(currentTime, Configs.GetConfigData().ScheduleFromTime) >= 0 &&
                                String.Compare(currentTime, Configs.GetConfigData().ScheduleToTime) < 0)
                            {
                                Globals.GmapName = Helper.GetRandomMaps();
                            }else
                            {
                                Globals.GmapName = Helper.GetRandomMap();
                            }
                        }else
                        {
                            Globals.GmapName = Helper.GetRandomMap();
                        }
                    }

                    if (Globals.GmapName.StartsWith(Configs.GetConfigData().Prefix_For_Ds_Workshop_Changelevel + ":"))
                    {
                        string getmapname = Globals.GmapName;
                        string mapName = getmapname.Split(':')[^1].Contains(":") ?getmapname.Split(':')[^1].Split(':')[2] : getmapname.Split(':')[^1];
                        string map = getmapname.Contains(":") ? getmapname.Split(':')[1] : getmapname;
                        if (!string.IsNullOrEmpty(Localizer["mapchange"]))
                        {
                            Helper.AdvancedPrintToServer(Localizer["mapchange"], mapName);
                        }
                        Server.NextFrame(() =>
                        {
                            if(Globals.timeisup)
                            {
                                if(Globals.timeisupTime)
                                {
                                    AddTimer(Configs.GetConfigData().DelayXInSecsChangeMapOnTimelimitEnd, () =>
                                    {
                                        Server.ExecuteCommand($"ds_workshop_changelevel {map}");
                                    });
                                }else if(Globals.timeisupEnd)
                                {
                                    AddTimer(Configs.GetConfigData().DelayXInSecsChangeMapOnRoundEnd, () =>
                                    {
                                        Server.ExecuteCommand($"ds_workshop_changelevel {map}");
                                    });
                                }
                            }else
                            {
                                AddTimer(2.0f, () =>
                                {
                                    Server.ExecuteCommand($"ds_workshop_changelevel {map}");
                                });
                            }
                        });
                        
                    }else if (Globals.GmapName.StartsWith(Configs.GetConfigData().Prefix_For_Host_Workshop_Map + ":"))
                    {
                        string getmapname = Globals.GmapName;
                        string mapName = getmapname.Split(':')[^1].Contains(":") ?getmapname.Split(':')[^1].Split(':')[2] : getmapname.Split(':')[^1];
                        string map = getmapname.Contains(":") ? getmapname.Split(':')[1] : getmapname;
                        if (!string.IsNullOrEmpty(Localizer["mapchange"]))
                        {
                            Helper.AdvancedPrintToServer(Localizer["mapchange"], mapName);
                        }
                        Server.NextFrame(() =>
                        {
                            if(Globals.timeisup)
                            {
                                if(Globals.timeisupTime)
                                {
                                    AddTimer(Configs.GetConfigData().DelayXInSecsChangeMapOnTimelimitEnd, () =>
                                    {
                                        Server.ExecuteCommand($"host_workshop_map {map}");
                                    });
                                }else if(Globals.timeisupEnd)
                                {
                                    AddTimer(Configs.GetConfigData().DelayXInSecsChangeMapOnRoundEnd, () =>
                                    {
                                        Server.ExecuteCommand($"host_workshop_map {map}");
                                    });
                                }
                            }else
                            {
                                AddTimer(2.0f, () =>
                                {
                                    Server.ExecuteCommand($"host_workshop_map {map}");
                                });
                            }
                        });
                    }else if (!(Globals.GmapName.StartsWith(Configs.GetConfigData().Prefix_For_Ds_Workshop_Changelevel + ":") || Globals.GmapName.StartsWith(Configs.GetConfigData().Prefix_For_Host_Workshop_Map + ":")))
                    {
                        string getmapname = Globals.GmapName;
                        string mapName = getmapname.Contains(":") ? getmapname.Split(':')[^1] : getmapname;
                        string map = getmapname.Contains(":") ? getmapname.Split(':')[0] : getmapname;
                        if (!string.IsNullOrEmpty(Localizer["mapchange"]))
                        {
                            Helper.AdvancedPrintToServer(Localizer["mapchange"], mapName);
                        }
                        Server.NextFrame(() =>
                        {
                            if(Globals.timeisup)
                            {
                                if(Globals.timeisupTime)
                                {
                                    AddTimer(Configs.GetConfigData().DelayXInSecsChangeMapOnTimelimitEnd, () =>
                                    {
                                        Server.ExecuteCommand($"changelevel {map}");
                                    });
                                }else if(Globals.timeisupEnd)
                                {
                                    AddTimer(Configs.GetConfigData().DelayXInSecsChangeMapOnRoundEnd, () =>
                                    {
                                        Server.ExecuteCommand($"changelevel {map}");
                                    });
                                }
                            }else
                            {
                                AddTimer(2.0f, () =>
                                {
                                    Server.ExecuteCommand($"changelevel {map}");
                                });
                            }
                        });
                        
                    }
                });
            }
        }
        return HookResult.Continue;
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