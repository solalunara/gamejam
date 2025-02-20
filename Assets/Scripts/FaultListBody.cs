using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FaultListBody : MonoBehaviour
{
    public static float StartTime;
    public float m_fTempDecreasePerComplete = 20.0f;
    public float m_fCharacteristicTime = 60.0f;
    public float m_fInitialTime = 60.0f;
    public readonly List<Puzzle> m_pFaults = new();
    TMP_Text m_pText;
    bool m_bRunningCoroutine = false;
    void OnEnable()
    {
        m_pText = GetComponent<TMP_Text>();
        PuzzleUI.FaultList = this;
    }
    void Update()
    {
        if ( !m_bRunningCoroutine )
        {
            StartTime = Time.time;
            m_bRunningCoroutine = true;
            StopAllCoroutines();
            StartCoroutine( nameof( FaultTrigger ) );
        }
    }

    public void RemoveFault( int i )
    {
        ReactorBarController.ReactorState -= m_fTempDecreasePerComplete;
        m_pFaults.RemoveAt( i );
        int iBeginLineIndex = 0;
        int iEndLineIndex = 0;
        for ( int n = 0; n < m_pText.text.Length; ++n )
        {
            if ( m_pText.text[ n ] == '\n' )
            {
                --i;
                iBeginLineIndex = iEndLineIndex;
                iEndLineIndex = n;
                if ( i == -1 )
                    break;
            }
        }
        string sNewText = "";
        if ( iBeginLineIndex != 0 )
            sNewText += m_pText.text[ ..iBeginLineIndex ];
        if ( iEndLineIndex != m_pText.text.Length - 1 )
            sNewText += m_pText.text[ (iEndLineIndex+1).. ];
        m_pText.text = sNewText;
    }


    // IN GAME FAULTS
    IEnumerator FaultTrigger()
    {
        while ( true )
        {
            float fTimeToNextFault = m_fInitialTime * Mathf.Exp( -Time.time / m_fCharacteristicTime );
            int iNumPuzzles = Enum.GetValues(typeof(Puzzle)).Length - 1;
            int iPuzzleNum = UnityEngine.Random.Range( 0, iNumPuzzles );
            Puzzle p = (Puzzle)(1<<iPuzzleNum);
            /*
            if ( m_pFaults.Contains( p ) )
            {
                yield return new WaitForSeconds( fTimeToNextFault );
                continue;
            }
            */
            PuzzleUI pUI = PuzzleUI.g_pPuzzleUIElems[ p ];
            pUI.InitPuzzle();
            m_pFaults.Add( p );
            m_pText.text += "- " + PuzzleUI.g_pPuzzleNames[ p ] + "\n";
            yield return new WaitForSeconds( fTimeToNextFault );
        }
    }

}
