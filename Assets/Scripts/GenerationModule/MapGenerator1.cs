using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Generation
{
    public class MapGenerator1
    {
        public Thread GenerationThread = null;

        public GameSettings.GeneratorSettings CurrentGenSettings;
        public int CurrentLevelToGenerate = 1;
        /// <summary>
        /// Indicates current progress of generation in UI
        /// </summary>
        public int UIGenerationProgress = 0;
        public bool GenerationCompleted = false;

        public List<Vector3Int> UnpassableToSet = new List<Vector3Int>();
        public List<Vector3Int> FloorsToSet = new List<Vector3Int>();
        public Stack<Sector> NewlyGeneratedSectors = new Stack<Sector>();

        private readonly Map map;
        private RuleTile[] unpassableTilesArrayToSet;
        private Tile[] floorTilesArrayToSet;
        private Tile[] levelGateTilesArrayToSet;
        private Vector3Int[] unpassablePositionsToSet;
        private Vector3Int[] floorPositionsToSet;
        private Vector3Int[] levelGatePositionsToSet;


        public bool ToGenerateOrder = false;


        public MapGenerator1(GameSettings.GeneratorSettings settings, Map map)
        {
            CurrentGenSettings = settings;
            this.map = map;
            GameSettings.Singleton.StartCoroutine(GameManager.unitSpawner.IterateUnitSpawningQueue());
        }

        public void GenerateMap(GameSettings.GeneratorSettings SettingsToGenerate)
        {
            CurrentGenSettings = SettingsToGenerate;
            CurrentLevelToGenerate++;
            map.LandscapeMap = new LandscapeMapHolder();
            map.SectorMap = new SectorMapHolder();
            NewlyGeneratedSectors = new Stack<Sector>();

            UIGenerationProgress = 0;
            GenerationCompleted = false;

            new Sector(0, 0, CurrentGenSettings.SectorRadius, CurrentGenSettings.PointsPerSector, map);
            if (CurrentGenSettings.ContiniousGeneration)
            {
                GenerationThread = new Thread(ContiniousGeneration);
                GenerationThread.Start();
            }
            else
            {
                GenerationThread = new Thread(FixedRoomGeneration);
                GenerationThread.Start();
            }
            GameSettings.Singleton.UnpassableTilemap.ClearAllTiles();
        }
        void ContiniousGeneration()
        {
            while (GameManager.GameIsRunning)
            {
                Sector PlayerSector = map.GetUnitSector(GameManager.LocalPlayerHeroUnit);
                if (PlayerSector != null)
                {
                    Debug.Log(PlayerSector.X * PlayerSector.RadiusValue + " " + PlayerSector.Y * PlayerSector.RadiusValue);
                    int GeneratedCount = 0;
                    Sector[] CurrentNeibSectors = Get4NeigbhourSectors(PlayerSector);
                    for (int i = 0; i < CurrentNeibSectors.Length; i++)
                    {
                        if (CurrentNeibSectors[i] != null)
                        { //generating the neibghours of selected neibghour
                            GeneratedCount += GenerataNeibghourSectors(CurrentNeibSectors[i]);
                        }
                        else
                        { //generating selected sector and his neibghours
                            var Cords = Sector.JointPointCords.NumberToCords(i);
                            new Sector(PlayerSector.X + Cords.x, PlayerSector.Y + Cords.y, CurrentGenSettings.SectorRadius, CurrentGenSettings.PointsPerSector, map);
                            GeneratedCount++;
                            GeneratedCount += GenerataNeibghourSectors(map.SectorMap[PlayerSector.X + Cords.x, PlayerSector.Y + Cords.y]);
                        }
                    }
                    Debug.Log("Generated count " + GeneratedCount);
                    if (GeneratedCount > 0)
                    {
                        Debug.Log("generated smth");
                        for (int n = 0; n < 1; n++)
                        {
                            CheckForUselessTiles();
                        }
                        FiltrateWallsList();
                        PrepareTilesToSet();
                        GameManager.MapGenerator.ToGenerateOrder = true;
                    }
                }
                Thread.Sleep(1500);
            }
        }

        void FixedRoomGeneration()
        {
            int GenRadius = CurrentGenSettings.StartingSectorsCreationRadius;
            for (int y = -GenRadius; y <= GenRadius; y++)
            {
                for (int x = -GenRadius; x <= GenRadius; x++)
                {
                    Thread.Sleep(50);
                    Sector newSector = new Sector(x, y, CurrentGenSettings.SectorRadius, CurrentGenSettings.PointsPerSector, map);

                    UIGenerationProgress++;
                }
            }
            CheckForUselessTiles();
            CreateBorderWalls();
            FiltrateWallsList();
            GenerateLevelGates();
            PrepareTilesToSet();
            GameManager.MapGenerator.ToGenerateOrder = true;
        }
        private void GenerateLevelGates()
        {
            int x = 0;
            int y = 0;
            new LevelGate(map.SectorMap[0, 0], map, true);

            for (int i = 0; i < CurrentGenSettings.DownWayGatesCount; i++)
            {
                if (GameManager.GenRandom.Next(0, 2) == 1) x = CurrentGenSettings.StartingSectorsCreationRadius;
                else x = -CurrentGenSettings.StartingSectorsCreationRadius;
                if (GameManager.GenRandom.Next(0, 2) == 1) y = CurrentGenSettings.StartingSectorsCreationRadius;
                else y = -CurrentGenSettings.StartingSectorsCreationRadius;

                new LevelGate(map.SectorMap[x, y], map, false);
            }
            for (int i = 0; i < CurrentGenSettings.UpperWayGatesCount; i++)
            {
                if (GameManager.GenRandom.Next(0, 2) == 1) x = CurrentGenSettings.StartingSectorsCreationRadius;
                else x = -CurrentGenSettings.StartingSectorsCreationRadius;
                if (GameManager.GenRandom.Next(0, 2) == 1) y = CurrentGenSettings.StartingSectorsCreationRadius;
                else y = -CurrentGenSettings.StartingSectorsCreationRadius;

                new LevelGate(map.SectorMap[x, y], map, true);
            }

            levelGatePositionsToSet = new Vector3Int[map.LevelGates.Count * 4];
            levelGateTilesArrayToSet = new Tile[map.LevelGates.Count * 4];
            int tilesCount = 0;
            foreach (var gate in map.LevelGates)
            {
                int currentTile = 0;
                foreach (Vector3Int offset in gate.neededImpassableOffsets)
                {
                    levelGatePositionsToSet[tilesCount] = gate.Position + offset;
                    if (gate.GoingUp)
                    {
                        levelGateTilesArrayToSet[tilesCount] = GameSettings.Singleton.UpLevelGateTiles[currentTile];
                    }
                    else
                    {
                        levelGateTilesArrayToSet[tilesCount] = GameSettings.Singleton.DownLevelGateTiles[currentTile];
                    }
                    currentTile++;
                    tilesCount++;
                }
            }
            UIGenerationProgress++;
        }
        private byte GenerataNeibghourSectors(Sector ReferenceSector)
        {
            byte GeneratedSectorsCount = 0;
            var Neibs = Get4NeigbhourSectors(ReferenceSector);
            for (int i = 0; i < Neibs.Length; i++)
            {
                if (Neibs[i] == null)
                {
                    var Cords = Sector.JointPointCords.NumberToCords(i);
                    new Sector(ReferenceSector.X + Cords.x, ReferenceSector.Y + Cords.y, CurrentGenSettings.SectorRadius, CurrentGenSettings.PointsPerSector, map);
                    GeneratedSectorsCount++;
                }
            }
            return GeneratedSectorsCount;
        }
        private void FiltrateWallsList()
        {
            Stack<Vector3Int> ToRemove = new Stack<Vector3Int>();
            foreach (var tile in UnpassableToSet)
            {
                if (map.LandscapeMap[tile.x, tile.y].Land != LandType.Impassable) ToRemove.Push(tile);
            }
            foreach (var tile in ToRemove) UnpassableToSet.Remove(tile);
        }
        private void PrepareTilesToSet()
        {//unity does require that
            unpassablePositionsToSet = UnpassableToSet.ToArray();
            UnpassableToSet = new List<Vector3Int>();
            unpassableTilesArrayToSet = new RuleTile[unpassablePositionsToSet.Length];
            for (int i = 0; i < unpassableTilesArrayToSet.Length; i++) unpassableTilesArrayToSet[i] = GameSettings.Singleton.WallTiles[0];

            floorPositionsToSet = FloorsToSet.ToArray();
            FloorsToSet = new List<Vector3Int>();
            floorTilesArrayToSet = new Tile[floorPositionsToSet.Length];
            for (int i = 0; i < floorTilesArrayToSet.Length; i++) floorTilesArrayToSet[i] = GameSettings.Singleton.FloorTiles[0];
        }
        public void SpawnAllTiles_MainThread(Tilemap unpassableTilemap, Tilemap passableTilemap, Tilemap levelGateTilemap)
        {
        //    System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        //    stopwatch.Start();
            unpassableTilemap.SetTiles(unpassablePositionsToSet, unpassableTilesArrayToSet);
            passableTilemap.SetTiles(floorPositionsToSet, floorTilesArrayToSet);
            levelGateTilemap.SetTiles(levelGatePositionsToSet, levelGateTilesArrayToSet);
            GenerationCompleted = true;
            if (CurrentGenSettings.ContiniousGeneration == false) GameManager.LocalPlayerHeroUnit.transform.position = BasicFunctions.ToVector3(map.SectorMap[0, 0].RandomPoint);
            //    stopwatch.Stop();
            //    Debug.Log("took time " + stopwatch.ElapsedMilliseconds);
        }
        private void CheckForUselessTiles()
        {
            foreach (var sector in NewlyGeneratedSectors) sector.CheckForUselessTiles(map);
        }
        private void CreateBorderWalls()
        {
            int GenRadius = CurrentGenSettings.StartingSectorsCreationRadius;
            foreach (var sector in NewlyGeneratedSectors)
            {
                if (sector.X == -GenRadius) sector.CreateBorderLine(-1, 0, map);
                else if (sector.X == GenRadius) sector.CreateBorderLine(1, 0, map);
                if (sector.Y == -GenRadius) sector.CreateBorderLine(0, -1, map);
                else if (sector.Y == GenRadius) sector.CreateBorderLine(0, 1, map);
            }
        }
        

        public class SectorMapHolder
        {
            Dictionary<Tuple<int, int>, Sector> SectorMapDictionary = new Dictionary<Tuple<int, int>, Sector>();


            public Sector this[int x, int y]
            {
                get
                {
                    var t = Tuple.Create(x, y);
                    if (SectorMapDictionary.ContainsKey(t)) return SectorMapDictionary[t];
                    return null;
                }
                set
                {
                    var t = Tuple.Create(x, y);
                    SectorMapDictionary[t] = value;
                }
            }
        }

        private Sector[] Get4NeigbhourSectors(Sector ReferenceSector)
        {
            Sector[] NeibSectors = new Sector[4];
            NeibSectors[0] = map.SectorMap[ReferenceSector.X, ReferenceSector.Y + 1];
            NeibSectors[1] = map.SectorMap[ReferenceSector.X + 1, ReferenceSector.Y];
            NeibSectors[2] = map.SectorMap[ReferenceSector.X, ReferenceSector.Y - 1];
            NeibSectors[3] = map.SectorMap[ReferenceSector.X - 1, ReferenceSector.Y];
            return NeibSectors;
        }

        
    }
}
