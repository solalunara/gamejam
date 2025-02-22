using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static Statics;

public class PuzzleUI : MonoBehaviour
{
    public Puzzle m_iPuzzleID;
    public bool Solved => !g_pFaultList.CheckFault( m_iPuzzleID );

    public void InitPuzzle()
    {
        GetComponentInChildren<PuzzleBoard>().InitBoard();
    }

    public void Resolve()
    {
        GetComponentInParent<ScreenUI>().pPlayerOwner.SetAllPuzzlesInactive();
        g_pFaultList.RemoveFault( g_pFaultList.GetPuzzleIndex( m_iPuzzleID ), true );
        if ( !Solved )
            InitPuzzle();
    }
}
