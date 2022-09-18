using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClientSideLogic.UI
{
    public class Button_StartMultiplayer : MonoBehaviour
    {
        void Start()
        {
            GetComponent<Button>().onClick.AddListener(StartMultiplayerButtonClicked);
        }
        public void StartMultiplayerButtonClicked()
        {
            if (GUIGameStarting.Singleton.StartClient())
            {
                UIController.Singleton.panel_LobbiesMenu.SetActive(true);
            }
            else
            {
                UIController.Singleton.CreateAlert("Failed to run a multiplayer. Check if you have the latest game updates or the server have some issues.");
            }
        }
    }
}
