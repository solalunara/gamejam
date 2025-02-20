using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Score : MonoBehaviour
{
    float m_fScore;
    // Start is called before the first frame update
    void OnEnable()
    {
        m_fScore = Time.time - FaultListBody.StartTime;
        GetComponent<TMP_Text>().text = m_fScore.ToString();
    }
}
