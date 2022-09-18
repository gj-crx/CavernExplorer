using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ServerSideLogic;
using ServerSideLogic.Matchmaking;


namespace ClientSideLogic
{
    namespace UI
    {
        public static class UILobbyManager
        {
            public static void VisualizeLobbiesList(GameLobby.LobbyData[] LobbiesData)
            {
                foreach (var Lobby in UIController.Singleton.list_LobbyPanels)
                {
                    if (Lobby != null)
                    {
                        GameObject.Destroy(Lobby.gameObject);
                    }
                }
                UIController.Singleton.list_LobbyPanels.Clear();

                foreach (var Lobby in LobbiesData)
                {
                    GameObject LobbyPanel = GameObject.Instantiate(UIController.Singleton.prefab_Lobby);
                    LobbyPanel.transform.SetParent(UIController.Singleton.content_LobbiesList.transform);
                    //Set players count in lobby text to recieved data
                    LobbyPanel.transform.Find("PlayersCountText").GetComponent<Text>().text =
                        UITextFormatter.CutOffNumericalPart(LobbyPanel.transform.Find("PlayersCountText").GetComponent<Text>().text) + Lobby.PlayersCount + 
                        " / " +  LobbyManager.LobbyTypes[Lobby.LobbyTypeID].MaxPossiblePlayer;
                    //Set lobby type text to recieved data
                    LobbyPanel.transform.Find("LobbyTypeText").GetComponent<Text>().text =
                        UITextFormatter.CutOffNumericalPart(LobbyPanel.transform.Find("PlayersCountText").GetComponent<Text>().text) + LobbyManager.LobbyTypes[Lobby.LobbyTypeID].TypeName;
                    UIController.Singleton.list_LobbyPanels.Add(LobbyPanel);
                    //Set ID of the UI panel
                    LobbyPanel.GetComponent<LobbyJoinButton>().LobbyID = Lobby.LobbyID;
                }
            }
        }

    }
}
