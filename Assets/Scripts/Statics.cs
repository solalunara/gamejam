using System.Collections.Generic;
using UnityEngine;

public enum Puzzle
{
    NONE          = 0,
    CULT_PUZZLE   = 1<<0,
    BOILER_PUZZLE = 1<<1,
    WIFI_PUZZLE   = 1<<2,
    BUNKER_PUZZLE = 1<<3,
    CODE_PUZZLE = 1<<4
}

public enum Room
{
    NONE        = 0,
    LEFT        = 1<<0,
    RIGHT       = 1<<1,
    BOTTOM      = 1<<2,
    CONTROL     = 1<<3,
    REACTOR     = 1<<4
}

public abstract class PuzzleBoard : MonoBehaviour
{
    public abstract void InitBoard();
    public void Resolve() => GetComponentInParent<PuzzleUI>().Resolve();
}

static class Statics
{
    public static readonly Dictionary<PlayerBodyController, CameraController> g_mapPlayerControllerMap = new();
    public static float g_fStartTime;
    public static Dictionary<Puzzle, PuzzleUI> g_mapPuzzleUIElems = new();
    public static Dictionary<Puzzle, Room> g_mapPuzzleRooms = new();
    public static Dictionary<Puzzle, Workstation> g_mapWorkstations = new();
    public static Dictionary<Puzzle, string> g_mapPuzzleNames = new()
    {
        {Puzzle.CULT_PUZZLE, "De-polarise neogenic collector"},
        {Puzzle.BOILER_PUZZLE, "Fix cadmium vacuum cruncher"},
        {Puzzle.WIFI_PUZZLE, "Restart wifi router"},
        {Puzzle.CODE_PUZZLE, "Recalibrate mainframe capacitance"}
    };
    public static FaultList g_pFaultList;
    public static float g_fReactorState = 0.0f;
    public static bool g_bMadeItToBunker = false;
}