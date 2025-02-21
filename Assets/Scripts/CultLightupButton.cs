using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CultLightupButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool m_bChannel2;
    public CultPuzzleBoard m_pPuzzleBoard;

    public void OnPointerDown( PointerEventData eventData )
    {
        foreach ( var m_pPuzzleButton in m_pPuzzleBoard.m_pPuzzleButtonValues )
        {
            if ( m_bChannel2 )
            {
                if ( ( m_pPuzzleButton.Value & 1<<0 ) != 0 )
                    m_pPuzzleButton.Key.gameObject.GetComponent<Image>().color = new Color( 9.0f/255, 0.0f/255, 255.0f/255 );
            }
            else
            {
                if ( ( m_pPuzzleButton.Value & 1<<1 ) != 0 )
                    m_pPuzzleButton.Key.gameObject.GetComponent<Image>().color = new Color( 255.0f/255, 109.0f/255, 0.0f/255 );
            }

        }
    }

    public void OnPointerUp( PointerEventData eventData )
    {
        foreach ( var m_pPuzzleButton in m_pPuzzleBoard.m_pPuzzleButtonValues )
        {
            if ( ( m_pPuzzleButton.Value & 1<<2 ) == 0 )
                m_pPuzzleButton.Key.gameObject.GetComponent<Image>().color = Color.white;
            else
                m_pPuzzleButton.Key.gameObject.GetComponent<Image>().color = Color.gray;
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
