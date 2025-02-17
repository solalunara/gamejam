using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Unity.VersionControl.Git;
using Unity.VisualScripting;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class PlayerBodyController : MonoBehaviour
{
    GameObject m_pCamera;
    GameObject m_pGround
    {
        get => CheckGround();
    }
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        m_pCamera = GetComponentInChildren<Camera>().gameObject;
    }

    public float m_fMouseSpeed = 1000.0f;
    public float m_fMaxSpeed = 5.0f;

    Dictionary<GameObject, Vector3> Collisions = new();
    void OnCollisionEnter( Collision c )
    {
        HashSet<Vector3> pContactNormals = new();
        foreach ( ContactPoint contact in c.contacts )
            pContactNormals.Add( contact.normal );
        
        if ( pContactNormals.Count > 1 )
            throw new NotImplementedException( "not implemented yet" );

        Collisions.Add( c.gameObject, pContactNormals.First() );
    }

    void OnCollisionStay( Collision c )
    {
        HashSet<Vector3> pContactNormals = new();
        foreach ( ContactPoint contact in c.contacts )
            pContactNormals.Add( contact.normal );
        
        if ( pContactNormals.Count > 1 )
            throw new NotImplementedException( "not implemented yet" );

        Collisions[ c.gameObject ] = pContactNormals.First();
    }
    void OnCollisionExit( Collision c )
    {
        HashSet<Vector3> pContactNormals = new();
        foreach ( ContactPoint contact in c.contacts )
            pContactNormals.Add( contact.normal );

        if ( pContactNormals.Count > 1 )
            throw new NotImplementedException( "not implemented yet" );

        Collisions.Remove( c.gameObject );
    }

    GameObject CheckGround()
    {
        foreach ( var Collision in Collisions )
        {
            Vector3 vCollisionNormal = Collision.Value;
            if ( vCollisionNormal.y > 0.7 )
                return Collision.Key;
        }
        return null;
    }

    // Update is called once per frame
    void Update()
    {
        float y = -Input.GetAxis( "Mouse Y" );
        float x = Input.GetAxis( "Mouse X" );
        m_pCamera.transform.Rotate( new Vector3( 1, 0, 0 ), y * m_fMouseSpeed * Time.deltaTime, Space.Self );
        transform.Rotate( new Vector3( 0, 1, 0 ), x * m_fMouseSpeed * Time.deltaTime, Space.World );
    }

    void FixedUpdate()
    {
        var pRigidBody = GetComponent<Rigidbody>();

        //try to walk
        Vector3 WalkForce = Vector3.zero;
        if ( Input.GetKey( KeyCode.W ) )
            WalkForce += transform.forward;
        if ( Input.GetKey( KeyCode.S ) )
            WalkForce -= transform.forward;
        if ( Input.GetKey( KeyCode.D ) )
            WalkForce += transform.right;
        if ( Input.GetKey( KeyCode.A ) )
            WalkForce -= transform.right;
        WalkForce = WalkForce.normalized;

        if ( Vector3.Dot( pRigidBody.velocity, WalkForce.normalized ) < m_fMaxSpeed )
            pRigidBody.AddForce( WalkForce * 100 * pRigidBody.mass );

        if ( Input.GetKey( KeyCode.Space ) && m_pGround != null )
        {
            print( "trying to fly" );
            pRigidBody.velocity += new Vector3( 0, 1, 0 );
        }



        //transform.rotation = Quaternion.Slerp( transform.rotation, BodyGoal.transform.rotation, .2f );
        //Head.transform.localRotation = Quaternion.Slerp( Head.transform.localRotation, LookGoal.transform.rotation, .2f );
    }
}
