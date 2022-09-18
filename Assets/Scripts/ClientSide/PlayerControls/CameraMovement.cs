using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float Speed = 3;

    // Update is called once per frame
    void Update()
    {
        transform.position = transform.position - (new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * Speed);
    }
}
