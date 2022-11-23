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


        private Vector3 movement;
        [SerializeField]
        private PlayerHitbox hitBox;
        [SerializeReference]
        private GameObject[] animationAvatars;
        [SerializeField]
        private GameObject prefab_Bullet;
        [SerializeField]
        private Vector3 shootingBulletsOffset = new Vector3(0, 0.4f, 0);
        [SerializeField]
        private float shootingBulletOffsetModifier = 1.2f;
        [SerializeField]
        private Animator animator = null;
        [SerializeField]
        private VariableJoystick joystick;

        private void Awake()
        {
            Singleton = this;
        }

        private void LateUpdate()
        {
            if (PlayerCharacterUnit != null)
            {
                MovementInputCheck();
            }
        }

        void FixedUpdate()
        { //moving and rotation object
            if (movement.magnitude > 0)
            {
                transform.eulerAngles = new Vector3(0, 0, 0);
                transform.Translate(movement.normalized * PlayerCharacterUnit.Stats.MoveSpeed * Time.fixedDeltaTime);
            }
            if (movement.x > 0) transform.eulerAngles = new Vector3(0, -180, 0);
        }

        /// <summary>
        /// Triggered by button script in Unity
        /// </summary>
        public void AttackInputCheck()
        { 
            if (AttackAnimatinoBeingPlayed == false)
            {
                //checking for input to change facing direction of character, but not no actually move it
                if (GameManager.LocalPlayerHeroUnit.Stats.attackType == Unit.AttackType.Melee)
                {
                    animator.SetBool("Attack", true);
                    ChangeAvatar(CurrentSelectedWeapon);
                    Hit();
                }
                else if (GameManager.LocalPlayerHeroUnit.Stats.attackType == Unit.AttackType.Ranged)
                {
                    Vector3 normalizedDirection = NormalizeDirection(LastDirection);
                    Vector3 BulletPosition = transform.position + shootingBulletsOffset + (normalizedDirection * shootingBulletOffsetModifier);
                    GameObject.Instantiate(prefab_Bullet, BulletPosition, Quaternion.identity).transform.eulerAngles = new Vector3(0, 0, BasicFunctions.DirectionToAngle(normalizedDirection));
                    animator.SetBool("Attack", true);
                    ChangeAvatar(DirectionToGunAvatar(normalizedDirection));
                }
            }
        }
        private void MovementInputCheck()
        {
            if (AttackAnimatinoBeingPlayed == false)
            {
                if (SystemInfo.deviceType == DeviceType.Handheld || true)
                {
                    movement = Vector3.up * joystick.Vertical + Vector3.right * joystick.Horizontal;
                }
                else
                {
                    movement = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
                }
            }
            else movement = Vector3.zero;


            animator.SetFloat("XSpeed", movement.x);
            animator.SetFloat("YSpeed", movement.y);
            if (movement.magnitude == 0)
            { //movement stops
                animator.SetBool("Stopped", true);
                animator.SetFloat("LastDirX", LastDirection.x);
                animator.SetFloat("LastDirY", LastDirection.y);
            }
            else
            {
                animator.SetBool("Stopped", false);
                LastDirection = movement.normalized;
                //correcting hitbox
                if (Mathf.Abs(LastDirection.x) > Mathf.Abs(LastDirection.y)) hitBox.transform.localPosition = hitBox.SidePosition;
                else
                {
                    if (LastDirection.y > 0) hitBox.transform.localPosition = hitBox.UpperPosition;
                    else if (LastDirection.y < 0) hitBox.transform.localPosition = hitBox.DownPosition;
                }
            }
        }
        public void ChangeAvatar(AnimationAvatarType NewAvatar)
        {
            foreach (var Avatar in animationAvatars) Avatar.SetActive(false);
            animationAvatars[(int)NewAvatar].SetActive(true);
        }

        private void Hit()
        {
            hitBox.gameObject.SetActive(true);
            AlreadyHittedTargets.Clear();
        }
        public void EndAttackingState()
        {
            ChangeAvatar(AnimationAvatarType.NoWeapon);
            AttackAnimatinoBeingPlayed = false;
            hitBox.gameObject.SetActive(false);
        }
        private Vector3 NormalizeDirection(Vector3 direction)
        {
            Vector3 newDirection = Vector3.zero;
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                if (direction.x > 0) newDirection.x = 1;
                else newDirection.x = -1;
            }
            else
            {
                if (direction.y > 0) newDirection.y = 1;
                else newDirection.y = -1;
            }
            return newDirection;
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
