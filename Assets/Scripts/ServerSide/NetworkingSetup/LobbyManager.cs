using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ServerSideLogic.Matchmaking;
using Unity.Netcode;
using System.Threading.Tasks;

namespace ServerSideLogic
{
    public static class LobbyManager
    {
        

        public static List<PlayerAccount> ConnectedToServerPlayers = new List<PlayerAccount>();

        public static GameLobby[] Lobbies = new GameLobby[1];
        public static List<LobbyType> LobbyTypes = new List<LobbyType>();


        public static short AliveLobbiesCount
        {
            get
            {
                short count = 0;
                foreach (var Lobby in Lobbies)
                {
                    if (Lobby != null) count++;
                }
                return count;
            }
        }
        public static short StartedLobbiesCount
        {
            get
            {
                short count = 0;
                foreach (var Lobby in Lobbies)
                {
                    if (Lobby != null && Lobby.IsStarted) count++;
                }
                return count;
            }
        }
        public static void LoadLobbyTypes()
        {
            LobbyTypes.Add(new LobbyType("Normal game", 0, 12, 0, 200, ScenariosManager.StandartScenario));
            LobbyTypes.Add(new LobbyType("Normal single player", 1, 1, 1, 200, ScenariosManager.StandartScenario));
            LobbyTypes.Add(new LobbyType("Battle royale", 2, 20, 5, 500, ScenariosManager.StandartScenario));
            LobbyTypes.Add(new LobbyType("Units testing scenario", 3, 0,  12, 200, ScenariosManager.CurrentTestingScenario));
        }
        public static PlayerAccount ConnectToServer(Player playerObject, string Nickname = "Noname", string password = "123")
        {
            PlayerAccount acc = new PlayerAccount(playerObject, Nickname);
            ConnectedToServerPlayers.Add(acc);
            acc.AccountID = ConnectedToServerPlayers.IndexOf(acc);
            return acc;
        }
        public static void KillAllGames()
        {
            foreach (var Lobby in Lobbies)
            {
                if (Lobby != null && Lobby.IsStarted)
                {
                    GameObject.Destroy(Lobby.GameRunningInLobby.gameObject);
                }
            }
            Lobbies = new GameLobby[1];
        }
        public static async Task StartLobbiesSynchronizationProcess(int LobbiesSendingDelay)
        {
            while (GameNetCoordinator.Singleton.gameObject.activeInHierarchy)
            {
                GameLobby.LobbyData[] LobbiesDataArray = new GameLobby.LobbyData[AliveLobbiesCount];
                int count = 0;
                foreach (var lobby in Lobbies)
                {
                    if (lobby != null)
                    {
                        LobbiesDataArray[count] = new GameLobby.LobbyData(lobby);
                        count++;
                    }
                }
                GameNetCoordinator.Singleton.SendLobbiesClientRpc(LobbiesDataArray);

                await Task.Delay(LobbiesSendingDelay);
            }
        }
    }
    namespace Matchmaking
    {
        public class PlayerAccount
        {
            public Player PlayerIntentsObject = null;
            public int AccountID = 0;
            public string Nickname = "Noname";
            public byte SelectedRace = 0;
            public PlayerAccount(Player playerObject, string nickname, byte SelectedRace = 0)
            {
                PlayerIntentsObject = playerObject;
                AccountID = 0;
                Nickname = nickname;
                this.SelectedRace = SelectedRace;
            }
        }
        public class GameLobby
        {
            public short LobbyID = 0;
            public Game GameRunningInLobby = null;
            public List<ulong> ConnectedPlayersIDs = new List<ulong>();
            public List<PlayerAccount> Players = new List<PlayerAccount>();
            public sbyte LobbyTypeID = 0;
            public int LobbyPlayerOwnerID = -1;
            public bool IsFull
            {
                get
                {
                    if (ConnectedPlayersIDs.Count >= LobbyManager.LobbyTypes[LobbyTypeID].MaxPossiblePlayer) return true;
                    else return false;
                }
            }
            public bool IsStarted = false;

            public GameLobby(sbyte LobbyTypeID, int OwnerID)
            {
                this.LobbyTypeID = LobbyTypeID;
                LobbyPlayerOwnerID = OwnerID;
                Players = new List<PlayerAccount>();
                ConnectedPlayersIDs = new List<ulong>();

                LobbyID = AddNewLobby(this);
            }
            private static short AddNewLobby(GameLobby LobbyToAdd)
            {
                for (short i = 0; i < LobbyManager.Lobbies.Length; i++)
                {
                    if (LobbyManager.Lobbies[i] == null)
                    {
                        LobbyManager.Lobbies[i] = LobbyToAdd;
                        return i;
                    }
                }
                //cycle did not return anything yet, so that means that Lobbies array has no empty space left
                //expanding Lobbies array
                GameLobby[] OldArray = LobbyManager.Lobbies;
                LobbyManager.Lobbies = new GameLobby[OldArray.Length + 20];
                for (int i = 0; i < OldArray.Length; i++)
                {
                    LobbyManager.Lobbies[i] = OldArray[i];
                }
                LobbyManager.Lobbies[OldArray.Length] = LobbyToAdd;
                return (short)OldArray.Length;
            }

            /// <summary>
            /// return true if connection accepted
            /// </summary>
            public bool JoinLobby(PlayerAccount player)
            {
                if (Players.Count < LobbyManager.LobbyTypes[LobbyTypeID].MaxPossiblePlayer)
                {
                    Players.Add(player);
                    ConnectedPlayersIDs.Add(player.PlayerIntentsObject.OwnerClientId);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            public void RemovePlayerFromLobby(PlayerAccount player)
            {
                Players.Remove(player);
            }
            private void Start()
            {
                GameRunningInLobby = GameNetCoordinator.Singleton.CreateNewGame(LobbyID);
                LobbyManager.LobbyTypes[LobbyTypeID].GameStartingScenario.StartScenario(this);
                IsStarted = true;
                foreach (PlayerAccount player in Players)
                {
                    player.PlayerIntentsObject.UIPreparingGameStartedClientRpc(GameRunningInLobby.SendingClientParams);
                }
                Debug.Log("stopped trying to send inits to clients");
            }
            public void ForceToStartLobby()
            {
                Start();

            }
            public void StartLobbyPlayerOrder(int OrderedPlayerID)
            {
                Debug.Log("Player id " + OrderedPlayerID + " owner " + LobbyPlayerOwnerID + " started lobby");
                if (LobbyPlayerOwnerID == OrderedPlayerID)
                {
                    Start();
                }
            }
            public struct LobbyData : INetworkSerializable
            {
                public short LobbyID;
                public sbyte LobbyTypeID;
                public sbyte PlayersCount;
                public bool IsStarted;
                public LobbyData(GameLobby lobby)
                {
                    LobbyID = lobby.LobbyID;
                    LobbyTypeID = lobby.LobbyTypeID;
                    PlayersCount = (sbyte)lobby.Players.Count;
                    IsStarted = lobby.IsStarted;
                }
                public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
                {
                    serializer.SerializeValue(ref LobbyID);
                    serializer.SerializeValue(ref LobbyTypeID);
                    serializer.SerializeValue(ref PlayersCount);
                    serializer.SerializeValue(ref IsStarted);
                }
            }
        }
        public class LobbyType
        {
            public string TypeName = "Normal";
            public short ID = 0;
            public IScenario GameStartingScenario = null;
            public sbyte MaxPossiblePlayer = 12;
            public byte AIPlayersCount = 0;
            public short StartingTreesAmount = 200;
            public LobbyType(string TypeName, sbyte id, sbyte MaxPlayers, byte AIPlayersCount, short StartingTreesAmount, IScenario GameStartingScenario)
            {
                this.TypeName = TypeName;
                ID = id;
                this.GameStartingScenario = GameStartingScenario;
                MaxPossiblePlayer = MaxPlayers;
                this.AIPlayersCount = AIPlayersCount;
                this.StartingTreesAmount = StartingTreesAmount;
            }
        }
    }
}
