using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowing : MonoBehaviour
{
    private float ZCord;
    private void Start()
    {
        ZCord = transform.position.z;
    }
    void Update()
    {
        transform.position = new Vector3(GameManager.PlayerRelatedCharacters[0].transform.position.x, GameManager.PlayerRelatedCharacters[0].transform.position.y, ZCord);
    }
}
