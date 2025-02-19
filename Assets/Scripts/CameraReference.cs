using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraReference : MonoBehaviour
{
    public static readonly Dictionary<PlayerBodyController, CameraController> Controller = new();
    void OnTriggerEnter( Collider c )
    {
        PlayerBodyController p = c.gameObject.GetComponentInParent<PlayerBodyController>();
        if ( p )
        {
            if ( !Controller.ContainsKey( p ) )
            {
                CameraController[] pControllers = FindObjectsOfType<CameraController>();
                foreach ( var pController in pControllers )
                {
                    if ( pController.m_pPlayer == p.gameObject )
                    {
                        if ( !Controller.ContainsKey( p ) )
                            Controller.Add( p, pController );
                        else
                            throw new InvalidProgramException( "Too many camera controllers for player " + p );
                    }
                }
                if ( !Controller.ContainsKey( p ) )
                    throw new InvalidProgramException( "No camera controller for player " + p );
            }
            Controller[ p ].CameraReference = this.gameObject;
        }
    }
}
