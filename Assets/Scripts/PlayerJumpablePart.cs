using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class PlayerJumpablePart : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter( Collision c )
    {
        // providesContacts must be turned on for Physics.ContactEvent to be called
        c.collider.providesContacts = true;
    }
    void OnCollisionStay( Collision c )
    {
        // providesContacts must be turned on for Physics.ContactEvent to be called
        c.collider.providesContacts = true;
    }
    void OnCollisionExit( Collision c )
    {
        // this won't be called sometimes if we only leave by crouching/uncrouching
        // so that code will handle this itself
        c.collider.providesContacts = false;
    }
}
