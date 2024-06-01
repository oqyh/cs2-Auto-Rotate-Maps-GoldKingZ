using System.Diagnostics;

namespace Auto_Rotate_Maps_GoldKingZ;

public class Globals
{
    public static List<string> availableMaps = new List<string>();
    public static List<string> allMaps = new List<string>();
    public static List<string> availableMapss = new List<string>();
    public static List<string> allMapss = new List<string>();
    public static Stopwatch stopwatch = new Stopwatch();
    public static string maplist = "";
    public static string GmapName = "";
    public static string[] _lines = null!;
    public static string[] _liness = null!;
    public static int CTWins = 0;
    public static int TWins = 0;
    public static int _currentIndex = -1;
    public static int _currentIndexs = -1;
    public static int mp_timelimit = 0;
    public static bool WinTeamDraw = false;
    public static bool onetime = false;
    public static bool getvalues = false;
    public static bool timeisup = false;
    public static bool timeisupTime = false;
    public static bool timeisupEnd = false;
    public static bool fiveMinsLeftPrinted = false;
    public static bool twoMinsLeftPrinted = false;
    public static bool oneMinLeftPrinted = false;
    public static bool thirtySecsLeftPrinted = false;
    public static bool fifteenSecsLeftPrinted = false;
    public static bool threeSecsLeftPrinted = false;
    public static bool twoSecsLeftPrinted = false;
    public static bool oneSecLeftPrinted = false;
    public static CounterStrikeSharp.API.Modules.Timers.Timer? Defaultmap;
    public static CounterStrikeSharp.API.Modules.Timers.Timer? RotationTimer;
    public static CounterStrikeSharp.API.Modules.Timers.Timer? RotationTimer2;
    public static CounterStrikeSharp.API.Modules.Timers.Timer? ForceEndTimer;
}