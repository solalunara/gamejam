using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Workstation : MonoBehaviour
{
    InteractPrompt pPrompt;
    void OnEnable()
    {
        pPrompt = FindObjectOfType<InteractPrompt>( true );
    }

    void OnTriggerEnter( Collider other )
    {
        if ( !pPrompt.gameObject.activeSelf )
            pPrompt.gameObject.SetActive( true );
    }
    void OnTriggerStay( Collider other )
    {
        if ( !pPrompt.gameObject.activeSelf )
            pPrompt.gameObject.SetActive( true );
    }
    void OnTriggerExit( Collider other )
    {
        pPrompt.gameObject.SetActive( false );
    }
}
