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
    public static FaultListBody FaultList;
    public Puzzle m_iPuzzleID;
    public bool Solved => !FaultList.m_pFaults.Contains( m_iPuzzleID );

    public void InitPuzzle()
    {
        switch ( m_iPuzzleID )
        {
            case Puzzle.CULT_PUZZLE:
                GetComponentInChildren<CultPuzzleBoard>().InitBoard();
                break;
        }
    }

    public void Resolve()
    {
        FaultList.RemoveFault( FaultList.m_pFaults.IndexOf( m_iPuzzleID ) );
        if ( !Solved )
            InitPuzzle();
    }
}
