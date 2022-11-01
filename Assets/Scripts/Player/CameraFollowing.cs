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
        if (GameManager.PlayerRelatedCharacters[0].isActiveAndEnabled)
        {
            transform.position = new Vector3(GameManager.PlayerRelatedCharacters[0].transform.position.x, GameManager.PlayerRelatedCharacters[0].transform.position.y, StaticZCord);
        }
    }
}
