using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Types;
using ServerSideLogic.Behaviors;
using System.Threading.Tasks;
using System.Threading;

namespace ServerSideLogic
{
    public class Unit
    {
        /// <summary>
        /// refers to the game this unit belongs to
        /// </summary>
        public Game game;
        /// <summary>
        /// main key for synchronization, refers to AllUnits in Database
        /// </summary>
        public int ID = -1;
        /// <summary>
        /// refers to unit type in TypesData
        /// </summary>
        public byte ArmyID = 0;
        public UnitType Type;



        public State state;
        public float CurrentHP = 100; //high synced value
        public float[] ResourcesCarried = new float[4];
        public Vector3 position;
        public bool IsDead { get; private set; } = false;

        public List<Vector2Int> Way = new List<Vector2Int>();
        public int CurrentDistance { private get; set; }

        public delegate void OnKill(Unit killed);
        public delegate void UnitControlling();

        public OnKill OnKilled;
        public UnitControlling unitActionsControlling;
        public IBehavior behavior;

        /// <summary>
        /// 0 - regular unit class, 1 - building class, 2 - tree class
        /// </summary>
        public Unit(Game GameToCreateUnit, Vector3 Position, State state, byte TypeID)
        {
            game = GameToCreateUnit;
            Type = TypesData.AllUnitTypes[TypeID];
            game.unitsController.AddNewUnit(this, TypesData.AllUnitTypes[TypeID].Class);
            this.position = Position;
            this.state = state;
            if (state != null)
            {
                state.UnitsOfState.Add(this);
            }
            OnKilled = OnKillMethod;
            CurrentDistance = 1;
            unitActionsControlling = UnitActionsControlling;
            //    Debug.Log("unit " + ID + " " + Type.Class + " " + Type.UnitTypeName);
            //change obstacles map
            GameToCreateUnit.map.ApplyObstacle(this);
            GetBehavior(Type);
            if (behavior != null) behavior.StartIterations(game.unitsController.NormalUnitActionsControllingDelay, Random.Range(0, 400));
            if (Type.Stats.MoveSpeed > 0) StartControllingUnitMovements(game.unitsController.NormalUnitMovementDelay, Random.Range(0, 400));
            GameNetCoordinator.Singleton.unitSynchronizator.UnitSynchronizationProcessingAsync(this, Random.Range(0, 100));
        }

        public bool GetWayTarget(Vector3 target)
        {
            if (Type.Stats.MoveSpeed == 0)
            {
                Debug.Log("Attempting to move unit with 0 movespeed");
            }
            bool Result = game.pf.GetWayPath(this, target, 2);
            if (game.DebugMode)
            {
                game.TestWay = game.pf.GetLastWay();
            }
            return Result;
        }
        public Vector3 PositionNextToUnit(Vector3 From)
        {
            Vector3 direction = position - From;
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.z))
            {
                if (direction.x > 0) return position + new Vector3(Type.Stats.ObstacleRadius + 1, 0, 0);
                else return position + new Vector3(Type.Stats.ObstacleRadius - 1, 0, 0);
            }
            else
            {
                if (direction.z > 0) return position + new Vector3(0, 0, Type.Stats.ObstacleRadius + 1);
                else return position + new Vector3(0, 0, Type.Stats.ObstacleRadius - 1);
            }
        }
        public void Death()
        { 
            IsDead = true;
            Debug.Log(Type.UnitTypeName + " " + ID + " killed");
            if (behavior != null)
            {
                behavior.Clear();
            }
            GameNetCoordinator.Singleton.unitSynchronizator.UnitDeathClientRpc(ID, game.SendingClientParams);
            game.unitsController.DeleteUnit(this);
        }
        private void OnKillMethod(Unit KilledUnit)
        {
            if (behavior != null)
            {
                behavior.BehaviorAction();
            }
        }
        /// <summary>
        /// unit actions can only be controlled through this method
        /// </summary>
        private void UnitActionsControlling()
        {
            if (behavior != null && behavior.HaveOrder == false && behavior.Active)
            {
              //  await behavior.StartIterations(game.unitsController.NormalUnitActionsControllingDelay, Random.Range(0, 1000));
            }

        }
        private async Task StartControllingUnitMovements(int ActualDelay, int RandomizedPreDelay = 0)
        {
            await Task.Delay(RandomizedPreDelay);
            while (game.StillRunning)
            {
                if (Way.Count > 0)
                {
                    WayMoving();
                }
                await Task.Delay(ActualDelay);
            }
        }
        private void GetBehavior(UnitType type)
        {
            if (state != null)
            {
                if (type.UnitTag == "Worker")
                {
                    Fighting _fighting = new Fighting(this);
                    Worker w = new Worker(this, _fighting);
                    OnKilled += w.OnUnitKillDelegated;
                    behavior = w;
                }
                
            }
        }
        private void MoveUnit(Vector3 To)
        {
            Vector3 MovementVector = UnitLogic.VectorToDirection(To - position);
            position += (MovementVector * game.TimeMSConstante * Type.Stats.MoveSpeed);
            //   Debug.Log(UnitName + position + " changed");
        }
        private void WayMoving()
        {
            if (Type.Stats.MoveSpeed == 0)
            {
                return;
            }
            //   Debug.Log("waymoving applyed");
            if (Vector3.Distance(position, game.pf.Vector2IntToVector3(Way[CurrentDistance], position.y)) < Type.Stats.MoveSpeed * 0.1f)
            {
                //  Debug.Log(Way[CurrentDistance] + " next way point of the distance " + CurrentDistance);
                position = game.pf.Vector2IntToVector3(Way[CurrentDistance], position.y);
                CurrentDistance++;
            }
            if (CurrentDistance >= Way.Count)
            {
                Way.Clear();
                CurrentDistance = 1;
                behavior.HaveOrder = false;
            }
            else
            {
                MoveUnit(game.pf.Vector2IntToVector3(Way[CurrentDistance], position.y));
            }
        }

        /// <summary>
        /// Contains all data (related to this unit) that should be transfered to clients from server
        /// </summary>
        public struct UnitSerializableData : INetworkSerializable
        {
            /// <summary>
            /// ID in units pool
            /// </summary>
            public int UnitObjectID;
            /// <summary>
            /// refers to GameData.UnitTypes
            /// </summary>
            public byte ArmyID;
            public byte UnitTypeID;
            public short StateID;
            public float CurrentHP;
            public Vector3 position;
            /// <summary>
            /// -1 means that this unit is not attacking anyone
            /// </summary>
            public int VictimOfAttackUnitID;
            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref UnitObjectID);
                serializer.SerializeValue(ref ArmyID);
                serializer.SerializeValue(ref UnitTypeID);
                serializer.SerializeValue(ref StateID);
                serializer.SerializeValue(ref CurrentHP);
                serializer.SerializeValue(ref position);
                serializer.SerializeValue(ref VictimOfAttackUnitID);
            }
            public UnitSerializableData(Unit unit)
            {
                UnitObjectID = unit.ID;
                ArmyID = unit.ArmyID;
                UnitTypeID = unit.Type.UnitTypeID;
                if (unit.state != null)
                {
                    StateID = unit.state.StateID;
                }
                else
                {
                    StateID = -1;
                }
                CurrentHP = unit.CurrentHP;
                position = unit.position;
                if (unit.behavior != null) VictimOfAttackUnitID = unit.behavior.CurrentTargetID;
                else VictimOfAttackUnitID = -1;
            }
        }
        public enum UnitClass : byte
        {
            RegularUnit = 0,
            Building = 1,
            Tree = 2
        }
        private enum UnitAnimation : byte
        {
            IdleOrMoving = 0,
            Attack = 1,
            ResourceGathering = 2,
            SpellCasting = 3
        }
    }
}
