using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Statics;

public class CameraReference : MonoBehaviour
{
    void OnTriggerEnter( Collider c )
    {
        PlayerBodyController p = c.gameObject.GetComponentInParent<PlayerBodyController>();
        if ( p )
        {
            if ( !g_mapPlayerControllerMap.ContainsKey( p ) )
            {
                CameraController[] pControllers = FindObjectsOfType<CameraController>();
                foreach ( var pController in pControllers )
                {
                    if ( pController.m_pPlayer == p.gameObject )
                    {
                        if ( !g_mapPlayerControllerMap.ContainsKey( p ) )
                            g_mapPlayerControllerMap.Add( p, pController );
                        else
                            throw new InvalidProgramException( "Too many camera controllers for player " + p );
                    }
                }
                if ( !g_mapPlayerControllerMap.ContainsKey( p ) )
                    throw new InvalidProgramException( "No camera controller for player " + p );
            }
            g_mapPlayerControllerMap[ p ].CameraReference = this.gameObject;
        }
    }
}
