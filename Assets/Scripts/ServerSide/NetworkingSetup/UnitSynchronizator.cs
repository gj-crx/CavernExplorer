using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using ClientSideLogic;
using System.Threading.Tasks;

namespace ServerSideLogic
{
    public class UnitSynchronizator : NetworkBehaviour
    {
        public int UnitSyncInterval = 300;


        public const int UnitSyncLimit = 50;


        public async Task UnitSynchronizationProcessingAsync(Unit UnitToSync, int PreDelay)
        {
            await Task.Delay(PreDelay);
            while (UnitToSync.IsDead == false && UnitToSync.game.StillRunning)
            {
                SyncUnitClientRpc(new Unit.UnitSerializableData(UnitToSync), UnitToSync.game.SendingClientParams);
                await Task.Delay(UnitSyncInterval);
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
        public void UnitDeathClientRpc(int DiedUnitReferenceID, ClientRpcParams clientRpcParams)
        {
            Debug.Log("Unit death synced to client " + DiedUnitReferenceID);
            if (ClientGameController.Singleton.dataBase.AllUnits[DiedUnitReferenceID] != null)
            {
                Destroy(ClientGameController.Singleton.dataBase.AllUnits[DiedUnitReferenceID].gameObject);
                ClientGameController.Singleton.dataBase.RemoveUnitFromDB(DiedUnitReferenceID);
            }
        }
    }
}
