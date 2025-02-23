using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardPuzzleFeedback : MonoBehaviour
{
    TMP_Text m_pText;
    void OnEnable()
    {
        m_pText = GetComponent<TMP_Text>();
    }

    public void UpdateText( string s )
    {
        if ( !m_pText )
            m_pText = GetComponent<TMP_Text>();
        m_pText.text = s;
    }
}
