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
    GameObject m_pGround => CheckGround();
    Vector3 m_vGroundNormal;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        m_pCamera = GetComponentInChildren<Camera>().gameObject;
    }

    public float m_fMouseSpeed = 1000.0f;
    public float m_fMaxSpeed = 5.0f;
    public int m_iGroundThreshold = 1;
    public float m_fFrictionConstant = 1.5f;
    public int m_iCoyoteFrames = 4;
    int m_iGroundFrames = 0;
    int m_iFramesSinceGround = 0;

    GameObject CheckGround()
    {
        foreach ( var Collision in Collisions )
        {
            Vector3[] vCollisionNormals = Collision.Value;
            foreach ( Vector3 vCollisionNormal in vCollisionNormals )
            {
                if ( vCollisionNormal.y > 0.7 )
                {
                    m_vGroundNormal = vCollisionNormal;
                    return Collision.Key;
                }
            }
        }
        return null;
    }

    // Update is called once per frame
    void Update()
    {
        // edit cursor lock state for editor
        if ( Application.isEditor )
        {
            bool mouseOverWindow = Input.mousePosition.x > 0 && Input.mousePosition.x < Screen.width && Input.mousePosition.y > 0 && Input.mousePosition.y < Screen.height;

            //check if the cursor is locked but not centred in the game viewport - i.e. like it is every time the game starts
            if ( Cursor.lockState == CursorLockMode.Locked && !mouseOverWindow )
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
            //if the player presses Escape, unlock the cursor
            if (Input.GetKeyDown(KeyCode.Escape) && Cursor.lockState == CursorLockMode.Locked )
            {
                Cursor.lockState = CursorLockMode.None;
            }
            //if the player releases any mouse button while the cursor is over the game viewport, then lock the cursor again
            else if ( Cursor.lockState == CursorLockMode.None )
            {
                if ( mouseOverWindow && ( Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1) || Input.GetMouseButtonUp(2) ) )
                {
                    Cursor.lockState = CursorLockMode.Locked;
                }
            }
        }

        if ( Cursor.lockState == CursorLockMode.Locked )
        {
            float y = -Input.GetAxis( "Mouse Y" );
            float x = Input.GetAxis( "Mouse X" );
            m_pCamera.transform.Rotate( new Vector3( 1, 0, 0 ), y * m_fMouseSpeed * Time.deltaTime, Space.Self );
            transform.Rotate( new Vector3( 0, 1, 0 ), x * m_fMouseSpeed * Time.deltaTime, Space.World );
        }
    }

    Dictionary<GameObject, Vector3[]> Collisions = new();
    void OnCollisionEnter( Collision c )
    {
        HashSet<Vector3> pContactNormals = new();
        List<ContactPoint> contacts = new();
        c.GetContacts( contacts );

        foreach ( ContactPoint contact in contacts )
            pContactNormals.Add( contact.normal );
        
        Collisions.Add( c.gameObject, pContactNormals.ToArray() );
    }

    void OnCollisionStay( Collision c )
    {
        HashSet<Vector3> pContactNormals = new();
        List<ContactPoint> contacts = new();
        c.GetContacts( contacts );

        foreach ( ContactPoint contact in contacts )
            pContactNormals.Add( contact.normal );

        Collisions[ c.gameObject ] = pContactNormals.ToArray();
    }
    void OnCollisionExit( Collision c )
    {
        Collisions.Remove( c.gameObject );
    }

    void Friction()
    {
        var pRigidBody = GetComponent<Rigidbody>();
        if ( m_pGround )
        {
            Vector3 vFriction = 9.81f * m_fFrictionConstant * Time.fixedDeltaTime * Vector3.Dot( m_vGroundNormal, Vector3.up ) / pRigidBody.velocity.magnitude * -pRigidBody.velocity;
            if ( vFriction.sqrMagnitude > pRigidBody.velocity.sqrMagnitude )
                pRigidBody.velocity = Vector3.zero;
            else
                pRigidBody.velocity += vFriction;
        }
    }

    void FixedUpdate()
    {
        var pRigidBody = GetComponent<Rigidbody>();

        m_iGroundFrames = m_pGround != null ? m_iGroundFrames + 1 : 0;
        m_iFramesSinceGround = m_iGroundFrames >= m_iGroundThreshold ? 0 : m_iFramesSinceGround + 1;

        pRigidBody.velocity += -9.81f * Time.fixedDeltaTime * Vector3.up;
        Friction();

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

        if ( m_iGroundFrames == 0 )
            WalkForce *= 0.1f;

        if ( pRigidBody.velocity.sqrMagnitude < m_fMaxSpeed * m_fMaxSpeed )
        {
            // clamp walk force to only add up to max speed
            if ( ( pRigidBody.velocity + WalkForce ).sqrMagnitude > m_fMaxSpeed * m_fMaxSpeed )
            {
                float __A = WalkForce.sqrMagnitude;
                float __B = 2 * Vector3.Dot( pRigidBody.velocity, WalkForce );
                float __C = pRigidBody.velocity.sqrMagnitude - m_fMaxSpeed * m_fMaxSpeed;
                float fScaleFactor = -__B + Mathf.Sqrt( __B * __B - 4 * __A * __C );
                fScaleFactor /= 2 * __A;
                if ( fScaleFactor < 0 || fScaleFactor > 1 )
                    throw new ArithmeticException( "Invalid scale factor: " + fScaleFactor );
                WalkForce *= fScaleFactor;
            }

            pRigidBody.AddForce( WalkForce, ForceMode.VelocityChange );
        }

        if ( Input.GetKey( KeyCode.Space ) && m_iFramesSinceGround < m_iCoyoteFrames )
        {
            pRigidBody.AddForce( new Vector3( 0, 2, 0 ), ForceMode.VelocityChange );
            m_iFramesSinceGround += 10000;
        }



        //transform.rotation = Quaternion.Slerp( transform.rotation, BodyGoal.transform.rotation, .2f );
        //Head.transform.localRotation = Quaternion.Slerp( Head.transform.localRotation, LookGoal.transform.rotation, .2f );
    }
}
