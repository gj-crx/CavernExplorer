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
            Vector3 inputTargetDelta;
            if (SystemInfo.deviceType == DeviceType.Handheld) inputTargetDelta = BasicFunctions.RemoveZCord(Camera.main.ScreenToWorldPoint(Input.touches[Input.touchCount - 1].position) - PlayerCharacterUnit.transform.position);
            else inputTargetDelta = BasicFunctions.RemoveZCord(Camera.main.ScreenToWorldPoint(Input.mousePosition) - PlayerCharacterUnit.transform.position);

            //facing character towards attack target
            if (Mathf.Abs(inputTargetDelta.x) > Mathf.Abs(inputTargetDelta.y))
            {
                animator.SetFloat("LastDirX", inputTargetDelta.x);
                animator.SetFloat("LastDirY", 0);
            }
            else
            {
                animator.SetFloat("LastDirX", 0);
                animator.SetFloat("LastDirY", inputTargetDelta.y);
            }
            LastDirection = inputTargetDelta;

            if (AttackAnimatinoBeingPlayed == false && PlayerCharacterUnit != null)
            {
                if (animator.gameObject.activeInHierarchy == false) Debug.LogError("Attack input check button running on wrong object");

                hitBox.CorrectHitBoxPosition(new Vector3(animator.GetFloat("LastDirX"), animator.GetFloat("LastDirY")));
                animator.SetFloat("AttackAnimationSpeed", PlayerCharacterUnit.Stats.AttackSpeed);
                AttackAnimatinoBeingPlayed = true;
                //checking for input to change facing direction of character, but not no actually move it
                if (GameManager.playerControls.PlayerCharacterUnit.Stats.attackType == Unit.AttackType.Melee)
                {
                    animator.SetBool("Attacked", true);
                    ChangeAvatar(CurrentSelectedWeapon);
                    AlreadyHittedTargets.Clear();
                }
                else if (GameManager.playerControls.PlayerCharacterUnit.Stats.attackType == Unit.AttackType.Ranged)
                {
                    animator.SetBool("Shooted", true);
                    ChangeAvatar(DirectionToGunAvatar(NormalizeAndRoundDirection(LastDirection)));
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

                if (movement.y > 0.9f) animator.SetFloat("XSpeed", 0);
                else animator.SetFloat("XSpeed", movement.x);

                if (movement.x > 0.1f) animator.SetFloat("YSpeed", 0);
                else animator.SetFloat("YSpeed", movement.y);

                animator.SetFloat("MovementAnimationSpeed", PlayerCharacterUnit.Stats.MoveSpeed / 2 * movement.magnitude);

                if (movement == Vector3.zero)
                { //movement stops
                    animator.SetBool("Stopped", true);
                    animator.SetFloat("LastDirX", LastDirection.x);
                    animator.SetFloat("LastDirY", LastDirection.y);
                }
                else
                {
                    animator.SetBool("Stopped", false);
                    LastDirection = movement.normalized;

                    if (movement != Vector3.zero) hitBox.CorrectHitBoxPosition(new Vector3(animator.GetFloat("XSpeed"), animator.GetFloat("YSpeed")));
                }
            }
            else movement = Vector3.zero;
        }
        
        public void ChangeAvatar(AnimationAvatarType NewAvatar)
        {
            if (UsesDifferentAvatars == false) return;
            foreach (var Avatar in animationAvatars) Avatar.SetActive(false);
            animationAvatars[(int)NewAvatar].SetActive(true);
        }

        public void EndAttackingState()
        {
            //check for shooting (it fires after the animation)
            if (animator.GetBool("Shooted") == true) shooting.Shoot(PlayerCharacterUnit, LastDirection.normalized);

            ChangeAvatar(AnimationAvatarType.NoWeapon);
            AttackAnimatinoBeingPlayed = false;
            animator.SetBool("Attacked", false);
            animator.SetBool("Shooted", false);
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
        private Vector3 NormalizeAndRoundDirection(Vector3 normalizedDirection)
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
