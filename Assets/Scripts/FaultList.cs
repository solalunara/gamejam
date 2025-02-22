using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using static Statics;

public class FaultList : MonoBehaviour
{
    public float m_fPuzzleTime = 20.0f;
    public float m_fStartTime = 10.0f;
    public float m_fWaitBeforeSameTaskTime = 5.0f;
    public float m_fTempDecreasePerComplete = 20.0f;
    public float m_fTempIncreasePerFail = 10.0f;
    public float m_fCharacteristicTime = 60.0f;
    public float m_fInitialTime = 60.0f;
    public int CompletedTasks => m_iCompletedTasks;
    public float FaultCount => m_pFaults.Count;
    public bool CheckFault( Puzzle p ) => m_pFaults.Where( e => e.Item1 == p ).Any() || ( m_bBunkerActive && p == Puzzle.BUNKER_PUZZLE );
    public int GetPuzzleIndex( Puzzle p ) => m_pFaults.IndexOf( m_pFaults.Where( e => e.Item1 == p ).First() );
    readonly List<(Puzzle,float)> m_pFaults = new();
    readonly List<(Puzzle,float)> m_pRecentlyCompletedTasks = new();
    readonly HashSet<Puzzle> m_pDisabledPuzzles = new();
    bool m_bRunningCoroutine = false;
    bool m_bBunkerActive = false;
    int m_iCompletedTasks = 0;

    void OnEnable()
    {
        g_pFaultList = this;
        m_fStartTime += Time.time;
        m_pDisabledPuzzles.Add( Puzzle.NONE );
        m_pDisabledPuzzles.Add( Puzzle.BUNKER_PUZZLE );
    }

    // Update is called once per frame
    void Update()
    {
        if ( !m_bRunningCoroutine && Time.time > m_fStartTime )
        {
            g_fStartTime = Time.time;
            m_bRunningCoroutine = true;
            StopAllCoroutines();
            StartCoroutine( nameof( FaultTrigger ) );
        }

        for ( int i = m_pFaults.Count; --i >= 0; )
        {
            float fNewTime = m_pFaults[ i ].Item2 - Time.deltaTime;
            if ( fNewTime < 0 )
            {
                RemoveFault( i, false );
                continue;
            }
            m_pFaults[ i ] = (m_pFaults[ i ].Item1, fNewTime);
        }
        for ( int i = m_pRecentlyCompletedTasks.Count; --i >= 0; )
        {
            float fNewTime = m_pRecentlyCompletedTasks[ i ].Item2 + Time.deltaTime;
            if ( fNewTime > m_fWaitBeforeSameTaskTime )
            {
                m_pRecentlyCompletedTasks.RemoveAt( i );
                continue;
            }
            m_pRecentlyCompletedTasks[ i ] = (m_pRecentlyCompletedTasks[ i ].Item1, fNewTime);
        }

    }

    public void RemoveFault( int i, bool bSuccess )
    {
        if ( i < 0 )
            throw new ArgumentOutOfRangeException(nameof(i));
        
        if ( bSuccess )
            m_iCompletedTasks++;
        g_fReactorState += !bSuccess ? m_fTempIncreasePerFail : -m_fTempDecreasePerComplete;
        m_pRecentlyCompletedTasks.Add( (m_pFaults[ i ].Item1, 0.0f) );
        Room r = g_mapPuzzleRooms[ m_pFaults[ i ].Item1 ];
        foreach ( var pFlash in FindObjectsOfType<Flash>( true ) )
            if ( pFlash.m_iRoom == r )
                pFlash.gameObject.SetActive( false );
        foreach ( var pWorkstation in FindObjectsOfType<Workstation>() )
            if ( pWorkstation.m_iType == m_pFaults[ i ].Item1 )
                pWorkstation.SetAlertActive( false );
        foreach ( var pPlayer in FindObjectsOfType<PlayerBodyController>() )
            pPlayer.SetPuzzleInactive( m_pFaults[ i ].Item1 );
        m_pFaults.RemoveAt( i );
        m_pFaults.Sort();
    }

    public void ForEach( Action<(Puzzle,float)> pFn )
    {
        foreach ( var fault in m_pFaults )
            pFn( fault );
    }

    public void SetPuzzleEnabled( Puzzle p, bool bEnabled )
    {
        if ( bEnabled && m_pDisabledPuzzles.Contains( p ) )
            m_pDisabledPuzzles.Remove( p );
        else if ( !bEnabled && ! m_pDisabledPuzzles.Contains( p ) )
            m_pDisabledPuzzles.Add( p );
    }

    public void ActivateFault( Puzzle p )
    {
        // only init bunker puzzle once
        if ( p == Puzzle.BUNKER_PUZZLE )
        {
            if ( m_bBunkerActive )
                return;
            m_bBunkerActive = true;
        }

        PuzzleUI pUI = g_mapPuzzleUIElems[ p ];
        pUI.InitPuzzle();
        if ( p != Puzzle.BUNKER_PUZZLE )
        {
            m_pFaults.Add( (p, m_fPuzzleTime) );
            m_pFaults.Sort();
        }
        Room r = g_mapPuzzleRooms[ p ];
        if ( p != Puzzle.BUNKER_PUZZLE )
            foreach ( var pFlash in FindObjectsOfType<Flash>( true ) )
                if ( pFlash.m_iRoom == r )
                    pFlash.gameObject.SetActive( true );
        foreach ( var pWorkstation in FindObjectsOfType<Workstation>() )
            if ( pWorkstation.m_iType == p )
                pWorkstation.SetAlertActive( true );
    }

    // IN GAME FAULTS
    IEnumerator FaultTrigger()
    {
        while ( true )
        {
            float fTimeToNextFault = m_fInitialTime * Mathf.Exp( -( Time.time - g_fStartTime ) / m_fCharacteristicTime );
            List<Puzzle> pAvaliableFaults = new();
            foreach ( Puzzle pPuzzle in Enum.GetValues( typeof( Puzzle ) ) )
                if ( !m_pDisabledPuzzles.Contains( pPuzzle ) &&
                     !CheckFault( pPuzzle ) && 
                     !m_pRecentlyCompletedTasks.Where( e => e.Item1 == pPuzzle ).Any() 
                    )
                    pAvaliableFaults.Add( pPuzzle );

            if ( !pAvaliableFaults.Any() )
            {
                yield return new WaitForSeconds( fTimeToNextFault );
                continue;
            }

            int iPuzzleNum = UnityEngine.Random.Range( 0, pAvaliableFaults.Count );
            ActivateFault( pAvaliableFaults[ iPuzzleNum ] );
            yield return new WaitForSeconds( fTimeToNextFault );
        }
    }
}
