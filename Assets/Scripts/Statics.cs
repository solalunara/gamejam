using System.Collections.Generic;

public enum Puzzle
{
    NONE          = 0,
    CULT_PUZZLE   = 1<<0,
    TEMP_PUZZLE   = 1<<1,
}

public interface IPuzzleBoard
{
    public void InitBoard();
}

static class Statics
{
    public static readonly Dictionary<PlayerBodyController, CameraController> g_mapPlayerControllerMap = new();
    public static float g_fStartTime;
    public static Dictionary<Puzzle, PuzzleUI> g_mapPuzzleUIElems = new();
    public static Dictionary<Puzzle, string> g_mapPuzzleNames = new()
    {
        {Puzzle.CULT_PUZZLE, "de-polarize neogenic collector"},
        {Puzzle.TEMP_PUZZLE, "fix cadmium vacuum cruncher"}
    };
    public static FaultList g_pFaultList;
    public static float g_fReactorState = 0.0f;
}