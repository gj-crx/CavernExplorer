using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class SplashAttackHitbox : MonoBehaviour
    {
        private PlayerControls controls;

        public bool IsBullet = false;
        public List<string> TargetsTagsToDamage = new List<string>();

        public Vector3 UpperPosition;
        public Vector3 DownPosition;
        public Vector3 SidePosition;

        private void Start()
        {
            controls = PlayerControls.Singleton;

        }
        private void OnTriggerStay2D(Collider2D  collision)
        {
            if (IsBullet == false) MeleeHit(collision);
            else BulletHit(collision);
        }
        private void MeleeHit(Collider2D collision)
        {
            if (TargetsTagsToDamage.Contains(collision.gameObject.tag) == false) return;

            var target = collision.gameObject.GetComponent<Unit>();
            if (controls.AlreadyHittedTargets.Contains(target) == false)
            {
                target.GetDamage(controls.PlayerCharacterUnit.Stats.Damage, controls.PlayerCharacterUnit);
                controls.AlreadyHittedTargets.Add(target);
            }
        }
        private void BulletHit(Collider2D collision)
        {
            if (TargetsTagsToDamage.Contains(collision.gameObject.tag))
            {
                collision.gameObject.GetComponent<Unit>().GetDamage(controls.PlayerCharacterUnit.Stats.Damage, controls.PlayerCharacterUnit);
            }
            if (collision.isTrigger == false && collision.tag != "Corpse")
            {
                Destroy(gameObject); //any collision event destroys bullet
            }
        }
    }
}
