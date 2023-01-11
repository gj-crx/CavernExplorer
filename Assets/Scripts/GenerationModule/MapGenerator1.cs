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
        public DungeonGenerator dungeonGenerator;
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
            dungeonGenerator = new DungeonGenerator(GameManager.GenRandom, map);
            GameSettings.Singleton.StartCoroutine(GameManager.unitSpawner.IterateUnitSpawningQueue());
        }

        public void GenerateMap(int gameLevelAdvance = 1)
        {
            CurrentLevelToGenerate += gameLevelAdvance;
            CurrentGenSettings = GameSettings.Singleton.GeneratorSettingsPerLevels[CurrentLevelToGenerate];

            PrefabManager.Singleton.UnpassableTilemap.color = CurrentGenSettings.unwalkableLayerColor;
            PrefabManager.Singleton.LevelGatesTilemap.color = CurrentGenSettings.unwalkableLayerColor;
            PrefabManager.Singleton.FloorsTilemap.color = CurrentGenSettings.floorLayerColor;

            map.LandscapeMap = new LandscapeMapHolder();
            map.SectorMap = new SectorMapHolder();
            NewlyGeneratedSectors = new Stack<Sector>();

            UIGenerationProgress = 0;
            GenerationCompleted = false;

            if (CurrentGenSettings.ContiniousGeneration)
            {
                new Sector(0, 0, CurrentGenSettings.SectorRadius, CurrentGenSettings.PointsPerSector, map);
                GenerationThread = new Thread(ContiniousGeneration);
                GenerationThread.Start();
            }
            else
            {
                GenerationThread = new Thread(FixedRoomGeneration);
                GenerationThread.Start();
            }
            ClearMap();
            GenerationThread = null;
        }
        void ContiniousGeneration()
        {
            while (GameManager.GameIsRunning)
            {
                Sector PlayerSector = map.GetUnitSector(GameManager.playerControls.PlayerCharacterUnit);
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
            GenerateStructures();
            FiltrateWallsList();
            GenerateLevelGates();
            PrepareTilesToSet();
            GameManager.MapGenerator.ToGenerateOrder = true;
        }
        private void GenerateLevelGates()
        {
            int x = 0;
            int y = 0;
            new LevelGate(map.SectorMap[0, 0], map, true); //placing way up

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
                        levelGateTilesArrayToSet[tilesCount] =  PrefabManager.Singleton.UpLevelGateTiles[currentTile];
                    }
                    else
                    {
                        levelGateTilesArrayToSet[tilesCount] = PrefabManager.Singleton.DownLevelGateTiles[currentTile];
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
        private void GenerateStructures()
        {
            for (int i = 0; i < CurrentGenSettings.dungeonsToGenerate.Count; i++)
            {
                //randomizing position of structure on the edge of the generated map
                Sector borderSectorOfStructure;
                Vector2Int structureEnterPoint;
                Vector2Int dungeonCenterPoint;

                int xSectorOffset = CurrentGenSettings.StartingSectorsCreationRadius;
                int ySectorOffset = CurrentGenSettings.StartingSectorsCreationRadius;
                if (GameManager.GenRandom.Next(0, 2) == 0) xSectorOffset *= -1;
                if (GameManager.GenRandom.Next(0, 2) == 0) ySectorOffset *= -1;

                bool xSide = GameManager.GenRandom.Next(0, 2) == 0;
                if (xSide)
                {
                    xSectorOffset = GameManager.GenRandom.Next(-CurrentGenSettings.StartingSectorsCreationRadius, CurrentGenSettings.StartingSectorsCreationRadius + 1);
                    borderSectorOfStructure = map.SectorMap[xSectorOffset, ySectorOffset];

                    if (ySectorOffset > 0)
                    {
                        structureEnterPoint = borderSectorOfStructure.JointPoints[0]; //joint of border sector with dungeon is on top (relatively to sector)
                        dungeonCenterPoint = structureEnterPoint + new Vector2Int(0, CurrentGenSettings.dungeonsToGenerate[i].YRadius - 1);
                    }
                    else
                    {
                        structureEnterPoint = borderSectorOfStructure.JointPoints[2];
                        dungeonCenterPoint = structureEnterPoint + new Vector2Int(0, -CurrentGenSettings.dungeonsToGenerate[i].YRadius + 1);
                    }
                }
                else
                {
                    ySectorOffset = GameManager.GenRandom.Next(-CurrentGenSettings.StartingSectorsCreationRadius, CurrentGenSettings.StartingSectorsCreationRadius + 1);
                    borderSectorOfStructure = map.SectorMap[xSectorOffset, ySectorOffset];

                    if (xSectorOffset > 0)
                    {
                        structureEnterPoint = borderSectorOfStructure.JointPoints[1];
                        dungeonCenterPoint = structureEnterPoint + new Vector2Int(CurrentGenSettings.dungeonsToGenerate[i].XRadius - 1, 0);
                    }
                    else
                    {
                        structureEnterPoint = borderSectorOfStructure.JointPoints[3];
                        dungeonCenterPoint = structureEnterPoint + new Vector2Int(-CurrentGenSettings.dungeonsToGenerate[i].XRadius + 1, 0);
                    }
                }

                dungeonGenerator.GenerateDungeon(dungeonCenterPoint, structureEnterPoint,CurrentGenSettings.dungeonsToGenerate[i]);
            }
        }
        private void PrepareTilesToSet()
        {//unity does require that
            unpassablePositionsToSet = UnpassableToSet.ToArray();
            UnpassableToSet = new List<Vector3Int>();
            unpassableTilesArrayToSet = new RuleTile[unpassablePositionsToSet.Length];
            for (int i = 0; i < unpassableTilesArrayToSet.Length; i++) unpassableTilesArrayToSet[i] = PrefabManager.Singleton.WallTiles[0];

            floorPositionsToSet = FloorsToSet.ToArray();
            FloorsToSet = new List<Vector3Int>();
            floorTilesArrayToSet = new Tile[floorPositionsToSet.Length];
            for (int i = 0; i < floorTilesArrayToSet.Length; i++) floorTilesArrayToSet[i] = PrefabManager.Singleton.FloorTiles[0];
        }
        public void SpawnAllTiles_MainThread(Tilemap unpassableTilemap, Tilemap passableTilemap, Tilemap levelGateTilemap)
        {
        //    System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        //    stopwatch.Start();
            unpassableTilemap.SetTiles(unpassablePositionsToSet, unpassableTilesArrayToSet);
            passableTilemap.SetTiles(floorPositionsToSet, floorTilesArrayToSet);
            levelGateTilemap.SetTiles(levelGatePositionsToSet, levelGateTilesArrayToSet);
            dungeonGenerator.PlaceDungeonWalls();
            GenerationCompleted = true;
            if (CurrentGenSettings.ContiniousGeneration == false) GameManager.playerControls.transform.position = BasicFunctions.ToVector3(map.SectorMap[0, 0].RandomPoint);
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
        public void ClearMap()
        {
            PrefabManager.Singleton.UnpassableTilemap.ClearAllTiles();
            PrefabManager.Singleton.LevelGatesTilemap.ClearAllTiles();
            map.LevelGates.Clear();
            foreach (var unit in GameManager.dataBase.AllUnits)
            {
                if (unit.tag != "Player") GameObject.Destroy(unit.gameObject);
            }
            GameManager.dataBase.AllUnits.Clear();
            foreach (var corpse in GameManager.dataBase.Corpses)
            {
                if (corpse != null) GameObject.Destroy(corpse.gameObject);
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
