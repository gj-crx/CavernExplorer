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
        if (GameManager.LocalPlayerHeroUnit != null && GameManager.LocalPlayerHeroUnit.isActiveAndEnabled)
        {
            transform.position = new Vector3(GameManager.LocalPlayerHeroUnit.transform.position.x, GameManager.LocalPlayerHeroUnit.transform.position.y, StaticZCord);
        }
    }
}
