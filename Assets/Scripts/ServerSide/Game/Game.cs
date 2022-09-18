using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Types;
using Unity.Netcode;
using ServerSideLogic.InternalMechanics;

namespace ServerSideLogic
{
    public class Game : MonoBehaviour
    {
        public sbyte GameID = 0;
        public Map map;
        public UnitsController unitsController;
        public StatesController statesController;
        public IPathfinding pf;
        public Database DB;

        public ClientRpcParams SendingClientParams;
        public float TimeMSConstante = 0.1f;


        [Header("Testing variables")]
        public List<Vector2Int> TestWay = new List<Vector2Int>();

        public bool StillRunning { get; private set; } = true;
        public bool DebugMode = true;


        private void Awake()
        {

        }
        public static Game SetupNewGame(GameObject prefab)
        {
            Game NewGame = Instantiate(prefab).GetComponent<Game>();
            NewGame.SendingClientParams = new ClientRpcParams();
            NewGame.SendingClientParams.Send = new ClientRpcSendParams();
            NewGame.SendingClientParams.Send.TargetClientIds = LobbyManager.Lobbies[NewGame.GameID].ConnectedPlayersIDs;

            NewGame.InitializeGame();
            return NewGame;
        }
        private void InitializeGame()
        {
            Debug.Log("New game started");
            map = new Map();
            pf = new AStarPathfinding(map);
            DB = new Database();
            unitsController = new UnitsController(DB);
            statesController = new StatesController(DB);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                DB.AllUnits[0].position += new Vector3(1, 0, 0);
            }
            if (Input.GetKeyDown(KeyCode.I))
            {
                TestWay = pf.GetLastWay(); ;
            }
        }
        private void OnDestroy()
        {
            StillRunning = false;
        }
    }
}
