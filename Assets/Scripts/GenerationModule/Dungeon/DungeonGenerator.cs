using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Generation
{
    public class DungeonGenerator
    {
        public Stack<Vector3Int> DungeonWallsPositions = new Stack<Vector3Int>();


        Vector2Int dungeonCenter;
        DungeonGenerationSettings settings;
        int dungeonRadiusX;
        int dungeonRadiusY;

        private System.Random genRandom;
        private Map referenceMap;

        public DungeonGenerator(System.Random genRandom, Map referenceMap)
        {
            this.genRandom = genRandom;
            this.referenceMap = referenceMap;
            Debug.Log(this.referenceMap);
        }

        public void GenerateDungeon(Vector2Int dungeonCenter, Vector2Int DungeonEntryPosition, DungeonGenerationSettings settings)
        {
            this.settings = settings;
            this.dungeonCenter = dungeonCenter;
            this.dungeonRadiusX = settings.XRadius;
            this.dungeonRadiusY = settings.YRadius;

            PrepareMapForDungeon();

            Vector2Int lastCorridorPosition = DungeonEntryPosition;
            int corridorsNumber = genRandom.Next(3, 8);
            for (int i = 0; i < corridorsNumber; i++)
            {
                lastCorridorPosition = GenerateDungeonCorridor(genRandom.Next(1, 4), lastCorridorPosition, genRandom.Next(0, 2) == 0);
            }

        }
        /// <summary>
        /// returns random point in corridor
        /// </summary>
        private Vector2Int GenerateDungeonCorridor(int corridorWidth, Vector2Int corridorStartingPosition, bool directionCordX)
        {
            int corridorLength;
            bool directionPositive;
            Vector2Int randomPointOfCorridor = Vector2Int.zero;
            //direction depends on delta of corridor position and dungeon center
            if (directionCordX) directionPositive = corridorStartingPosition.x < dungeonCenter.x;
            else directionPositive = corridorStartingPosition.y < dungeonCenter.y;

            if (directionCordX)
            {
                if (directionPositive)
                {
                    corridorLength = Mathf.Min(genRandom.Next(4, 10), dungeonCenter.x + dungeonRadiusX - corridorStartingPosition.x);
                    int distanceOfRandomPoint = genRandom.Next(0, corridorLength);
                    //placing walls across all X length
                    for (int currentDistance = 0; currentDistance < corridorLength; currentDistance++)
                    {
                        PlaceDungeonWall(corridorStartingPosition + new Vector2Int(currentDistance * 3, corridorWidth * 3));
                        PlaceDungeonWall(corridorStartingPosition + new Vector2Int(currentDistance * 3, -corridorWidth * 3));
                        referenceMap.LandscapeMap[corridorStartingPosition.x + currentDistance * 3, corridorStartingPosition.y] = new LandscapePoint(LandType.Passable); //to detect already existing corridors
                        if (currentDistance == distanceOfRandomPoint) randomPointOfCorridor = corridorStartingPosition + new Vector2Int(currentDistance, 0);
                    }
                }
                else
                {
                    corridorLength = Mathf.Min(genRandom.Next(4, 10), corridorStartingPosition.x - (dungeonCenter.x - dungeonRadiusX));
                    int distanceOfRandomPoint = genRandom.Next(0, corridorLength);
                    //placing walls across all NEGATIVE X length
                    for (int currentDistance = 0; currentDistance < corridorLength; currentDistance++)
                    {
                        PlaceDungeonWall(corridorStartingPosition + new Vector2Int(-currentDistance * 3, corridorWidth * 3));
                        PlaceDungeonWall(corridorStartingPosition + new Vector2Int(-currentDistance * 3, -corridorWidth * 3));
                        referenceMap.LandscapeMap[corridorStartingPosition.x - currentDistance * 3, corridorStartingPosition.y] = new LandscapePoint(LandType.Passable);
                        if (currentDistance == distanceOfRandomPoint) randomPointOfCorridor = corridorStartingPosition + new Vector2Int(-currentDistance, 0);
                    }
                }
            }

            if (directionCordX == false) //all same but with Y cord
            {
                if (directionPositive)
                {
                    corridorLength = Mathf.Min(genRandom.Next(4, 10), dungeonCenter.y + dungeonRadiusY - corridorStartingPosition.y);
                    int distanceOfRandomPoint = genRandom.Next(0, corridorLength);
                    //placing walls across all Y length
                    for (int currentDistance = 0; currentDistance < corridorLength; currentDistance++)
                    {
                        PlaceDungeonWall(corridorStartingPosition + new Vector2Int(corridorWidth * 3, currentDistance * 3));
                        PlaceDungeonWall(corridorStartingPosition + new Vector2Int(-corridorWidth * 3, currentDistance * 3));
                        referenceMap.LandscapeMap[corridorStartingPosition.x, corridorStartingPosition.y + currentDistance * 3] = new LandscapePoint(LandType.Passable);
                        if (currentDistance == distanceOfRandomPoint) randomPointOfCorridor = corridorStartingPosition + new Vector2Int(0, currentDistance);
                }
                }
                else
                {
                    corridorLength = Mathf.Min(genRandom.Next(4, 10), corridorStartingPosition.y - (dungeonCenter.y - dungeonRadiusY));
                    int distanceOfRandomPoint = genRandom.Next(0, corridorLength);
                    //placing walls across all NEGATIVE Y length
                    for (int currentDistance = 0; currentDistance < corridorLength; currentDistance++)
                    {
                        PlaceDungeonWall(corridorStartingPosition + new Vector2Int(corridorWidth * 3, -currentDistance * 3));
                        PlaceDungeonWall(corridorStartingPosition + new Vector2Int(-corridorWidth * 3, -currentDistance * 3));
                        referenceMap.LandscapeMap[corridorStartingPosition.x, corridorStartingPosition.y - currentDistance * 3] = new LandscapePoint(LandType.Passable);
                        if (currentDistance == distanceOfRandomPoint) randomPointOfCorridor = corridorStartingPosition + new Vector2Int(0, -currentDistance);
                    }
                }
            }
            //getting random position in corridor
            return randomPointOfCorridor;
        }

        private void PlaceDungeonWall(Vector2Int positionToPlace)
        {
            DungeonWallsPositions.Push(new Vector3Int(positionToPlace.x, positionToPlace.y, 0));
            for (int y = -1; y <= 1; y++)
            {
                for (int x = -1; x <= 1; x++)
                {
                    referenceMap.LandscapeMap[positionToPlace.x + x, positionToPlace.y + y] = new LandscapePoint(LandType.Impassable);
                }
            }
        }
        private void PrepareMapForDungeon()
        {
            for (int y = -dungeonRadiusY; y <= dungeonRadiusY; y++)
            {
                for (int x = -dungeonRadiusX; x <= dungeonRadiusX; x++)
                {
                    referenceMap.LandscapeMap[dungeonCenter.x + x, dungeonCenter.y + y] = new LandscapePoint(LandType.Passable);
                    GameManager.MapGenerator.FloorsToSet.Add(new Vector3Int(dungeonCenter.x + x, dungeonCenter.y + y, 0));
                }
            }
        }
        public void PlaceDungeonWalls()
        {
            foreach (var dungeonWallPosition in DungeonWallsPositions)
            {
                GameObject.Instantiate(PrefabManager.Singleton.DungeonWallPrefabs[0], dungeonWallPosition, Quaternion.identity);
            }
        }
    }

    public class DungeonCorridor
    {


        private int length = 3;
        private int width = 1;
        public void ClearWallsInsideCorridor()
        {
            for (int currentLength = 0; currentLength < length; currentLength++)
            {

            }
        }
        public GameObject GetWall(Vector2Int wallPosition)
        {

        }
    }
}
