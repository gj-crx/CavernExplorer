using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

namespace ClientSideLogic.UI
{
    public class CreateLobbyButton : MonoBehaviour
    {
        public sbyte SelectedLobbyTypeID = 0;
        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(CreateLobbyButtonExecution);
        }
        public void CreateLobbyButtonExecution()
        {
            if (NetworkManager.Singleton.IsClient || NetworkManager.Singleton.IsHost)
            {
                UIPlayerActions.LocalPlayer.PlayerLobbyActions.CreateLobbyRequestServerRpc(SelectedLobbyTypeID);
            }
            else
            {
                if (NetworkManager.Singleton.IsServer)
                {
                    ServerSideLogic.Matchmaking.GameLobby NewLobby = new ServerSideLogic.Matchmaking.GameLobby(SelectedLobbyTypeID, -2);
                }
            }
        }
    }
}
