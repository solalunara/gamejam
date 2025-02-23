using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alarm : MonoBehaviour
{
    void OnEnable()
    {
        StopAllCoroutines();
        StartCoroutine(nameof(DisableAfterTime));
    }

    IEnumerator DisableAfterTime()
    {
        yield return new WaitForSeconds( 5.0f );
        gameObject.SetActive( false );
    }
}
