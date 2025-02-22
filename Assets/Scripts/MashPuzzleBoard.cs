using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MashPuzzleBoard : PuzzleBoard
{
    public float m_fPassiveDecrease = 4.0f;
    public float m_fNumClicks = 16.0f;
    float m_fValue = 0.0f;
    Slider m_pSlider;

    public override void InitBoard()
    {
        m_pSlider = GetComponentInChildren<Slider>( true );
        m_fValue = 0.0f;
        m_pSlider.maxValue = Mathf.Sqrt( m_fNumClicks );
        m_pSlider.minValue = 0.0f;
        m_pSlider.value = m_pSlider.minValue;
    }

    public void ButtonClicked()
    {
        m_fValue += 1.0f;
        if ( m_fValue >= m_fNumClicks )
            Resolve();
    }

    // Update is called once per frame
    void Update()
    {
        if ( m_fValue > 0.0f )
        {
            m_fValue -= m_fPassiveDecrease * Time.deltaTime;
            if ( m_fValue < 0.0f )
                m_fValue = 0.0f;
        }
        m_pSlider.value = Mathf.Sqrt( m_fValue );
    }
}
