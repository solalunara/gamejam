using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FaultListBody : MonoBehaviour
{
    public float m_fCharacteristicTime = 60.0f;
    public float m_fInitialTime = 60.0f;
    List<Puzzle> m_pFaults = new();
    TMP_Text m_pText;
    bool m_bRunningCoroutine = false;
    void OnEnable()
    {
        m_pText = GetComponent<TMP_Text>();
    }
    void Update()
    {
        if ( !m_bRunningCoroutine )
        {
            m_bRunningCoroutine = true;
            StopAllCoroutines();
            StartCoroutine(nameof(FaultTrigger));
        }
    }


    IEnumerator FaultTrigger()
    {
        while ( true )
        {
            float fTimeToNextFault = m_fInitialTime * Mathf.Exp( -Time.time / m_fCharacteristicTime );
            int iNumPuzzles = Enum.GetValues(typeof(Puzzle)).Length - 1;
            int iPuzzleNum = UnityEngine.Random.Range( 0, iNumPuzzles );
            Puzzle p = (Puzzle)(1<<iPuzzleNum);
            /*
            if ( m_pFaults.Contains( p ) )
            {
                yield return new WaitForSeconds( fTimeToNextFault );
                continue;
            }
            */
            PuzzleUI pUI = PuzzleUI.g_pPuzzleUIElems[ p ];
            pUI.InitPuzzle();
            m_pFaults.Add( p );
            m_pText.text += "- " + PuzzleUI.g_pPuzzleNames[ p ] + "\n";
            yield return new WaitForSeconds( fTimeToNextFault );
        }
    }

}
