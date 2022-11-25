using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Shooting", menuName = "ScriptableObjects/Shooting", order = 1)]
public class Shooting : ScriptableObject
{
    [SerializeField]
    private GameObject bulletPrefab;
    [SerializeField]
    private Vector3 bulletSpawningOffset;

    public float BulletSpeed = 5;
    public float BulletLifeTime = 3f;

}
