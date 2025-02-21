using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Statics;

public class FaultListBody : MonoBehaviour
{
    public TMP_Text Text => m_pText;
    TMP_Text m_pText;
    void OnEnable()
    {
        m_pText = GetComponent<TMP_Text>();
    }
}
