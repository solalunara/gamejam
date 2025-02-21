using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
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

    // Start is called before the first frame update
    void Start()
    {
        m_pCamera = GetComponentInChildren<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 vPlanePos = Vector3.ProjectOnPlane( m_pPlayer.GetComponent<PlayerBodyController>().ActiveRigidBody.position, Vector3.up ) - m_pCamera.transform.parent.position;
        m_pCamera.transform.LookAt( vPlanePos / 10 + m_pCamera.transform.parent.position + new Vector3( 0, 5, -5 ) );
        m_pPlayer.GetComponent<PlayerBodyController>().m_pInteractionPrompt.transform.rotation = Quaternion.LookRotation( m_pCamera.transform.forward );
    }
}
