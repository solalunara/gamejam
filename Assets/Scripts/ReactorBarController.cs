using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ReactorBarController : MonoBehaviour
{
    public int m_iMaxStable = 30;
    public float m_fTempPerSecondPerFault = 1.0f;
    public float m_fStartBlinking = 75.0f;
    public static float ReactorState = 0.0f;
    Slider m_pSlider;
    RawImage m_pPointer;
    bool m_bBlinking = false;
    // Start is called before the first frame update
    void Start()
    {
        m_pSlider = GetComponent<Slider>();
        m_pPointer = GetComponentInChildren<SliderPointer>().gameObject.GetComponent<RawImage>();
    }

    // Update is called once per frame
    void Update()
    {
        // method 1:
        //m_fReactorState = Mathf.Lerp( m_fReactorState, PuzzleUI.FaultList.m_pFaults.Count * 100.0f / m_iMaxStable, Time.deltaTime );
        // method 2:
        ReactorState += Mathf.Pow( PuzzleUI.FaultList.m_pFaults.Count, 1.1f ) * Time.deltaTime * m_fTempPerSecondPerFault;
        m_pSlider.value = Mathf.Lerp( m_pSlider.value, ReactorState, Time.deltaTime );

        if ( ReactorState > 100.0f )
            SceneManager.LoadScene( "GameOverScene" );
        else if ( ReactorState < 0.0f )
            ReactorState = 0.0f;

        if ( ReactorState >= m_fStartBlinking )
        {
            if ( !m_bBlinking )
            {
                m_bBlinking = true;
                m_pPointer.color = Color.red;
                StopAllCoroutines();
                StartCoroutine(nameof(Blink));
            }
        }
        else
        {
            if ( m_bBlinking )
            {
                m_bBlinking = false;
                m_pPointer.color = Color.white;
                m_pPointer.enabled = true;
                StopAllCoroutines();
            }
        }
    }

    IEnumerator Blink()
    {
        while ( true )
        {
            m_pPointer.enabled = !m_pPointer.enabled;
            float fWaitTime = 0.5f * ( 1.0f - 0.9f * ( ReactorState - m_fStartBlinking  ) / ( 100.0f - m_fStartBlinking ) );
            yield return new WaitForSeconds( fWaitTime );
        }
    }
}
