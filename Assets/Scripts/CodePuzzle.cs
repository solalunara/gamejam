using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
public class CodePuzzle : PuzzleBoard
{
    public Dictionary<Button, int> m_pPuzzleButtonValues = new();
    int numMax = 7;
    int pButtonsClicked;
    string iValues;
    string noteText;
    string Code;
    public void ClickCodePuzzleBoardButton( GameObject pButton )
    {   
        iValues+=$"{pButton.GetComponentInChildren<TMP_Text>().text}";
        transform.parent.GetComponentInChildren<CodeDisplayOutput>( true ).UpdateText($"{iValues}");
        pButtonsClicked++;
        if(iValues == Code){
            ResolveBoard( true );
        }
        else if(pButtonsClicked == numMax){
            ResolveBoard( false );
        }
    }
    public override void InitBoard()
    {
        pButtonsClicked = 0;
        iValues = "";
        noteText = "Code:\n";
        Code = "";
        transform.parent.GetComponentInChildren<CodeDisplayOutput>( true ).UpdateText("");
        for( int i = 0; i < numMax; i++ )
            Code+=$"{Random.Range(1,9)}";
        noteText+=$"{Code}";
        GetComponentInChildren<CodeNote>( true ).UpdateText2($"{noteText}");
        }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ResolveBoard( bool bSuccess )
    {
        // clear the board to prevent trial and error
        m_pPuzzleButtonValues.Clear();
        InitBoard();
        // Resolve if success
        if ( bSuccess )
            Resolve();
    }
}
