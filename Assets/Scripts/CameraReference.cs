using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraReference : MonoBehaviour
{
    CameraController m_pCameraController;
    void Start()
    {
        m_pCameraController = FindObjectOfType<CameraController>();
    }
    void OnTriggerEnter( Collider c )
    {
        m_pCameraController.CameraReference = this.gameObject;
    }
}
