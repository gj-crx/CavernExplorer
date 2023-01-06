using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Generation
{
    public class DungeonGenerator
    {
        public Stack<Vector2Int> DungeonPositions = new Stack<Vector2Int>();


        Vector2Int dungeonCenter;
        int dungeonRadiusX;
        int dungeonRadiusY;

        private System.Random genRandom;
        private Map referenceMap;

        public DungeonGenerator(System.Random genRandom, Map referenceMap)
        {
            this.genRandom = genRandom;
            this.referenceMap = referenceMap;
        }

        public void GenerateDungeon(Vector2Int dungeonCenter, int dungeonRadiusX, int dungeonRadiusY, Vector2Int DungeonEntryPosition)
        {
            this.dungeonCenter = dungeonCenter;
            this.dungeonRadiusX = dungeonRadiusX;
            this.dungeonRadiusY = dungeonRadiusY;

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
                        PlaceDungeonWall(corridorStartingPosition + new Vector2Int(currentDistance, corridorWidth));
                        PlaceDungeonWall(corridorStartingPosition + new Vector2Int(currentDistance, -corridorWidth));
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
                        PlaceDungeonWall(corridorStartingPosition + new Vector2Int(-currentDistance, corridorWidth));
                        PlaceDungeonWall(corridorStartingPosition + new Vector2Int(-currentDistance, -corridorWidth));
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
                        PlaceDungeonWall(corridorStartingPosition + new Vector2Int(corridorWidth, currentDistance));
                        PlaceDungeonWall(corridorStartingPosition + new Vector2Int(-corridorWidth, currentDistance));
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
                        PlaceDungeonWall(corridorStartingPosition + new Vector2Int(corridorWidth, -currentDistance));
                        PlaceDungeonWall(corridorStartingPosition + new Vector2Int(-corridorWidth, -currentDistance));
                        if (currentDistance == distanceOfRandomPoint) randomPointOfCorridor = corridorStartingPosition + new Vector2Int(0, -currentDistance);
                    }
                }
            }
            //getting random position in corridor
            return randomPointOfCorridor;
        }

        private void PlaceDungeonWall(Vector2Int positionToPlace)
        {
            for (int y = -1; y <= 1; y++)
            {
                for (int x = -1; x <= 1; x++)
                {
                    referenceMap.LandscapeMap[positionToPlace.x + x, positionToPlace.y + y].Land = LandType.Impassable;
                }
            }
            GameObject.Instantiate(PrefabManager.Singleton.DungeonWallPrefabs[0], new Vector3(positionToPlace.x, positionToPlace.y, 0), Quaternion.identity);
        }
        private void PrepareMapForDungeon()
        {
            for (int y = -dungeonRadiusY; y <= dungeonRadiusY; y++)
            {
                for (int x = -dungeonRadiusX; x <= dungeonRadiusX; x++)
                {
                    referenceMap.LandscapeMap[dungeonCenter.x + x, dungeonCenter.y + y].Land = LandType.Passable;
                }
            }
        }
    }
}
