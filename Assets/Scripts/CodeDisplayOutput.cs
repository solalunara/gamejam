using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CodeDisplayOutput : MonoBehaviour
{
    // Start is called before the first frame update
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
