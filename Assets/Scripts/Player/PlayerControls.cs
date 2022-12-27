using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

namespace Player
{
    public class PlayerControls : MonoBehaviour
    {
        public bool UsesDifferentAvatars = true;
        public bool TwoSideAnimation = false;
        public Unit PlayerCharacterUnit = null;
        public AnimationAvatarType CurrentSelectedWeapon = AnimationAvatarType.SwordAttack;
        public Shooting shooting;
        public List<Unit> AlreadyHittedTargets = new List<Unit>();
        public float percentageTimeOfAttackSwing = 0.25f;


        [HideInInspector]
        public Vector3 LastDirection;
        [HideInInspector]
        public bool AttackAnimatinoBeingPlayed = false;


        private Vector3 movement;
        [SerializeField]
        private SplashAttackHitbox hitBox;
        [SerializeReference]
        private GameObject[] animationAvatars;
        [SerializeField]
        private Animator animator = null;
        [SerializeField]
        private VariableJoystick joystick;

        private void Awake()
        {
            Debug.Log(gameObject.name);
            GameManager.playerControls = this;
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
                if (TwoSideAnimation == false) transform.eulerAngles = new Vector3(0, 0, 0);
                transform.Translate(movement * PlayerCharacterUnit.Stats.MoveSpeed * Time.fixedDeltaTime);
            }
            if (TwoSideAnimation == false && movement.x < 0) transform.eulerAngles = new Vector3(0, -180, 0);
        }

        /// <summary>
        /// Triggered by button script in Unity
        /// </summary>
        public void AttackInputCheck()
        { 
            if (AttackAnimatinoBeingPlayed == false && PlayerCharacterUnit != null)
            {
                if (animator.gameObject.activeInHierarchy == false) Debug.LogError("Attack input check button running on wrong object");
                animator.SetFloat("AttackAnimationSpeed", PlayerCharacterUnit.Stats.AttackSpeed);
                //checking for input to change facing direction of character, but not no actually move it
                if (GameManager.playerControls.PlayerCharacterUnit.Stats.attackType == Unit.AttackType.Melee)
                {
                    Debug.Log(animator.gameObject.name);
                    animator.SetBool("Attacked", true);
                    ChangeAvatar(CurrentSelectedWeapon);
                    AlreadyHittedTargets.Clear();
                }
                else if (GameManager.playerControls.PlayerCharacterUnit.Stats.attackType == Unit.AttackType.Ranged)
                {
                    Debug.Log("ranged attack");
                    shooting.Shoot(PlayerCharacterUnit, NormalizeDirection(LastDirection));
                    animator.SetBool("Attacked", true);
                    ChangeAvatar(DirectionToGunAvatar(NormalizeDirection(LastDirection)));
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

            
            if (movement.y > 0.85f) animator.SetFloat("XSpeed", 0);
            else animator.SetFloat("XSpeed", movement.x);

            if (movement.x > 0.25f) animator.SetFloat("YSpeed", 0);
            else animator.SetFloat("YSpeed", movement.y);

            animator.SetFloat("MovementAnimationSpeed", PlayerCharacterUnit.Stats.MoveSpeed / 2);

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

                CorrectHitBoxPosition();
            }
        }
        private void CorrectHitBoxPosition()
        {
            if (UsesDifferentAvatars == false) return;
            if (Mathf.Abs(LastDirection.x) > Mathf.Abs(LastDirection.y)) hitBox.transform.localPosition = hitBox.SidePosition;
            else
            {
                if (LastDirection.y > 0) hitBox.transform.localPosition = hitBox.UpperPosition;
                else if (LastDirection.y < 0) hitBox.transform.localPosition = hitBox.DownPosition;
            }
        }
        public void ChangeAvatar(AnimationAvatarType NewAvatar)
        {
            if (UsesDifferentAvatars == false) return;
            foreach (var Avatar in animationAvatars) Avatar.SetActive(false);
            animationAvatars[(int)NewAvatar].SetActive(true);
        }

        public void EndAttackingState()
        {
            ChangeAvatar(AnimationAvatarType.NoWeapon);
            AttackAnimatinoBeingPlayed = false;
            animator.SetBool("Attacked", false);
            hitBox.gameObject.SetActive(false);
        }

        public static void RespawnPlayer()
        {
            GameManager.playerControls.PlayerCharacterUnit.Stats.CurrentHP = GameManager.playerControls.PlayerCharacterUnit.Stats.MaxHP;
            GameManager.playerControls.gameObject.SetActive(true);
            GameManager.playerControls.AttackAnimatinoBeingPlayed = false;
        }
        public void EnableHitBox()
        {
            hitBox.gameObject.SetActive(true);
        }
        private Vector3 NormalizeDirection(Vector3 normalizedDirection)
        {
            Vector3 newDirection = Vector3.zero;
            if (Mathf.Abs(normalizedDirection.x) > Mathf.Abs(normalizedDirection.y))
            {
                if (normalizedDirection.x > 0) newDirection.x = 1;
                else newDirection.x = -1;
            }
            else
            {
                if (normalizedDirection.y > 0) newDirection.y = 1;
                else newDirection.y = -1;
            }
            return newDirection;
        }
        private void OnDisable()
        {
            movement = Vector3.zero;
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
