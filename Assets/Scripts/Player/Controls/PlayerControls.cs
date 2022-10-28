using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

namespace Player
{
    public class PlayerControls : MonoBehaviour
    {
        public Unit PlayerCharacterUnit = null;
        public AnimationAvatarType CurrentSelectedWeapon = AnimationAvatarType.SwordAttack;
        public List<Unit> AlreadyHittedTargets = new List<Unit>();

        [HideInInspector]
        public static PlayerControls Singleton;
        [HideInInspector]
        public Vector3 LastDirection;
        [HideInInspector]
        public bool AttackAnimatinoBeingPlayed = false;


        private Vector3 Movement;
        
        [SerializeField]
        private Hitbox _hitBox;
        [SerializeReference]
        private GameObject[] AnimationAvatars;
        [SerializeField]
        private GameObject Prefab_Bullet;
        [SerializeField]
        private Vector3 ShootingBulletsOffset = new Vector3(0, 0.4f, 0);
        [SerializeField]
        private float ShootingBulletOffsetModifier = 1.2f;
        [SerializeField]
        private Animator _Animator = null;
        [SerializeField]
        private FloatingJoystick joystick;

        private void Awake()
        {
            Singleton = this;
        }

        private void LateUpdate()
        {
            if (AttackAnimatinoBeingPlayed == false)
            {
                if (SystemInfo.deviceType == DeviceType.Handheld || true)
                {
                    Movement = Vector3.up * joystick.Vertical + Vector3.right * joystick.Horizontal;
                }
                else
                {
                    Movement = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
                }
            }
            else Movement = Vector3.zero;

            
            _Animator.SetFloat("XSpeed", Movement.x);
            _Animator.SetFloat("YSpeed", Movement.y);

            if (Movement.magnitude == 0)
            { //movement stops
                _Animator.SetBool("Stopped", true);
                _Animator.SetFloat("LastDirX", LastDirection.x);
                _Animator.SetFloat("LastDirY", LastDirection.y);
            }
            else
            {
                _Animator.SetBool("Stopped", false);
                LastDirection = Movement.normalized;
                //correcting hitbox
                if (LastDirection.x != 0) _hitBox.transform.localPosition = _hitBox.SidePosition;
                else
                {
                    if (LastDirection.y > 0) _hitBox.transform.localPosition = _hitBox.UpperPosition;
                    else if (LastDirection.y < 0) _hitBox.transform.localPosition = _hitBox.DownPosition;
                }
            }
        }

        void FixedUpdate()
        {
            if (Movement.magnitude > 0)
            {
                transform.eulerAngles = new Vector3(0, 0, 0);
                transform.Translate(Movement.normalized * PlayerCharacterUnit.Stats.MoveSpeed * Time.fixedDeltaTime);
            }
            if (Movement.x > 0) transform.eulerAngles = new Vector3(0, -180, 0);
        }

        public void AttackInputCheck()
        {
            Debug.Log("attack");
            if (AttackAnimatinoBeingPlayed == false)
            {
                //checking for input to change facing direction of character, but not no actually move it
                if (GameManager.LocalPlayerHeroUnit.Stats.attackType == Unit.AttackType.Melee)
                {
                    _Animator.SetBool("Attack", true);
                    ChangeAvatar(CurrentSelectedWeapon);
                    Hit();
                }
                else if (GameManager.LocalPlayerHeroUnit.Stats.attackType == Unit.AttackType.Ranged)
                {
                    GameObject.Instantiate(Prefab_Bullet, transform.position + ShootingBulletsOffset + (LastDirection * ShootingBulletOffsetModifier),
                        Quaternion.identity).transform.eulerAngles = new Vector3(0, 0, BasicFunctions.DirectionToAngle(LastDirection));
                    _Animator.SetBool("Attack", true);
                    ChangeAvatar(DirectionToGunAvatar(LastDirection));
                }
            }
        }
        public void ChangeAvatar(AnimationAvatarType NewAvatar)
        {
            foreach (var Avatar in AnimationAvatars) Avatar.SetActive(false);
            AnimationAvatars[(int)NewAvatar].SetActive(true);
        }

        private void Hit()
        {
            _hitBox.gameObject.SetActive(true);
            AlreadyHittedTargets.Clear();
        }
        public void EndAttackingState()
        {
            ChangeAvatar(AnimationAvatarType.NoWeapon);
            AttackAnimatinoBeingPlayed = false;
            _hitBox.gameObject.SetActive(false);
        }


        public enum AnimationAvatarType : int
        {
            NoWeapon = 0,
            SwordAttack = 1,
            GunAttackDown = 2,
            GunAttackUp = 3,
            GunAttackSide = 4,
            ScytheAttack = 5,
            HammerAttack = 6,
            BowAttack = 7
        }
        private AnimationAvatarType DirectionToGunAvatar(Vector3 Direction)
        {
            if (Direction.x != 0 && Mathf.Abs(Direction.x) > Mathf.Abs(Direction.y)) return AnimationAvatarType.GunAttackSide;
            else if (Direction.y > 0) return AnimationAvatarType.GunAttackUp;
            else return AnimationAvatarType.GunAttackDown;
        }
    }
    
}
