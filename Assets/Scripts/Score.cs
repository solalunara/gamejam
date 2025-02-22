using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Statics;

public class Score : MonoBehaviour
{
    float m_fScore;
    // Start is called before the first frame update
    void OnEnable()
    {
        m_fScore = g_pFaultList.CompletedTasks * 10.0f;
        if ( g_bMadeItToBunker )
            m_fScore *= 2.00f;
        TMP_Text t = GetComponent<TMP_Text>();
        t.text = m_fScore.ToString();
        if ( !g_bMadeItToBunker )
        t.text += "\n You Died! (-50%)";
    }
}
