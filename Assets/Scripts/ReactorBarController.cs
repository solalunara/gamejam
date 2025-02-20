using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReactorBarController : MonoBehaviour
{
    public float m_fStartBlinking = 75.0f;
    public float m_fReactorState = 50.0f;
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
        m_pSlider.value = m_fReactorState;

        if ( m_fReactorState > 100.0f )
            m_fReactorState = 100.0f;
        else if ( m_fReactorState < 0.0f )
            m_fReactorState = 0.0f;

        if ( m_fReactorState >= m_fStartBlinking )
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
            float fWaitTime = 0.5f * ( 1.0f - 0.9f * ( m_fReactorState - m_fStartBlinking  ) / ( 100.0f - m_fStartBlinking ) );
            yield return new WaitForSeconds( fWaitTime );
        }
    }
}
