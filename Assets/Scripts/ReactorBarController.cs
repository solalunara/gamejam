using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Statics;

public class ReactorBarController : MonoBehaviour
{
    // max slider value - for max reactor value do MAX_VALUE^2
    const float MAX_VALUE = 10.0f;
    public int m_iMaxStable = 30;
    public float m_fTempPerSecondPerFault = 1.0f;
    public float m_fStartBlinking = 0.75f;
    Slider m_pSlider;
    RawImage m_pPointer;
    bool m_bBlinking = false;
    // Start is called before the first frame update
    void Start()
    {
        m_pSlider = GetComponent<Slider>();
        m_pSlider.maxValue = MAX_VALUE;
        m_pPointer = GetComponentInChildren<SliderPointer>().gameObject.GetComponent<RawImage>();
    }

    // Update is called once per frame
    void Update()
    {
        // method 1:
        //m_fReactorState = Mathf.Lerp( m_fReactorState, PuzzleUI.FaultList.m_pFaults.Count * 100.0f / m_iMaxStable, Time.deltaTime );
        // method 2:
        if ( g_fReactorState < 0 )
            g_fReactorState = 0;
        g_fReactorState += Mathf.Pow( g_pFaultList.FaultCount, 1.1f ) * Time.deltaTime * m_fTempPerSecondPerFault;
        m_pSlider.value = Mathf.Lerp( m_pSlider.value, Mathf.Sqrt( g_fReactorState ), Time.deltaTime );

        if ( m_pSlider.value >= MAX_VALUE )
            SceneManager.LoadScene( "GameOverScene" );

        if ( m_pSlider.value >= m_fStartBlinking * MAX_VALUE )
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
            float fWaitTime = 0.5f * ( 1.0f - 0.9f * ( m_pSlider.value - m_fStartBlinking * MAX_VALUE  ) / ( MAX_VALUE - m_fStartBlinking * MAX_VALUE ) );
            yield return new WaitForSeconds( fWaitTime );
        }
    }
}
