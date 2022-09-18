using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ServerSideLogic.Matchmaking;
using Types;

namespace ServerSideLogic
{
    public static class ScenariosManager
    {
        public static IScenario StandartScenario = new BasicScenario();
        public static IScenario CurrentTestingScenario = new UnitsTestingScenario();

        private static Vector3[] GetPlayersStartingPositions(int PlayersCount, int MapSizeRadius, Vector3 MapCenter)
        {
            Vector3[] positions = new Vector3[PlayersCount];
            float AngleStep = 360 / PlayersCount;
            for (int i = 0; i < PlayersCount; i++)
            {
                float angle = AngleStep * i;
                Vector3 Direction = new Vector3(0, Mathf.Sin(Mathf.Deg2Rad * angle), Mathf.Cos(Mathf.Deg2Rad * angle));
                positions[i] = MapCenter + (Direction * MapSizeRadius) + new Vector3(0, 1, 0);
            }
            return positions;
        }
        private static void CreatePlayerBase(Game GameToCreate, Player player, Vector3 PositionToSpawn, byte RaceID)
        {
            State PlayerState = new State(GameToCreate, RaceID, PositionToSpawn, player);

        }
        private static void CreateAIBase(Game GameToCreate, Vector3 PositionToSpawn, byte RaceID)
        {
            State AIState = new State(GameToCreate, RaceID, PositionToSpawn, null, "AI state", true);
        }
        private static void SpawnStartingBases(GameLobby LobbyToStart)
        {
            byte AIsCountInLobby = LobbyManager.LobbyTypes[LobbyToStart.LobbyTypeID].AIPlayersCount;
            Debug.Log("ai players count " + AIsCountInLobby);
            Vector3[] PlayersPositions = GetPlayersStartingPositions(LobbyToStart.Players.Count + AIsCountInLobby, LobbyToStart.GameRunningInLobby.map.MapSizeX / 4, LobbyToStart.GameRunningInLobby.map.CenterOfTheMap);
            int Counter = 0;
            foreach (var Player in LobbyToStart.Players)
            {
                CreatePlayerBase(LobbyToStart.GameRunningInLobby, Player.PlayerIntentsObject, PlayersPositions[Counter], Player.SelectedRace);
                Counter++;
            }
            //Spawning AIs
            for (byte i = 0; i < AIsCountInLobby; i++)
            {
                Debug.Log("spawning ai in position " + PlayersPositions[LobbyToStart.Players.Count + i]);
                CreateAIBase(LobbyToStart.GameRunningInLobby, PlayersPositions[LobbyToStart.Players.Count + i], 0);
            }
        }

        private class BasicScenario : IScenario
        {
            public void StartScenario(GameLobby LobbyToStart)
            {
                Debug.Log("Scenario of lobby type " + LobbyToStart.LobbyTypeID + " is started");
                //Generate map
                MapGenerator.GenerateTrees(LobbyToStart.GameRunningInLobby, LobbyManager.LobbyTypes[LobbyToStart.LobbyTypeID].StartingTreesAmount);
                //Create players starting bases
                Debug.Log("starting trees spawned");
                SpawnStartingBases(LobbyToStart);
                Debug.Log("bases spawned");


            }
        }
        private class UnitsTestingScenario : IScenario
        {
            public void StartScenario(GameLobby LobbyToStart)
            {
                Unit u1 = new Unit(LobbyToStart.GameRunningInLobby, LobbyToStart.GameRunningInLobby.map.CenterOfTheMap + new Vector3(15, 1, 0), null, (byte)0);
                u1.Type.Stats.MoveSpeed = 3;
                u1.GetWayTarget(u1.position + new Vector3(15, 0, 0));
                Testing.TestingController.TestUnitsMovement(u1.position + new Vector3(15, 0, 0), u1);

            }
        }
    }
}