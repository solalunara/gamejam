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
