using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Statics;

public class FaultList : MonoBehaviour
{
    public float m_fTempDecreasePerComplete = 20.0f;
    public float m_fCharacteristicTime = 60.0f;
    public float m_fInitialTime = 60.0f;
    public int CompletedTasks => m_iCompletedTasks;
    public float FaultCount => m_pFaults.Count;
    public bool CheckFault( Puzzle p ) => m_pFaults.Contains( p );
    public int GetPuzzleIndex( Puzzle p ) => m_pFaults.IndexOf( p );
    readonly List<Puzzle> m_pFaults = new();
    bool m_bRunningCoroutine = false;
    int m_iCompletedTasks = 0;

    void OnEnable()
    {
        g_pFaultList = this;
    }

    // Update is called once per frame
    void Update()
    {
        if ( !m_bRunningCoroutine )
        {
            g_fStartTime = Time.time;
            m_bRunningCoroutine = true;
            StopAllCoroutines();
            StartCoroutine( nameof( FaultTrigger ) );
        }
    }

    public void RemoveFault( int i )
    {
        if ( i < 0 )
            throw new Exception( "please help me" );
        
        m_iCompletedTasks++;
        g_fReactorState -= m_fTempDecreasePerComplete;
        m_pFaults.RemoveAt( i );
        int iBeginLineIndex = 0;
        int iEndLineIndex = 0;
        FaultListBody[] pFLBodies = FindObjectsOfType<FaultListBody>();
        foreach ( var pFLBody in pFLBodies )
        {
            for ( int n = 0; n < pFLBody.Text.text.Length; ++n )
            {
                if ( pFLBody.Text.text[ n ] == '\n' )
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
                sNewText += pFLBody.Text.text[ ..iBeginLineIndex ] + '\n';
            if ( iEndLineIndex != pFLBody.Text.text.Length - 1 )
                sNewText += pFLBody.Text.text[ (iEndLineIndex+1).. ];
            pFLBody.Text.text = sNewText;
        }
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
            PuzzleUI pUI = g_mapPuzzleUIElems[ p ];
            pUI.InitPuzzle();
            m_pFaults.Add( p );
            FaultListBody[] pFLBodies = FindObjectsOfType<FaultListBody>();
            foreach ( var pFLBody in pFLBodies )
                pFLBody.Text.text += "- " + g_mapPuzzleNames[ p ] + "\n";
            yield return new WaitForSeconds( fTimeToNextFault );
        }
    }
}
