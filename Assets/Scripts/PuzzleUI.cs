using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleUI : MonoBehaviour
{
    public Puzzle m_iPuzzleID;

    public void InitPuzzle()
    {
        switch ( m_iPuzzleID )
        {
            case Puzzle.CULT_PUZZLE:
                GetComponentInChildren<CultPuzzleBoard>().InitBoard();
                break;
        }
    }
}
