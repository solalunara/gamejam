using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Statics;

public class StartScreenButtons : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Begin()
    {
        g_fReactorState = 0.0f;
        g_bMadeItToBunker = false;
        g_mapPlayerControllerMap.Clear();
        g_mapPuzzleUIElems.Clear();
        g_mapPuzzleRooms.Clear();
        g_mapWorkstations.Clear();
        SceneManager.LoadScene( "LunaScene" );
    }

    public void Quit()
    {
        Application.Quit();
    }
}
