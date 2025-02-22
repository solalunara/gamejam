using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardSwipePuzzleBoard : PuzzleBoard
{
    enum FailType
    {
        TOO_FAST,
        TOO_SLOW
    }
    public float m_fSwipeTime = 1.0f;
    public float m_fLeniency = 0.1f;
    public float m_fRecallSpeed = 1.0f;

    Slider m_pSlider;
    bool m_bInSwipe = false;
    bool m_bInRecall = false;
    float m_fStartTime = 0.0f;

    void OnEnable()
    {
        InitBoard();
    }

    public override void InitBoard()
    {
        m_pSlider = GetComponentInChildren<Slider>( true );
        transform.parent.GetComponentInChildren<CardPuzzleFeedback>().UpdateText( "" );
    }

    // Update is called once per frame
    void Update()
    {
        if ( m_bInRecall )
        {
            m_pSlider.value -= m_fRecallSpeed * Time.deltaTime;
            if ( m_pSlider.value <= 0.0f )
            {
                m_pSlider.value = 0.0f;
                m_pSlider.interactable = true;
                m_bInRecall = false;
            }
        }
    }

    void CardTestFailed( FailType iFailType )
    {
        switch ( iFailType )
        {
            case FailType.TOO_FAST:
                transform.parent.GetComponentInChildren<CardPuzzleFeedback>().UpdateText( "Too Fast!" );
                break;
            case FailType.TOO_SLOW:
                transform.parent.GetComponentInChildren<CardPuzzleFeedback>().UpdateText( "Too Slow!" );
                break;
        }
    }

    public void NewSliderValue()
    {
        if ( m_pSlider.value != 0.0f && !m_bInRecall && !m_bInSwipe )
        {
            m_fStartTime = Time.time;
            m_bInSwipe = true;
            return;
        }
        if ( m_pSlider.value > 0.98f && m_bInSwipe )
        {
            m_bInSwipe = false;
            float fDeltaTime = Time.time - m_fStartTime - m_fSwipeTime;
            if ( Mathf.Abs( fDeltaTime ) < m_fLeniency )
                Resolve();
            else
            {
                CardTestFailed( fDeltaTime > 0 ? FailType.TOO_SLOW : FailType.TOO_FAST );
                m_pSlider.interactable = false;
                m_bInRecall = true;
            }
        }
    }
}
