using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class SplashAttackHitbox : MonoBehaviour
    {
        private PlayerControls controls;
        public List<string> TargetsTagsToDamage = new List<string>();

        public bool UsesDifferentPositions = false;
        public Vector3 UpperPosition;
        public Vector3 DownPosition;
        public Vector3 SidePosition;
        public Vector3 LeftSidePosition;

        private void Start()
        {
            controls = GameManager.playerControls;

        }

        private void OnTriggerStay2D(Collider2D  collision)
        {
            MeleeHit(collision);
        }

        public void CorrectHitBoxPosition(Vector3 animationDirections)
        {
            if (UsesDifferentPositions == false) return;
            if (Mathf.Abs(animationDirections.x) > Mathf.Abs(animationDirections.y))
            {
                if (animationDirections.x > 0 || LeftSidePosition == Vector3.zero) transform.localPosition = SidePosition;
                if (animationDirections.x < 0) transform.localPosition = LeftSidePosition;
            }
            else
            {
                if (animationDirections.y > 0) transform.localPosition = UpperPosition;
                else if (animationDirections.y < 0) transform.localPosition = DownPosition;
            }
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
    }
}
