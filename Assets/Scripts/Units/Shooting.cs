using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Shooting", menuName = "ScriptableObjects/Shooting", order = 1)]
public class Shooting : ScriptableObject
{
    [SerializeField]
    private Projectile.ProjectileStats projectileStats;

    [SerializeField]
    private int projectilesMultishotCount = 1;
    [SerializeField]
    private float projectileMultishotOffsetY = 0.25f;
    [SerializeField]
    private GameObject bulletPrefab;
    [SerializeField]
    private Vector3 bulletSpawningOffset;
    [SerializeField]
    private float bulletInitialDistance = 1f;
    
   



    public void Shoot(Unit shootingUnit, Vector3 direction)
    {
        for (int multishotNumber = 0; multishotNumber < projectilesMultishotCount; multishotNumber++)
        {
            Vector3 BulletPosition = shootingUnit.transform.position + bulletSpawningOffset + (direction * bulletInitialDistance) + GetProjectileOffset(multishotNumber);
            GameObject bullet = GameObject.Instantiate(bulletPrefab, BulletPosition, Quaternion.identity);
            projectileStats.Damage = shootingUnit.Stats.Damage;
            bullet.GetComponent<Projectile>().SetProjectileValues(projectileStats, shootingUnit);
            bullet.GetComponent<Projectile>().RotateWithDirection(direction);
        }
    }
    private Vector3 GetProjectileOffset(int projectileNumber)
    {
        if ((float)projectileNumber / 2 > projectileNumber / 2) return new Vector3(0, projectileMultishotOffsetY * projectileNumber);
        else return new Vector3(0, -projectileMultishotOffsetY * projectileNumber);
    }
}
