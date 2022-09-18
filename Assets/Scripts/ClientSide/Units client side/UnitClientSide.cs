using Unity.Netcode;
using UnityEngine;
using Types;

namespace ClientSideLogic
{
    public class UnitClientSide : MonoBehaviour
    { //represent a basic game object (building, character, tree)
        [Header("Actual versions of UnitType variable that by default same, but can be changed in game")]

        [Header("Synced variables")]
        public float currentHP = 100;
        public float[] a_ResourcesCarried = new float[4];


        public int ID = -1;
        public byte ArmyID = 0;
        public int CurrentTargetID = -1;

        public UnitType _type;
        /// <summary>
        /// -1 equals to no state
        /// </summary>
        public short StateID = -1;


        [SerializeField]
        private Vector3 LatestPosition;
        private Animator _animator;
        [SerializeField]
        private float RotationSpeed = 25;

        private float LatestDistanceBetweenTarget = 0;
        private bool IsMoved = false;
        const float MinimalSpeed = 1;
        const float MinimalDistance = 0.1f;

        private void Start()
        {
            LatestPosition = transform.position;
            try 
            { 
                _animator = GetComponent<Animator>(); 
            }
            catch { }
        }

        private void FixedUpdate()
        {
            if (_animator != null)
            {
                ManageUnitAnimations();
                //rotations
                UnitFacing();
            }
            //transform.position = Vector3.Lerp(transform.position, LatestPosition, Time.deltaTime * ClientGameController.Singleton.ClientSpeedModificator);
            Move(LatestPosition);
        }

        public void ApplyRecievedData(ServerSideLogic.Unit.UnitSerializableData data)
        {
            currentHP = data.CurrentHP;
            ArmyID = data.ArmyID;
            LatestPosition = data.position; //temporal solution
            CurrentTargetID = data.VictimOfAttackUnitID;
        }
        private void Move(Vector3 Target)
        {
            IsMoved = false;
            if (transform.position != Target)
            {
                IsMoved = true;
                Vector3 Direction = Vector3.Normalize(Target - transform.position);
                float speed = Mathf.Max(MinimalSpeed, Vector3.Distance(transform.position, Target));
                transform.position = transform.position + (Direction * speed * Time.fixedDeltaTime);
                LatestDistanceBetweenTarget = Vector3.Distance(transform.position, Target);
                if (LatestDistanceBetweenTarget < MinimalDistance)
                {
                    transform.position = LatestPosition;
                }
            }
        }
        private void RotateUnitTowards(Vector3 point, float speed)
        {
            var direction = (point - transform.position).normalized;
            direction.y = 0f;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(direction), speed * Time.fixedDeltaTime);
        }
        private void ManageUnitAnimations()
        {
            //moving animation
            if (IsMoved)
            {
                _animator.SetBool("IsMoving", true);
            }
            else _animator.SetBool("IsMoving", false);
            //fighting animation
            _animator.SetBool("IsFighting", false);
            if (CurrentTargetID != -1)
            {
                UnitClientSide CurrentTarget = ClientGameController.Singleton.dataBase.AllUnits[CurrentTargetID];
                if (CurrentTarget != null)
                {
                    if (LatestDistanceBetweenTarget < _type.Stats.AttackDelay) _animator.SetBool("IsFighting", true);
                }
                else CurrentTargetID = -1;
            }
        }
        private void UnitFacing()
        {
            if (CurrentTargetID == -1) RotateUnitTowards(LatestPosition, RotationSpeed);
            else
            {
                if (LatestDistanceBetweenTarget < _type.Stats.AttackRange)
                {
                    RotateUnitTowards(ClientGameController.Singleton.dataBase.AllUnits[CurrentTargetID].transform.position, RotationSpeed);
                }
                else
                {
                    RotateUnitTowards(LatestPosition, RotationSpeed);
                }
            }
        }
        
    }
}
