using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Types;
using Unity.Netcode;
using ClientSideLogic;
using System.Threading.Tasks;
using ServerSideLogic.AI;

namespace ServerSideLogic
{
    public class State : IState
    {
        private Game _game;
        public byte StateRaceID = 0;
        public string StateName = "Kazakhstan";

        public short StateID = 0;
        public Player ControllingPlayer;
        private int ControllingPlayerID = 0;
        public Vector3 position;

        public bool[] UnlockedTechnologies = new bool[100];

        public float[] ResourcesAmount = new float[4];
        public List<Unit> UnitsOfState = new List<Unit>();
        public List<byte> TrainingQueuedUnitsTypeIDs = new List<byte>();
        public Building Townhall;

        private AIController _aiController = null;

        public int[] CountUnitsOfType
        {
            get
            {
                int[] UnitsOfTypeCount = new int[TypesData.AllUnitTypes.Length];
                foreach (Unit _unit in UnitsOfState)
                {
                    UnitsOfTypeCount[_unit.Type.UnitTypeID]++;
                }
                foreach (var QueuedUnitTypeID in TrainingQueuedUnitsTypeIDs)
                {
                    UnitsOfTypeCount[QueuedUnitTypeID]++;
                }
                return UnitsOfTypeCount;
            }
        }




        public State(Game game, byte Race, Vector3 TownhallPosition, Player ControllingPlayer, string StateName = "Kazakhstan", bool AIControlled = false)
        {
            StateRaceID = Race;
            this._game = game;
            BuildingType TownhallType = TypesData.GetBuildingTypeByTag("Townhall", StateRaceID);
            Townhall = new Building(TownhallType, game, TownhallPosition, this, TownhallType.UnitTypeID);
            position = TownhallPosition;
            UnlockedTechnologies[0] = true;
            this.StateName = StateName;
            StateID = game.statesController.AddState(this);

            if (AIControlled == false)
            {
                this.ControllingPlayer = ControllingPlayer;
                this.ControllingPlayerID = (int)ControllingPlayer.OwnerClientId;
                this.ControllingPlayer.serverSide_ControlledState = this;
            }
            else
            {
                this.ControllingPlayer = null;
                this.ControllingPlayerID = StateID;
                _aiController = new AIController(this, 350);
            }
            SpawnStartingUnits();

            GameNetCoordinator.Singleton.statesSynchronizator.ApplyStateDataToClientRpc(new State.StateSerializableData(this), game.SendingClientParams);
            StartControllingState(game.statesController.NormalDelayToControlState, Random.Range(0, 1000));
            StartStateResourcesSynchronization(GameNetCoordinator.Singleton.statesSynchronizator.NormalDelayFor1State, Random.Range(0, 100));
        }

        private void SpawnStartingUnits()
        {
            if (StateRaceID == 0 || StateRaceID == 1)
            {
                for (int i = 0; i < 1; i++)
                {
                    Unit StartingWorker = new Unit(_game, Townhall.position + new Vector3(Random.Range(-16, 16f), 0, Random.Range(-16, 16f)),
                        this, TypesData.GetUnitTypeByTag("Worker", StateRaceID).UnitTypeID);
                }
            }
        }

        private async Task StartControllingState(int ActualDelay, int RandomizedPreDelay = 0)
        {
            await Task.Delay(RandomizedPreDelay);
            while (_game.StillRunning)
            {
                StateControllingActions();
                await Task.Delay(ActualDelay);
            }
        }
        private async Task StartStateResourcesSynchronization(int ActualDelay, int RandomizedPreDelay = 0)
        {
            await Task.Delay(RandomizedPreDelay);
            while (_game.StillRunning)
            {
                GameNetCoordinator.Singleton.statesSynchronizator.SyncStateResourcesClientRpc(ResourcesAmount, StateID, _game.SendingClientParams);
                await Task.Delay(ActualDelay);
            }
        }
        private void StateControllingActions()
        {

        }

        public Building GetTrainingBuilding()
        {
            return Townhall;
        }

        public struct StateSerializableData : INetworkSerializable
        {
            public short StateID;
            public byte RaceID;
            public int ControllingPlayerID;
            public string StateName;
            public Vector3 position;

            public float[] ResourcesAmount;
            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref StateID);
                serializer.SerializeValue(ref RaceID);
                serializer.SerializeValue(ref ControllingPlayerID);
                serializer.SerializeValue(ref ResourcesAmount);
                serializer.SerializeValue(ref StateName);
                serializer.SerializeValue(ref position);
            }
            public StateSerializableData(State state)
            {
                StateID = state.StateID;
                RaceID = state.StateRaceID;
                ControllingPlayerID = state.ControllingPlayerID;
                StateName = state.StateName;
                ResourcesAmount = state.ResourcesAmount;
                position = state.position;
            }
            public void Apply(StateClientSide StateToApply)
            {
                StateToApply.StateID = StateID;
                StateToApply.RaceID = RaceID;
                StateToApply.ControllingPlayerID = ControllingPlayerID;
                StateToApply.StateName = StateName;
                StateToApply.ResourcesAmount = ResourcesAmount;
                StateToApply.transform.position = position;
            }
        }
    }
}