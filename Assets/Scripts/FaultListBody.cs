using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Statics;

public class FaultListBody : MonoBehaviour
{
    TMP_Text m_pText;
    void OnEnable()
    {
        m_pText = GetComponent<TMP_Text>();
    }

    void FixedUpdate()
    {
        m_pText.text = "";
        g_pFaultList.ForEach( fault => m_pText.text += "- " + g_mapPuzzleNames[ fault.Item1 ] + " (" + (int)fault.Item2 + ")" + "\n" );
    }
}
