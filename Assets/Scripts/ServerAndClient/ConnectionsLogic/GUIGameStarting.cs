using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using ClientSideLogic;
using ClientSideLogic.UI;
using Types;
using ServerSideLogic;
using System.Threading;
using System.Threading.Tasks;

namespace ClientSideLogic
{
    public class GUIGameStarting : NetworkBehaviour
    {
        public static GUIGameStarting Singleton;
        public bool CheckingInternetConnection = true;
        public bool IsConnected = false;


        private void Awake()
        {
            Init();
        }
        private void Start()
        {
            Application.targetFrameRate = 60; //temporal solution
            Debug.Log("Framerate limited to 60");
        }
        private void Update()
        {

        }
        private void Init()
        {
            Singleton = this;
            TypesData.LoadAllTypes(PrefabManager.Singleton.prefabs_Units, PrefabManager.Singleton.prefabs_Buildings, PrefabManager.Singleton.prefabs_Trees);
            LobbyManager.LoadLobbyTypes();
            StartInternetCheckingProcess();
            UIController.Singleton.ShowConnectionStatus(true);
        }
        public bool StartClient()
        {
            bool ResultOfConnectionToServer = NetworkManager.Singleton.StartClient();
            if (ResultOfConnectionToServer)
            {
                ClientGameController.Singleton.InitializeClient();
            }
            return ResultOfConnectionToServer;
        }
        //
        public async Task StartHostElseServer(bool StartHost = false)
        {
            GameObject.Find("GameCoordinator").GetComponent<GameNetCoordinator>().InitServer();
            if (StartHost)
            {
                NetworkManager.Singleton.StartHost();
            }
            else
            {
                NetworkManager.Singleton.StartServer();
            }
            await Task.Delay(500);
        }
        public void Button_RunSinglePlayerScenario(int ScenarioLobbyType)
        {
            if (IsLocalPlayer)
            {
                NetworkManager.Shutdown();
            }
            RunSinglePlayerScenario(ScenarioLobbyType);
            
        }
        private async Task RunSinglePlayerScenario(int ScenarioLobbyType)
        {
            await StartHostElseServer(true);
            Debug.Log("started");
            UIPlayerActions.LocalPlayer.PlayerLobbyActions.CreateLobbyRequestServerRpc((sbyte)ScenarioLobbyType);
            UIPlayerActions.LocalPlayer.PlayerLobbyActions.TryToJoinLobbyServerRpc(0);
            LobbyManager.Lobbies[0].StartLobbyPlayerOrder((int)UIPlayerActions.LocalPlayer.OwnerClientId);
        }
        private async Task StartInternetCheckingProcess(int DelayMS = 1500)
        {
            while (CheckingInternetConnection)
            {
                IsConnected = true;
                if (UIPlayerActions.LocalPlayer == null)
                {
                    UIController.Singleton.ShowConnectionStatus(false);
                    IsConnected = false;
                }
                else UIController.Singleton.ShowConnectionStatus(true);

                await Task.Delay(DelayMS);
            }
        }
    }
}