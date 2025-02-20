using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleUI : MonoBehaviour
{
    public static Dictionary<Puzzle, PuzzleUI> g_pPuzzleUIElems = new();
    public static Dictionary<Puzzle, string> g_pPuzzleNames = new()
    {
        {Puzzle.CULT_PUZZLE, "de-polarize neogenic collector"}
    };
    public Puzzle m_iPuzzleID;
    public bool Solved => bSolved;

    bool bSolved = true;

    public void InitPuzzle()
    {
        bSolved = false;

        switch ( m_iPuzzleID )
        {
            case Puzzle.CULT_PUZZLE:
                GetComponentInChildren<CultPuzzleBoard>().InitBoard();
                break;
        }
    }
}
