using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Workstation : MonoBehaviour
{
    public static int g_iActivePuzzles = 0;
    public Puzzle m_iType;

    void OnTriggerEnter( Collider other )
    {
        PlayerBodyController p = other.GetComponentInParent<PlayerBodyController>();
        if ( p )
        {
            p.ActiveWorkstation = this;
            if ( !p.m_pInteractionPrompt.activeSelf )
                p.m_pInteractionPrompt.SetActive( true );
        }
    }
    void OnTriggerStay( Collider other )
    {
        PlayerBodyController p = other.GetComponentInParent<PlayerBodyController>();
        if ( p )
        {
            p.ActiveWorkstation = this;
            if ( !p.m_pInteractionPrompt.activeSelf )
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
}
