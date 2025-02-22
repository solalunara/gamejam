using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Statics;

public class Workstation : MonoBehaviour
{
    public Puzzle m_iType;
    public Room m_iRoom;
    public bool Solved => m_pUIElement.Solved;
    PuzzleUI m_pUIElement;
    RawImage m_pAlertAbove;

    void OnEnable()
    {
        m_pAlertAbove = GetComponentInChildren<RawImage>( true );
        if ( m_pUIElement == null )
        {
            if ( !g_mapPuzzleUIElems.ContainsKey( m_iType ) )
            {
                PuzzleUI[] pUIs = FindObjectsOfType<PuzzleUI>( true );
                foreach ( var pUI in pUIs )
                {
                    if ( pUI.m_iPuzzleID == m_iType )
                    {
                        if ( !g_mapPuzzleUIElems.ContainsKey( m_iType ) )
                            g_mapPuzzleUIElems.Add( m_iType, pUI );
                        else
                            throw new InvalidProgramException( "Too many puzzle UIs for puzzle " + m_iType );
                    }
                }
                if ( !g_mapPuzzleUIElems.ContainsKey( m_iType ) )
                    throw new InvalidProgramException( "No puzzle UIs for puzzle " + m_iType );
            }
            m_pUIElement = g_mapPuzzleUIElems[ m_iType ];
        }
        if ( !g_mapPuzzleRooms.ContainsKey( m_iType ) )
            g_mapPuzzleRooms.Add( m_iType, m_iRoom );
        else if ( g_mapPuzzleRooms[ m_iType ] != m_iRoom )
            throw new InvalidProgramException( "Differing rooms for puzzle " + m_iType );
    }

    void OnTriggerEnter( Collider other )
    {
        PlayerBodyController p = other.GetComponentInParent<PlayerBodyController>();
        if ( p && !m_pUIElement.Solved )
        {
            p.ActiveWorkstation = this;
            if ( !p.m_pInteractionPrompt.activeSelf && p.ActivePuzzles == 0 )
                p.m_pInteractionPrompt.SetActive( true );
        }
    }
    void OnTriggerStay( Collider other )
    {
        PlayerBodyController p = other.GetComponentInParent<PlayerBodyController>();
        if ( p && !m_pUIElement.Solved )
        {
            p.ActiveWorkstation = this;
            if ( !p.m_pInteractionPrompt.activeSelf && p.ActivePuzzles == 0 )
                p.m_pInteractionPrompt.SetActive( true );
        }
    }
    void OnTriggerExit( Collider other )
    {
        PlayerBodyController p = other.GetComponentInParent<PlayerBodyController>();
        if ( p )
        {
            p.ActiveWorkstation = null;
            p.SetAllPuzzlesInactive();
            if ( p.m_pInteractionPrompt.activeSelf )
                p.m_pInteractionPrompt.SetActive( false );
        }
    }

    public void SetAlertActive( bool bActive )
    {
        m_pAlertAbove.gameObject.SetActive( bActive );
    }

    public void SetUIElemActive( bool bActive )
    {
        if ( m_pUIElement.Solved && bActive )
            return; //don't pop up if this puzzle has been solved

        m_pUIElement.gameObject.SetActive( bActive );
    }
}
