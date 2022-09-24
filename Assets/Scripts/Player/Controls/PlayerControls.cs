using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

namespace Player
{
    public class PlayerControls : MonoBehaviour
    {
        [HideInInspector]
        public static PlayerControls Singleton;
        public Unit PlayerCharacterUnit = null;

        public AnimationAvatarType CurrentSelectedWeapon = AnimationAvatarType.SwordAttack;
        [HideInInspector]
        public Vector3 LastDirection;

        [SerializeField]
        private Animator _Animator = null;



        private Vector3 Movement;
        [HideInInspector]
        public bool AttackAnimatinoBeingPlayed = false;
        public List<Unit> AlreadyHittedTargets = new List<Unit>();
        [SerializeField]
        private Hitbox _hitBox;
        [SerializeReference]
        private GameObject[] AnimationAvatars;
        private GameObject CurrentAvatar = null;
        [SerializeField]
        private GameObject Prefab_Bullet;
        [SerializeField]
        private Vector3 ShootingBulletsOffset = new Vector3(0, 0.4f, 0);

        private void Awake()
        {
            Singleton = this;
            CurrentAvatar = gameObject.transform.Find("Avatar").gameObject;
        }

        private void Update()
        {
            AttackInputCheck();

            if (AttackAnimatinoBeingPlayed == false)
            {
                Movement = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            }
            else Movement = Vector3.zero;

            
            _Animator.SetFloat("XSpeed", Movement.x);
            _Animator.SetFloat("YSpeed", Movement.y);

            if (Movement.magnitude == 0)
            {
                _Animator.SetBool("Stopped", true);
                _Animator.SetFloat("LastDirX", LastDirection.x);
                _Animator.SetFloat("LastDirY", LastDirection.y);
            }
            else
            {
                _Animator.SetBool("Stopped", false);
                LastDirection = Movement.normalized;
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

        private void AttackInputCheck()
        {
            if (Input.GetButtonDown("Fire1") && AttackAnimatinoBeingPlayed == false)
            {
                if (GameManager.LocalPlayerHeroUnit.Stats.attackType == Unit.AttackType.Melee)
                {
                    _Animator.SetBool("Attack", true);
                    ChangeAvatar(CurrentSelectedWeapon);
                    Hit();
                }
                else if (GameManager.LocalPlayerHeroUnit.Stats.attackType == Unit.AttackType.Ranged)
                {
                    GameObject.Instantiate(Prefab_Bullet, transform.position + ShootingBulletsOffset + (LastDirection.normalized * 1f),
                        Quaternion.identity).transform.eulerAngles = new Vector3(0, 0, BasicFunctions.DirectionToAngle(LastDirection));
                }
            }
        }
        public void ChangeAvatar(AnimationAvatarType NewAvatar)
        {
            CurrentAvatar.SetActive(false);
            AnimationAvatars[(int)NewAvatar].SetActive(true);
            CurrentAvatar = AnimationAvatars[(int)NewAvatar];
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
            GunAttack = 2,
            ScytheAttack = 3,
            HammerAttack = 4,
            BowAttack = 5
        }
    }
    
}
