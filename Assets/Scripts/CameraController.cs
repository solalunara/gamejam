using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float m_fStartRotatingZ = -5.0f;
    public float m_fEndRotatingZ = -10.0f;
    Camera m_pCamera;
    GameObject m_pCameraReference;
    public GameObject m_pPlayer;
    public GameObject CameraReference
    {
        get => m_pCameraReference;
        set {
            m_pCameraReference = value;
            transform.position = m_pCameraReference.transform.position;
        }
    }

    void OnEnable()
    {
        m_pCamera = GetComponentInChildren<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if ( !m_pCamera )
            return;

        Vector3 vPlanePos = Vector3.ProjectOnPlane( m_pPlayer.GetComponent<PlayerBodyController>().transform.position, Vector3.up ) - m_pCamera.transform.parent.position;
        transform.rotation = Quaternion.identity;
        if ( m_pCameraReference && m_pCameraReference.GetComponent<CameraReference>().m_bMoveCameraNearBottom )
            if ( vPlanePos.z < m_fStartRotatingZ )
                transform.rotation = Quaternion.AngleAxis( Mathf.LerpAngle( 0.0f, 90.0f, Mathf.Min( ( -vPlanePos.z - -m_fStartRotatingZ ) / ( m_fStartRotatingZ - m_fEndRotatingZ ), 1.0f ) ), Vector3.up );
        m_pCamera.transform.LookAt( vPlanePos / 5 + m_pCamera.transform.parent.position + new Vector3( 0, 5, -5 ) );
        m_pPlayer.GetComponent<PlayerBodyController>().m_pInteractionPrompt.transform.rotation = Quaternion.LookRotation( m_pCamera.transform.forward );
    }
}
