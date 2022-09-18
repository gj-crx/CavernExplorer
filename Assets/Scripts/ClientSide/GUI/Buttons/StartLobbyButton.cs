using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

namespace ClientSideLogic.UI
{
    public class StartLobbyButton : MonoBehaviour
    {
        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(StartLobbyOrder);
        }
        public void StartLobbyOrder()
        {
            UIPlayerActions.LocalPlayer.PlayerLobbyActions.StartConnectedLobbyOrderServerRpc();
        }
    }
}
