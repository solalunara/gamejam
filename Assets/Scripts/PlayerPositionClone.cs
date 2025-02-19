using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPositionClone : MonoBehaviour
{
    public GameObject m_pPlayer;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = m_pPlayer.GetComponent<PlayerBodyController>().EyeLevel;
    }
}
