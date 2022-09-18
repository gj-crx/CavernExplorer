using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using ClientSideLogic;
using System.Threading.Tasks;

namespace ServerSideLogic
{
    public class UnitSynchronizatorLegacy : NetworkBehaviour
    {
        private float CurrentUnitSyncDelay = 0.3f;
        public float NormalDelayPer1UnitToSync = 0.5f;
        public float MinimumTimePerUnitToSync = 0.00f;
        public int SyncSpeedMultiplicator = 10;

        public const int UnitSyncLimit = 50;

        private short CurrentGameToBeSynced = 0;
        private short CurrentUnitToBeSynced = 0;

        private float timer_UnitSync = 0;
        public void UnitSynchronization()
        {
            if (timer_UnitSync > CurrentUnitSyncDelay)
            {
                if (LobbyManager.StartedLobbiesCount > 0)
                {
                    for (int i = 0; i < SyncSpeedMultiplicator; i++)
                    {
                        SyncAllUnitsByOne();
                    }
                }
            }
            else
            {
                timer_UnitSync += Time.deltaTime;
            }
        }
        public void SyncAllUnitsByOne()
        {
          //  Debug.Log(CurrentGameToBeSynced + " / " + LobbyManager.Lobbies.Length + " | " + CurrentUnitToBeSynced + " out of " + LobbyManager.Lobbies[CurrentGameToBeSynced].GameRunningInLobby.DB.AllUnits.Length);
            if (LobbyManager.Lobbies[CurrentGameToBeSynced].GameRunningInLobby.DB.AllUnits[CurrentUnitToBeSynced] != null)
            {
                timer_UnitSync = 0; //timer resets only if unit actually was synced and not just skipped
                SyncUnitClientRpc(new Unit.UnitSerializableData(LobbyManager.Lobbies[CurrentGameToBeSynced].GameRunningInLobby.DB.AllUnits[CurrentUnitToBeSynced]),
                            LobbyManager.Lobbies[CurrentGameToBeSynced].GameRunningInLobby.SendingClientParams);
            }
            CurrentUnitToBeSynced++;
            if (CurrentUnitToBeSynced >= LobbyManager.Lobbies[CurrentGameToBeSynced].GameRunningInLobby.DB.AllUnits.Length)
            {
                CurrentUnitToBeSynced = 0;
                CurrentGameToBeSynced++;
                if (CurrentGameToBeSynced >= LobbyManager.Lobbies.Length)
                {
                    CurrentGameToBeSynced = 0;
                }
                //tell clients to remove recently killed units
                foreach (int ID in LobbyManager.Lobbies[CurrentGameToBeSynced].GameRunningInLobby.unitsController.RecentlyRemovedUnitIDs)
                {
                    UnitDeathClientRpc(ID, LobbyManager.Lobbies[CurrentGameToBeSynced].GameRunningInLobby.SendingClientParams);
                }
                LobbyManager.Lobbies[CurrentGameToBeSynced].GameRunningInLobby.unitsController.RecentlyRemovedUnitIDs.Clear();
            }

        }
        [ClientRpc]
        private void SyncUnitClientRpc(Unit.UnitSerializableData Data, ClientRpcParams clientRpcParams)
        {
            // Debug.Log("Recieved unit " + Data.UnitObjectID);
            if (ClientUnitController.UnitExistInClient(Data.UnitObjectID))
            {
                UnitClientSide ClientSideUnit = ClientGameController.Singleton.dataBase.AllUnits[Data.UnitObjectID];
                if (ClientSideUnit == null)
                {
                    Debug.LogError(Data.UnitObjectID + " got error exist on client but not found (wtf)");
                }
                ClientSideUnit.ApplyRecievedData(Data);
            }
            else
            { //create client side unit representation
                ClientUnitController.CreateUnitRepresentation(Data);
            }
        }
        [ClientRpc]
        private void UnitDeathClientRpc(int DiedUnitReferenceID, ClientRpcParams clientRpcParams)
        {
            Debug.Log("Unit death synced to client " + DiedUnitReferenceID);
            if (ClientGameController.Singleton.dataBase.AllUnits[DiedUnitReferenceID] != null)
            {
                Destroy(ClientGameController.Singleton.dataBase.AllUnits[DiedUnitReferenceID].gameObject);
                ClientGameController.Singleton.dataBase.RemoveUnitFromDB(DiedUnitReferenceID);
            }
        }
        public void RecalculateUnitSyncDelay()
        {
            int AllUnitsInAllGames = 0;
            foreach (var Lobby in LobbyManager.Lobbies)
            {
                if (Lobby != null && Lobby.IsStarted)
                {
                    foreach (var unit in Lobby.GameRunningInLobby.DB.AllUnits)
                    {
                        if (unit != null) AllUnitsInAllGames++;
                    }
                }
            }
            CurrentUnitSyncDelay = Mathf.Max(NormalDelayPer1UnitToSync / AllUnitsInAllGames, MinimumTimePerUnitToSync);
        }
    }
}
