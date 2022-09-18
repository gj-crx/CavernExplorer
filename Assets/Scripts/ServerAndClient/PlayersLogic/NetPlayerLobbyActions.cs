using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using ServerSideLogic;
using ServerSideLogic.Matchmaking;

namespace ClientSideLogic
{
    [RequireComponent(typeof(Player))]
    public class NetPlayerLobbyActions : NetworkBehaviour
    {
        private Player player;



        private void Awake()
        {
            player = GetComponent<Player>();
        }
        [ServerRpc]
        public void CreateLobbyRequestServerRpc(sbyte LobbyTypeID)
        {
            new GameLobby(LobbyTypeID, (int)NetworkObject.OwnerClientId);
        }
        [ServerRpc]
        public void StartConnectedLobbyOrderServerRpc()
        {
            LobbyManager.Lobbies[player.ConnectedLobbyID].StartLobbyPlayerOrder((int)NetworkObject.OwnerClientId);
        }
        /// <summary>
        /// Requires to be connected to the server first
        /// </summary>
        [ServerRpc]
        public void TryToJoinLobbyServerRpc(short LobbyID = 0)
        {
            if (LobbyManager.Lobbies[LobbyID].JoinLobby(player.account))
            {
                Debug.Log("Player connected to lobby " + LobbyID + " as " + player.account.Nickname);
                player.ConnectedLobbyID = (sbyte)LobbyID;
            }
            else
            {
                Debug.Log("Player connection failed");
            }
        }
    }
}
