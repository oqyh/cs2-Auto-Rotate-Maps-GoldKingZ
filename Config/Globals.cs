namespace Auto_Rotate_Maps_GoldKingZ;

public class Globals
{
    public static List<string> availableMaps = new List<string>();
    public static List<string> allMaps = new List<string>();
    public static string maplist = "";
    public static string GmapName = "";
    public static string[] _lines = null!;
    public static int _currentIndex = -1;
    public static bool onetime = false;
    public static CounterStrikeSharp.API.Modules.Timers.Timer? Defaultmap;
    public static CounterStrikeSharp.API.Modules.Timers.Timer? RotationTimer;
    public static CounterStrikeSharp.API.Modules.Timers.Timer? RotationTimer2;
}