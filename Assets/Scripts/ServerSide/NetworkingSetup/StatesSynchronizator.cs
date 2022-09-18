using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using ClientSideLogic;
using System.Threading.Tasks;

namespace ServerSideLogic
{
    public class StatesSynchronizator : NetworkBehaviour
    {
        public int NormalDelayFor1State = 2000;
        public short CurrentGameToSync = 0;


        //TO BE CHANGED TO ASYNCRHONIUS STATE


        [ClientRpc]
        public void ApplyStateDataToClientRpc(State.StateSerializableData data, ClientRpcParams clientRpcParams)
        {
            //Debug.Log("Recieved state " + data.StateID);
            if (ClientGameController.Singleton.dataBase.States[data.StateID] != null)
            {
                data.Apply(ClientGameController.Singleton.dataBase.States[data.StateID]);
            }
            else
            { //create client side unit representation
                ClientStateController.CreateClientStateRepresentation(data);
                UIPlayerActions.LocalPlayer.AssignControlledState(data.StateID, data.ControllingPlayerID);
            }

        }
        [ClientRpc]
        public void SyncStateResourcesClientRpc(float[] Resources, short StateID, ClientRpcParams clientRpcParams)
        {
            Debug.Log("recieved resources from " + StateID + " wood " + Resources[0] + ClientGameController.Singleton.dataBase.States[StateID].StateName);
            ClientGameController.Singleton.dataBase.States[StateID].ResourcesAmount = Resources;
        }



    }
}
