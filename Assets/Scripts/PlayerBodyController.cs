using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;
using Cursor = UnityEngine.Cursor;
using static Statics;

[RequireComponent(typeof(Rigidbody))]
public class PlayerBodyController : MonoBehaviour
{
    Vector3 m_vPlayerUncrouchedEyeoffset;
    Vector3 m_vPlayerCrouchedEyeoffset;
    Vector3 m_vPlayerUncrouchedEyeoffsetFromFloor;
    Vector3 m_vPlayerGroundUnrouchDelta;
    Vector3 m_vPlayerGroundCrouchDelta;
    const float EPSILON = 0.1f;


    public GameObject GroundEntity => CheckGround();
    public readonly Dictionary<GameObject, Vector3[]> CollisionNormalsSet = new();

    public Workstation ActiveWorkstation 
    {
        get => m_pActiveWorkstation;
        set => m_pActiveWorkstation = value;
    }

    public int ActivePuzzles => m_iActivePuzzles;

    public GameObject m_pInteractionPrompt;

    public float m_fMouseSpeed = 1000.0f;
    public float m_fLookSpeed = 10.0f;
    public float m_fAirMoveFraction = 0.1f;
    public float m_fMaxSpeed = 3.0f;
    public float m_fMaxSprintSpeed = 6.0f;
    public int m_iGroundThreshold = 1;
    public float m_fFrictionConstant = 1.5f;
    public int m_iCoyoteFrames = 2;
    public Vector3 m_vGravity = -9.81f * Vector3.up;
    public Vector3 m_vJumpVector = 4.0f * Vector3.up;
    //public bool m_bEnableABH = true;
    public float m_fMaxStamina = 1.0f;
    public float m_fMinStamina = 0.05f;
    public float m_fStamina = 1.0f;
    public float m_fStaminaTime = 3.0f;
    public float m_fStaminaRecoveryTime = 5.0f;
    public bool m_bLocalMovement = true;
    Camera m_pCamera;

    bool m_bCrouched = false;
    int m_iGroundFrames = 0;
    int m_iFramesSinceGround = 0;
    int m_iCrouchedFrames = 0;
    bool m_bWantsToCrouch = false;
    int m_iJumpTimer = 0;
    bool m_bSprinting = false;
    Vector3 m_vGroundNormal;
    GameObject m_pUncrouchedObj;
    GameObject m_pCrouchedObj;
    Workstation m_pActiveWorkstation;
    int m_iActivePuzzles = 0;

    void OnEnable()
    {
        m_pUncrouchedObj = GetComponentInChildren<PlayerUncrouchedPart>().gameObject;
        m_pCrouchedObj = GetComponentInChildren<PlayerCrouchedPart>( true ).gameObject;
        m_pCamera = GetComponentInChildren<Camera>();
        Physics.ContactEvent += Physics_ContactEvent;

        CameraController[] pControllers = FindObjectsOfType<CameraController>();
        foreach ( var pController in pControllers )
        {
            if ( pController.m_pPlayer == gameObject )
            {
                if ( !g_mapPlayerControllerMap.ContainsKey( this ) )
                    g_mapPlayerControllerMap.Add( this, pController );
                else
                    throw new InvalidProgramException( "Too many camera controllers for player " + this );
            }
        }
        if ( !g_mapPlayerControllerMap.ContainsKey( this ) )
            throw new InvalidProgramException( "No camera controller for player " + this );

        m_pCrouchedObj.SetActive( true );
        Bounds bbxWorldSpaceCrouched = m_pCrouchedObj.GetComponent<Collider>().bounds;
        m_pCrouchedObj.SetActive( false );
        Bounds bbxWorldSpaceUncrouched = m_pUncrouchedObj.GetComponent<Collider>().bounds;
        m_vPlayerCrouchedEyeoffset = -m_pCrouchedObj.transform.localPosition;
        m_vPlayerUncrouchedEyeoffset = -m_pUncrouchedObj.transform.localPosition;
        Vector3 vContactPtCrouched = new( bbxWorldSpaceCrouched.center.x, bbxWorldSpaceCrouched.min.y, bbxWorldSpaceCrouched.center.z );
        Vector3 vContactPtUncrouched = new( bbxWorldSpaceUncrouched.center.x, bbxWorldSpaceUncrouched.min.y, bbxWorldSpaceUncrouched.center.z );
        Vector3 vUpperPtCrouched = new( bbxWorldSpaceCrouched.center.x, bbxWorldSpaceCrouched.max.y, bbxWorldSpaceCrouched.center.z );
        Vector3 vUpperPtUncrouched = new( bbxWorldSpaceUncrouched.center.x, bbxWorldSpaceUncrouched.max.y, bbxWorldSpaceUncrouched.center.z );
        m_vPlayerUncrouchedEyeoffsetFromFloor = m_pUncrouchedObj.transform.position - vContactPtUncrouched + m_vPlayerUncrouchedEyeoffset;
        m_vPlayerGroundCrouchDelta = vContactPtUncrouched - vContactPtCrouched; //difference from contact pt crouched to contact pt uncrouched (for crouching on ground)
        m_vPlayerGroundUnrouchDelta = -m_vPlayerGroundCrouchDelta + ( vUpperPtUncrouched - vUpperPtCrouched ); //not exactly -gndcrouch b/c top extents not neccesarily alligned
    }

    private void Physics_ContactEvent( PhysicsScene scene, NativeArray<ContactPairHeader>.ReadOnly pHeaderArray )
    {
        CollisionNormalsSet.Clear();
        foreach ( var Header in pHeaderArray )
        {
            Component rFirst = Header.Body;
            Component rSecond = Header.OtherBody;

            PlayerBodyController pBody = null;
            if ( rFirst  &&  rFirst.gameObject.GetComponent<PlayerBodyController>() )
                pBody =  rFirst.gameObject.GetComponent<PlayerBodyController>();
            if ( rSecond && rSecond.gameObject.GetComponent<PlayerBodyController>() )
                pBody = rSecond.gameObject.GetComponent<PlayerBodyController>();

            if ( !pBody )
                continue;

            for ( int i = Header.PairCount; --i >= 0; )
            {
                ref readonly var Pair = ref Header.GetContactPair( i );

                if ( !Pair.Collider || !Pair.OtherCollider )
                    continue;
                
                PlayerJumpablePart cFirst = Pair.Collider.gameObject.GetComponent<PlayerJumpablePart>();
                PlayerJumpablePart cSecond = Pair.OtherCollider.gameObject.GetComponent<PlayerJumpablePart>();

                if ( !cFirst && !cSecond )
                    continue;

                if ( cFirst && cSecond )
                    throw new Exception( "Two objects with PlayerJumpablePart are colliding: " + cFirst.gameObject + " and " + cSecond.gameObject );

                // 'normal' way around is 1st obj jumpable, 2nd object other
                bool bReversed = !cFirst;

                HashSet<Vector3> pContactNormals = new();
                NativeArray<ContactPairPoint> contacts = new( Pair.ContactCount, Allocator.Temp );
                Pair.CopyToNativeArray( contacts );

                foreach ( ContactPairPoint contact in contacts )
                    pContactNormals.Add( bReversed ? -contact.Normal : contact.Normal );
                

                GameObject pOtherObject = bReversed ? Pair.Collider.gameObject : Pair.OtherCollider.gameObject;

                if ( !CollisionNormalsSet.ContainsKey( pOtherObject ) )
                {
                    if ( pContactNormals.Any() )
                        CollisionNormalsSet.Add( pOtherObject, pContactNormals.ToArray() );
                }
                else
                {
                    if ( !pContactNormals.Any() )
                        CollisionNormalsSet.Remove( pOtherObject );
                    else
                    {
                        pContactNormals.UnionWith( CollisionNormalsSet[ pOtherObject ] );
                        CollisionNormalsSet[ pOtherObject ] = pContactNormals.ToArray();
                    }
                }
            }
        }
    }

    GameObject CheckGround()
    {
        foreach ( var CollisionNormals in CollisionNormalsSet )
        {
            foreach ( Vector3 vCollisionNormal in CollisionNormals.Value )
            {
                if ( vCollisionNormal.y > 0.7 )
                {
                    m_vGroundNormal = vCollisionNormal;
                    return CollisionNormals.Key;
                }
            }
        }
        return null;
    }

    // Update is called once per frame
    void Update()
    {
        // edit cursor lock state for editor
        if ( m_bLocalMovement )
        {
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
                        if ( ActivePuzzles == 0 )
                            Cursor.lockState = CursorLockMode.Locked;
                    }
                }
            }

            if ( Cursor.lockState == CursorLockMode.Locked )
            {
                float y = -Input.GetAxis( "Mouse Y" );
                float x = Input.GetAxis( "Mouse X" );
                if ( m_pCamera && m_pCamera.transform.parent )
                    m_pCamera.transform.parent.Rotate( new Vector3( 1, 0, 0 ), y * m_fMouseSpeed * Time.deltaTime, Space.Self );
                else
                {
                    m_pCamera = GetComponentInChildren<Camera>();
                    Debug.LogWarning( "no camera found or camera without parent, despite localmovement being set to true" );
                }
                transform.rotation *= Quaternion.AngleAxis( x * m_fMouseSpeed * Time.deltaTime, new Vector3( 0, 1, 0 ) );
            }
        }

        if ( Input.GetKeyDown( KeyCode.LeftControl ) )
            m_bWantsToCrouch = true;
        else if ( Input.GetKeyUp( KeyCode.LeftControl ) )
            m_bWantsToCrouch = false;

        if ( Input.GetKeyDown( KeyCode.LeftShift ) )
            m_bSprinting = true;
        else if ( Input.GetKeyUp( KeyCode.LeftShift ) )
            m_bSprinting = false;

        if ( Input.GetKeyDown( KeyCode.E ) )
        {
            if ( ActivePuzzles == 0 && m_pActiveWorkstation && !m_pActiveWorkstation.Solved )
                SetPuzzleActive( m_pActiveWorkstation.m_iType );
            else
                SetAllPuzzlesInactive();
        }
        if ( Input.GetKeyDown( KeyCode.Escape ) )
            SetAllPuzzlesInactive();


        GameObject.FindWithTag( "StaminaBar" ).GetComponent<Slider>().value = m_fStamina;

        if ( transform.position.y < -5 )
        {
            Debug.LogWarning( "Player fell through floor" );
            transform.position = new Vector3( transform.position.x, 5.0f, transform.position.y );
        }
    }

    void SetPuzzleActive( Puzzle iPuzzle )
    {
        bool bAlreadyActive = ( ActivePuzzles & (int)iPuzzle ) != 0;
        if ( bAlreadyActive == true )
            return; //nothing to do

        m_iActivePuzzles |= (int)iPuzzle;
        Cursor.lockState = CursorLockMode.None;

        m_pActiveWorkstation.SetUIElemActive( true );
        m_pInteractionPrompt.SetActive( false );
    }

    public void SetAllPuzzlesInactive()
    {
        bool bAlreadyActive = ActivePuzzles != 0;
        if ( bAlreadyActive == false )
            return; //nothing to do

        m_iActivePuzzles = 0;
        Cursor.lockState = CursorLockMode.Locked;
        Workstation[] pWorkstations = FindObjectsOfType<Workstation>();
        foreach ( var pWorkstation in pWorkstations )
            pWorkstation.SetUIElemActive( false );
    }
    public void SetPuzzleInactive( Puzzle iPuzzle )
    {
        bool bAlreadyActive = ( ActivePuzzles & (int)iPuzzle ) != 0;
        if ( bAlreadyActive == false )
            return; //nothing to do

        m_iActivePuzzles &= ~(int)iPuzzle;
        Cursor.lockState = CursorLockMode.Locked;

        m_pActiveWorkstation.SetUIElemActive( false );
    }

    void Friction()
    {
        var pRigidBody = GetComponent<Rigidbody>();
        if ( m_iGroundFrames > m_iGroundThreshold )
        {
            if ( pRigidBody.velocity.sqrMagnitude == 0.0f )
                return;
            Vector3 vFriction = Vector3.Dot( m_vGravity, -Vector3.up ) * m_fFrictionConstant * Time.fixedDeltaTime * Vector3.Dot( m_vGroundNormal, Vector3.up ) / pRigidBody.velocity.magnitude * -pRigidBody.velocity;
            if ( vFriction.sqrMagnitude > pRigidBody.velocity.sqrMagnitude )
                pRigidBody.velocity = Vector3.zero;
            else
                pRigidBody.velocity += vFriction;
        }
    }

    void SetCrouchedState( bool bCrouch )
    {
        var pRigidBody = GetComponent<Rigidbody>();
        bool bChangingState = bCrouch != m_bCrouched;
        if ( !bChangingState )
            return;

        // see PlayerJumpablePart.cs -> OnCollisionExit
        foreach ( var CollisionNormals in CollisionNormalsSet )
            CollisionNormals.Key.GetComponent<Collider>().providesContacts = false;

        GameObject pPreChangeObject = bCrouch ? m_pUncrouchedObj : m_pCrouchedObj;
        GameObject pPostChangeObject = bCrouch ? m_pCrouchedObj : m_pUncrouchedObj;
        Vector3 vPos;
        if ( !bCrouch )
        {
            //have to manually test for floor as we could be uncrouching in the air but with not enough space
            bool bHit = pRigidBody.SweepTest( -m_vPlayerUncrouchedEyeoffsetFromFloor, out RaycastHit pTrace, m_vPlayerUncrouchedEyeoffsetFromFloor.magnitude + EPSILON, QueryTriggerInteraction.Ignore );
            if ( bHit )
                vPos = pTrace.point + m_vPlayerUncrouchedEyeoffsetFromFloor;
            else
                vPos = transform.position;
        }
        else
        {
            if ( m_iGroundFrames > 0 )
                vPos = transform.position + m_vPlayerGroundCrouchDelta;
            else
                vPos = transform.position;
        }

        transform.position = vPos;

        m_pUncrouchedObj.SetActive( !bCrouch );
        m_pCrouchedObj.SetActive( bCrouch );

        CollisionNormalsSet.Clear();
        m_pInteractionPrompt.SetActive( false );

        m_bCrouched = bCrouch;
    }



    void FixedUpdate()
    {
        var pRigidBody = GetComponent<Rigidbody>();

        m_iGroundFrames = GroundEntity != null ? m_iGroundFrames + 1 : 0;
        m_iCrouchedFrames = m_iCrouchedFrames > 0 ? m_iCrouchedFrames + 1 : 0;
        m_iFramesSinceGround = m_iGroundFrames >= m_iGroundThreshold ? 0 : m_iFramesSinceGround + 1;
        m_iJumpTimer = m_iJumpTimer > 0 ? m_iJumpTimer - 1 : 0;

        //if ( m_bEnableABH && m_bCrouched && m_iGroundFrames == 1 && pRigidBody.velocity.sqrMagnitude > m_fMaxSpeed * m_fMaxSpeed )
        //    pRigidBody.AddForce( -transform.forward * 5.0f, ForceMode.VelocityChange );

        pRigidBody.velocity += Time.fixedDeltaTime * m_vGravity;
        Friction();

        //try to walk
        Vector3 WalkForce = Vector3.zero;
        if ( Input.GetKey( KeyCode.W ) )
            WalkForce += Vector3.forward;
        if ( Input.GetKey( KeyCode.S ) )
            WalkForce -= Vector3.forward;
        if ( Input.GetKey( KeyCode.D ) )
            WalkForce += Vector3.right;
        if ( Input.GetKey( KeyCode.A ) )
            WalkForce -= Vector3.right;
        WalkForce = WalkForce.normalized;

        if ( m_bLocalMovement )
            WalkForce = transform.TransformDirection( WalkForce );

        // stamina should recover if
        //  1/ the player is not sprinting, or
        //  2/ the player is sprinting but not moving, or
        //  3/ the player is in a puzzle
        if ( m_fStamina < m_fMaxStamina && ( !m_bSprinting || WalkForce == Vector3.zero || ActivePuzzles != 0 ) )
        {
            m_fStamina += Time.fixedDeltaTime / m_fStaminaRecoveryTime * ( ActivePuzzles == 0 ? 1 : 0.3f );
            if ( m_fStamina > m_fMaxStamina )
                m_fStamina = m_fMaxStamina;
        }

        if ( ActivePuzzles != 0 )
            return;

        if ( m_bWantsToCrouch != ( m_iCrouchedFrames > 0 ) ) //not using m_bCrouched to prevent multiple runs while waiting to crouch
        {
            bool bCanChangeState = true;
            if ( !m_bWantsToCrouch )
                bCanChangeState = !pRigidBody.SweepTest( m_vPlayerGroundUnrouchDelta, out RaycastHit __debug, m_vPlayerGroundUnrouchDelta.magnitude + EPSILON, QueryTriggerInteraction.Ignore );

            if ( bCanChangeState )
            {
                if ( !m_bWantsToCrouch )
                    SetCrouchedState( m_bWantsToCrouch );

                m_iCrouchedFrames = m_bWantsToCrouch ? 1 : 0;
            }
        }


        if ( m_bSprinting && m_fStamina > m_fMinStamina && WalkForce != Vector3.zero )
        {
            m_fStamina -= Time.fixedDeltaTime / m_fStaminaTime;
            if ( m_fStamina < m_fMinStamina )
            {
                m_fStamina = m_fMinStamina;
                m_bSprinting = false;
            }
        }

        if ( WalkForce != Vector3.zero && !m_bLocalMovement )
            pRigidBody.rotation = UnityEngine.Quaternion.LookRotation( WalkForce ) * UnityEngine.Quaternion.AngleAxis( 0.0f, Vector3.up );

        if ( m_iGroundFrames == 0 )
            WalkForce *= m_fAirMoveFraction;

        float fMaxSpeed = m_bSprinting ? m_fMaxSprintSpeed : m_fMaxSpeed;

        if ( pRigidBody.velocity.sqrMagnitude < fMaxSpeed * fMaxSpeed )
        {
            // clamp walk force to only add up to max speed
            if ( ( pRigidBody.velocity + WalkForce ).sqrMagnitude > fMaxSpeed * fMaxSpeed )
            {
                float __A = WalkForce.sqrMagnitude;
                float __B = 2 * Vector3.Dot( pRigidBody.velocity, WalkForce );
                float __C = pRigidBody.velocity.sqrMagnitude - fMaxSpeed * fMaxSpeed;
                float fScaleFactor = -__B + Mathf.Sqrt( __B * __B - 4 * __A * __C );
                fScaleFactor /= 2 * __A;
                if ( fScaleFactor < 0 || fScaleFactor > 1 )
                    throw new ArithmeticException( "Invalid scale factor: " + fScaleFactor );
                WalkForce *= fScaleFactor;
            }

            pRigidBody.AddForce( WalkForce, ForceMode.VelocityChange );
        }


        if ( Input.GetKey( KeyCode.Space ) && m_iJumpTimer == 0 && m_iFramesSinceGround < m_iCoyoteFrames )
        {
            m_iJumpTimer = m_iCoyoteFrames + 1;
            pRigidBody.velocity += m_vJumpVector;
            m_iFramesSinceGround += m_iCoyoteFrames;
        }

        // crouch only after we are sure whether or not we're crouch jumping
        if ( m_iCrouchedFrames > m_iCoyoteFrames && m_pUncrouchedObj.activeSelf )
            SetCrouchedState( true );


        //transform.rotation = Quaternion.Slerp( transform.rotation, BodyGoal.transform.rotation, .2f );
        //Head.transform.localRotation = Quaternion.Slerp( Head.transform.localRotation, LookGoal.transform.rotation, .2f );
    }
}
