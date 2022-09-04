using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class Hitbox : MonoBehaviour
    {
        private PlayerControls _controls;
        public Vector3 UpperPosition;
        public Vector3 DownPosition;
        public Vector3 SidePosition;

        private void Start()
        {
            _controls = PlayerControls.Singleton;

        }
        private void OnTriggerStay2D(Collider2D  collision)
        {
            if (collision.gameObject.tag != "Creep") return;
            var _target = collision.gameObject.GetComponent<Unit>();
            if (_controls.AlreadyHittedTargets.Contains(_target) == false)
            {
                _target.GetDamage(_controls.PlayerCharacterUnit.Stats.Damage, _controls.PlayerCharacterUnit);
                _controls.AlreadyHittedTargets.Add(_target);
            }
        }
    }
}
