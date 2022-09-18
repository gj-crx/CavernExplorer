using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Unity.Netcode;
using ClientSideLogic;
using System.Threading.Tasks;

namespace ServerSideLogic
{
    public class GameNetCoordinator : NetworkBehaviour
    {
        /// <summary>
        /// there is only one coordinator for all lobbies at this server
        /// </summary>
        public static GameNetCoordinator Singleton;
        public UnitSynchronizator unitSynchronizator;
        public StatesSynchronizator statesSynchronizator;
        public float LobbiesSyncInterval = 3f;


        private bool IsInitialized = false;

        private void Update()
        {
            if (IsServer)
            {

            }
        }

        public void InitServer()
        {

            if (IsInitialized == false)
            {
                IsInitialized = true;
                Singleton = this;
                LobbyManager.StartLobbiesSynchronizationProcess(1000);
            }
            else
            {
                return;
            }
        }


        public Game CreateNewGame(short GameLobbyID)
        {
            Game NewGame = Game.SetupNewGame(PrefabManager.Singleton.prefab_GameLobby);
            NewGame.gameObject.name = "Game " + GameLobbyID;
            return NewGame;
        }

        [ClientRpc]
        public void SendLobbiesClientRpc(Matchmaking.GameLobby.LobbyData[] lobbies)
        {
            ClientSideLogic.UI.UILobbyManager.VisualizeLobbiesList(lobbies);
        }
        private void OnApplicationQuit()
        {
            LobbyManager.KillAllGames();
        }
    }
}
