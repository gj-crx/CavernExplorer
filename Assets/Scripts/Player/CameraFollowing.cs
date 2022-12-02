using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowing : MonoBehaviour
{
    private float StaticZCord;
    private void Start()
    {
        StaticZCord = transform.position.z;
    }
    void Update()
    {
        if (GameManager.playerControls != null && GameManager.playerControls.isActiveAndEnabled)
        {
            transform.position = new Vector3(GameManager.playerControls.transform.position.x, GameManager.playerControls.transform.position.y, StaticZCord);
        }
    }
}
