using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CultPuzzleBoard : MonoBehaviour, IPuzzleBoard
{
    public Dictionary<Button, int> m_pPuzzleButtonValues = new();
    const int NUM_WHITE = 3;
    int m_iNumClicked = 0;

    public void ClickCultPuzzleBoardButton( GameObject pButton )
    {
        int iValue = m_pPuzzleButtonValues[ pButton.GetComponent<Button>() ];
        if ( iValue == 3 && ( iValue & 1<<2 ) == 0 )
        {
            pButton.GetComponent<Image>().color = Color.gray;
            m_iNumClicked++;
            m_pPuzzleButtonValues[ pButton.GetComponent<Button>() ] |= 1<<2;
            if ( m_iNumClicked == NUM_WHITE )
                ResolveBoard( true );
        }
        else
            ResolveBoard( false );
    }


    public void InitBoard()
    {
        if ( m_pPuzzleButtonValues.Any() )
            return; //nothing to init

        Button[] pPuzzleButtons = GetComponentsInChildren<Button>();

        List<int> pSelection = new( 9 );
        for ( int i = 0; i < NUM_WHITE; ++i )
            pSelection.Add( 3 );
        
        int n = 0;
        while ( pSelection.Count < 9 )
        {
            pSelection.Add( n );
            n++;
            n %= 3;
        }

        for ( int i = 0; i < 9; ++i )
        {
            int index = Random.Range( 0, pSelection.Count );
            int iValue = pSelection[ index ];
            pSelection.RemoveAt( index );

            m_pPuzzleButtonValues.Add( pPuzzleButtons[ i ], iValue );
        }
    }

    void ResolveBoard( bool bSuccess )
    {
        m_iNumClicked = 0;
        foreach ( var m_pPuzzleButtonValue in m_pPuzzleButtonValues )
            m_pPuzzleButtonValue.Key.gameObject.GetComponent<Image>().color = Color.white;
        for ( int i = 0; i < m_pPuzzleButtonValues.Count; ++i )
            m_pPuzzleButtonValues[ m_pPuzzleButtonValues.ElementAt( i ).Key ] &= ~(1<<2);

        // clear the board on failure to prevent trial and error
        m_pPuzzleButtonValues.Clear();
        InitBoard();

        if ( bSuccess )
            GetComponentInParent<PuzzleUI>().Resolve();
    }
}
