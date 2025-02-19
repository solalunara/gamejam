using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Camera m_pCamera;
    GameObject m_pCameraReference;
    PlayerBodyController m_pPlayer;
    public GameObject CameraReference
    {
        get => m_pCameraReference;
        set {
            m_pCameraReference = value;
            transform.position = m_pCameraReference.transform.position;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        m_pCamera = FindObjectOfType<Camera>();
        m_pPlayer = FindObjectOfType<PlayerBodyController>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 vPlanePos = Vector3.ProjectOnPlane( m_pPlayer.m_pActiveRigidBody.position, Vector3.up ) - m_pCamera.transform.parent.position;
        m_pCamera.transform.LookAt( vPlanePos / 10 + m_pCamera.transform.parent.position + new Vector3( 0, 0, -5 ) );
    }
}
