using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClientSideLogic
{
    namespace UI
    {
        public class LobbyJoinButton : MonoBehaviour
        {
            public short LobbyID = 0;
            private void Awake()
            {
                GetComponent<Button>().onClick.AddListener(JoinLobbyButton);
            }
            public void JoinLobbyButton()
            {
                UIPlayerActions.LocalPlayer.PlayerLobbyActions.TryToJoinLobbyServerRpc(LobbyID);
            }
        }
    }
}
