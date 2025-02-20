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
            PuzzleUI[] pUIElements = FindObjectsOfType<PuzzleUI>( true );
            foreach ( var pUIElement in pUIElements )
            {
                if ( pUIElement.m_iPuzzleID == m_iType )
                {
                    if ( m_pUIElement == null )
                        m_pUIElement = pUIElement;
                    else
                        throw new InvalidProgramException( "Too many puzzle UI elements for workstation " + this );
                }
            }
            if ( m_pUIElement == null )
                throw new InvalidProgramException( "No puzzle UI elements for workstation " + this );
        }
    }

    void OnTriggerEnter( Collider other )
    {
        PlayerBodyController p = other.GetComponentInParent<PlayerBodyController>();
        if ( p )
        {
            p.ActiveWorkstation = this;
            if ( !p.m_pInteractionPrompt.activeSelf && g_iActivePuzzles == 0 )
                p.m_pInteractionPrompt.SetActive( true );
        }
    }
    void OnTriggerStay( Collider other )
    {
        PlayerBodyController p = other.GetComponentInParent<PlayerBodyController>();
        if ( p )
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
        m_pUIElement.gameObject.SetActive( bActive );
        if ( bActive )
            m_pUIElement.InitPuzzle();
    }
}
