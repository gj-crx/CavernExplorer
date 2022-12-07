using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Shooting", menuName = "ScriptableObjects/Shooting", order = 1)]
public class Shooting : ScriptableObject
{
    [SerializeField]
    private Projectile.ProjectileStats projectileStats;

    [SerializeField]
    private GameObject bulletPrefab;
    [SerializeField]
    private Vector3 bulletSpawningOffset;
    [SerializeField]
    private float bulletInitialDistance = 1f;



    public void Shoot(Unit shootingUnit, Vector3 direction)
    {
        Vector3 BulletPosition = shootingUnit.transform.position + bulletSpawningOffset + (direction * bulletInitialDistance);
        GameObject bullet = GameObject.Instantiate(bulletPrefab, BulletPosition, Quaternion.identity);
        projectileStats.Damage = shootingUnit.Stats.Damage;
        bullet.GetComponent<Projectile>().SetProjectileValues(projectileStats, shootingUnit);
        bullet.GetComponent<Projectile>().RotateWithDirection(direction);
    }
}
