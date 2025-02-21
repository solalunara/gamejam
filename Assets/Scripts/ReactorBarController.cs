using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Statics;

public class ReactorBarController : MonoBehaviour
{
    public float m_fReactorMaxValue = 100.0f;
    public float m_fChangeBehaviourPoint = 0.5f; // the point [0-1) at which the ScaledReactorValue switches to a nonlinear expression
    public float m_fReactorNonlinearScalingValue = 0.2f; // the exponent of nonlinearity such that the output slider value [0-1] is proportional to the input value to this power
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
        m_pSlider.maxValue = 1.0f;
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
        float m_fTempPerSecondPerFault = g_pFaultList.m_fTempDecreasePerComplete / g_pFaultList.m_fPuzzleTime;
        g_fReactorState += g_pFaultList.FaultCount * Time.deltaTime * m_fTempPerSecondPerFault;
        m_pSlider.value = Mathf.Lerp( m_pSlider.value, ScaledReactorValue( g_fReactorState ), Time.deltaTime );

        if ( m_pSlider.value >= 1.0f )
            SceneManager.LoadScene( "GameOverScene" );

        if ( m_pSlider.value >= m_fStartBlinking )
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

    float ScaledReactorValue( float fReactorValue )
    {
        fReactorValue /= m_fReactorMaxValue;
        if ( fReactorValue < m_fChangeBehaviourPoint )
            return fReactorValue;
        else
        {
            float __c = m_fChangeBehaviourPoint;
            float __v = m_fReactorNonlinearScalingValue;
            float __a = __c - Mathf.Pow( 1/__v, 1/(__v-1) );
            float __b = __c - Mathf.Pow( __c - __a, __v );
            return Mathf.Pow( fReactorValue - __a, __v ) + __b;
        }
    }

    IEnumerator Blink()
    {
        while ( true )
        {
            m_pPointer.enabled = !m_pPointer.enabled;
            float fWaitTime = 0.5f * ( 1.0f - 0.9f * ( m_pSlider.value - m_fStartBlinking ) / ( 1.0f - m_fStartBlinking ) );
            yield return new WaitForSeconds( fWaitTime );
        }
    }
}
