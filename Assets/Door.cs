using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] GameObject left_door;
    [SerializeField] GameObject right_door;
    Vector3 left_door_pos;
    Vector3 right_door_pos;
    [SerializeField] bool is_Horizontal;

    private void Awake()
    {
        left_door_pos = left_door.transform.position;
        right_door_pos = right_door.transform.position;
    }

    private void Update()
    {
        Vector3 johnvector = Vector3.Lerp(left_door.transform.position, left_door_pos, 1.5f * Time.deltaTime);
        Vector3 johnathanvector = Vector3.Lerp(right_door.transform.position, right_door_pos, 1.5f * Time.deltaTime);
        left_door.transform.position = johnvector;
        right_door.transform.position = johnathanvector;

    }

    private void OnTriggerEnter(Collider col)
    {
        if (is_Horizontal)
        {
            left_door_pos = left_door_pos + new Vector3(-4, 0, 0);
            right_door_pos = right_door_pos + new Vector3(4, 0, 0);
        }
        else
        {
            left_door_pos = left_door_pos + new Vector3(0, 0, -4);
            right_door_pos = right_door_pos + new Vector3(0, 0, 4);
        }
    }
    private void OnTriggerExit(Collider col)
    {
        if (is_Horizontal)
        {
            left_door_pos = left_door_pos + new Vector3(4, 0, 0);
            right_door_pos = right_door_pos + new Vector3(-4, 0, 0);
        }
        else
        {
            left_door_pos = left_door_pos + new Vector3(0, 0, 4);
            right_door_pos = right_door_pos + new Vector3(0, 0, -4);
        }
    }
}
