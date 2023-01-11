using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Generation
{
    public class DungeonGenerator
    {
        public List<Vector3Int> DungeonWallsPositions = new List<Vector3Int>();
        public Map referenceMap;

        public Vector2Int dungeonCenter { get; private set; }
        public System.Random genRandom { get; private set; }
        public DungeonGenerationSettings settings { get; private set; }

        int dungeonRadiusX;
        int dungeonRadiusY;


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
            Vector2Int lastDirection = Vector2Int.zero;
            int corridorsNumber = genRandom.Next(3, settings.CorridorsNumberToGenerateMax);
            DungeonCorridor[] currentGeneratedCorridors = new DungeonCorridor[corridorsNumber];
            for (int i = 0; i < corridorsNumber; i++)
            {
                lastDirection = BasicFunctions.ReverseDirection(lastDirection);
                if (genRandom.Next(0, 2) == 0) lastDirection.x *= -1;
                if (genRandom.Next(0, 2) == 0) lastDirection.y *= -1;

                currentGeneratedCorridors[i] = new DungeonCorridor(lastCorridorPosition, genRandom.Next(5, settings.CorridorMaxLength), genRandom.Next(1, settings.CorridorMaxWidth), this, lastDirection);
                lastCorridorPosition = currentGeneratedCorridors[i].CorridorLastPosition;
                lastDirection = currentGeneratedCorridors[i].direction;
            }
            //making corridors clean
            foreach (var corridor in currentGeneratedCorridors) corridor.ClearWallsInsideCorridor();
        }
        
        public void PlaceDungeonWall(Vector2Int positionToPlace)
        {
            if (PointInBorders(positionToPlace) == false) return;
            for (int y = -1; y <= 1; y++)
            {
                for (int x = -1; x <= 1; x++)
                {
                    referenceMap.LandscapeMap[positionToPlace.x + x, positionToPlace.y + y] = new LandscapePoint(LandType.Impassable);
                    DungeonWallsPositions.Add(new Vector3Int(positionToPlace.x + x, positionToPlace.y + y, 0));
                }
            }
        }
        public bool PointInBorders(Vector2Int point)
        {
            return point.x < dungeonCenter.x + dungeonRadiusX && point.x > dungeonCenter.x - dungeonRadiusX &&
                point.y < dungeonCenter.y + dungeonRadiusY && point.y > dungeonCenter.y - dungeonRadiusY;
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
            Debug.Log("walls count " + DungeonWallsPositions.Count);
            TileBase[] dungeonWallsPrefabs = new TileBase[DungeonWallsPositions.Count];
            for (int i = 0; i < dungeonWallsPrefabs.Length; i++) dungeonWallsPrefabs[i] = PrefabManager.Singleton.DungeonWallPrefabs[0];

            PrefabManager.Singleton.UnpassableTilemap.SetTiles(DungeonWallsPositions.ToArray(), dungeonWallsPrefabs);
        }
    }

    public class DungeonCorridor
    {
        public Vector2Int CorridorLastPosition { get; private set; }
        public Vector2Int direction;

        private Vector2Int corridorStartingPoint;
        private int length = 3;
        private int width = 1;

        private DungeonGenerator generator;

        public DungeonCorridor(Vector2Int corridorStartingPoint, int length, int width, DungeonGenerator generator, Vector2Int direction)
        {
            this.corridorStartingPoint = corridorStartingPoint;
            this.length = length;
            this.width = width;
            this.generator = generator;

            if (direction == Vector2Int.zero) this.direction = GetDirection(generator.genRandom.Next(0, 2) == 0);
            else this.direction = direction;

            GenerateCorridor();
            Debug.Log("corridor generated");
        }


        public void ClearWallsInsideCorridor()
        {
            for (int currentLength = 0; currentLength < length * 3; currentLength++)
            {
                for (int currentWidth = -width * 3 + 2; currentWidth < width * 3 - 1; currentWidth++)
                {
                    Vector3Int currentPosition = BasicFunctions.ToVector3Int(corridorStartingPoint + direction * currentLength) + (BasicFunctions.ReverseDirection(direction, true) * currentWidth);
                    while (generator.DungeonWallsPositions.Contains(currentPosition)) generator.DungeonWallsPositions.Remove(currentPosition);

                    //negative width check
                    currentPosition = BasicFunctions.ToVector3Int(corridorStartingPoint + direction * currentLength) - (BasicFunctions.ReverseDirection(direction, true) * currentWidth);
                    while (generator.DungeonWallsPositions.Contains(currentPosition)) generator.DungeonWallsPositions.Remove(currentPosition);
                }
            }
        }
        private Vector2Int GenerateCorridor()
        {
            CorridorLastPosition = corridorStartingPoint;
            for (int currentLength = 0; currentLength < length; currentLength++)
            {
                Vector2Int currentPoint = corridorStartingPoint + direction * currentLength * 3;
                if (generator.PointInBorders(currentPoint))
                {
                    generator.PlaceDungeonWall(currentPoint + BasicFunctions.ReverseDirection(direction) * width * 3);
                    generator.PlaceDungeonWall(currentPoint + BasicFunctions.ReverseDirection(direction) * -width * 3);
                    CorridorLastPosition = currentPoint;
                }
                else return CorridorLastPosition;
            }
            return CorridorLastPosition;
        }

        private Vector2Int GetDirection(bool directionCordX)
        {
            Vector2Int deltaWithCenter = generator.dungeonCenter - corridorStartingPoint;
            if (directionCordX)
            {
                if (deltaWithCenter.x > 0) return new Vector2Int(1, 0);
                else return new Vector2Int(-1, 0);
            }
            else
            {
                if (deltaWithCenter.y > 0) return new Vector2Int(0, 1);
                else return new Vector2Int(0, -1);
            }
        }
    }
}
