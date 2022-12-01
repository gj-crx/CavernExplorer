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
    [SerializeField]
    private float bulletInitialDistance = 1f;

    public float BulletSpeed = 5;
    public float BulletLifeTime = 3f;


    public void Shoot(Unit shootingUnit, Vector3 direction)
    {
        Vector3 BulletPosition = shootingUnit.transform.position + bulletSpawningOffset + (direction * bulletInitialDistance);
        GameObject bullet = GameObject.Instantiate(bulletPrefab, BulletPosition, Quaternion.identity);
        bullet.GetComponent<Bullet>().SetBulletValues(this, shootingUnit);

        float rotationZCord = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        bullet.transform.rotation = Quaternion.Euler(0f, 0f, rotationZCord - 90);

    }
}
