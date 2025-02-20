using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Workstation : MonoBehaviour
{
    public static int g_iActivePuzzles = 0;
    public Puzzle m_iType;

    PuzzleUI m_pUIElement;

    void OnEnable()
    {
        if ( m_pUIElement == null )
        {
            if ( !PuzzleUI.g_pPuzzleUIElems.ContainsKey( m_iType ) )
            {
                PuzzleUI[] pUIs = FindObjectsOfType<PuzzleUI>( true );
                foreach ( var pUI in pUIs )
                {
                    if ( pUI.m_iPuzzleID == m_iType )
                    {
                        if ( !PuzzleUI.g_pPuzzleUIElems.ContainsKey( m_iType ) )
                            PuzzleUI.g_pPuzzleUIElems.Add( m_iType, pUI );
                        else
                            throw new InvalidProgramException( "Too many puzzle UIs for puzzle " + m_iType );
                    }
                }
                if ( !PuzzleUI.g_pPuzzleUIElems.ContainsKey( m_iType ) )
                    throw new InvalidProgramException( "No puzzle UIs for puzzle " + m_iType );
            }
            m_pUIElement = PuzzleUI.g_pPuzzleUIElems[ m_iType ];
        }
    }

    void OnTriggerEnter( Collider other )
    {
        PlayerBodyController p = other.GetComponentInParent<PlayerBodyController>();
        if ( p && !m_pUIElement.Solved )
        {
            p.ActiveWorkstation = this;
            if ( !p.m_pInteractionPrompt.activeSelf && g_iActivePuzzles == 0 )
                p.m_pInteractionPrompt.SetActive( true );
        }
    }
    void OnTriggerStay( Collider other )
    {
        PlayerBodyController p = other.GetComponentInParent<PlayerBodyController>();
        if ( p && !m_pUIElement.Solved )
        {
            p.ActiveWorkstation = this;
            if ( !p.m_pInteractionPrompt.activeSelf && g_iActivePuzzles == 0 )
                p.m_pInteractionPrompt.SetActive( true );
        }
    }
    void OnTriggerExit( Collider other )
    {
        PlayerBodyController p = other.GetComponentInParent<PlayerBodyController>();
        if ( p )
        {
            p.ActiveWorkstation = null;
            p.SetPuzzleActive( false, m_iType );
            if ( p.m_pInteractionPrompt.activeSelf )
                p.m_pInteractionPrompt.SetActive( false );
        }
    }

    public void SetUIElemActive( bool bActive )
    {
        if ( m_pUIElement.Solved && bActive )
            return; //don't pop up if this puzzle has been solved

        m_pUIElement.gameObject.SetActive( bActive );
    }
}
