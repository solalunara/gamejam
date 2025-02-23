using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Statics;

public class CameraReference : MonoBehaviour
{
    public bool m_bMoveCameraNearBottom = false;
    void OnTriggerEnter( Collider c )
    {
        PlayerBodyController p = c.gameObject.GetComponentInParent<PlayerBodyController>();
        if ( p )
            g_mapPlayerControllerMap[ p ].CameraReference = this.gameObject;
    }
}
