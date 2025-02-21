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
        m_fScore = g_pFaultList.CompletedTasks;
        GetComponent<TMP_Text>().text = m_fScore.ToString();
    }
}
