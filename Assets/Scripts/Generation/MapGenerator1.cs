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
        public bool Generated = false;

        private GeneratorSettings GenSettings;
        private readonly Map map;

        public List<Vector3Int> UnpassableToSet = new List<Vector3Int>();
        public List<Vector3Int> FloorsToSet = new List<Vector3Int>();
        public Stack<Sector> NewlyGeneratedSectors = new Stack<Sector>();

        public bool ToGenerateOrder = false;


        public MapGenerator1(GeneratorSettings _settings, Map _map, Tilemap _tileMap, RuleTile[] WallTiles)
        {
            GenSettings = _settings;
            this.map = _map;
            GameSettings.Singleton.StartCoroutine(GameManager.unitSpawner.IterateUnitSpawningQueue());
        }

        public void GenerateMap()
        {
            Generated = true;
            new Sector(0, 0, GenSettings.SectorRadius, GenSettings.PointsPerSector, map);
            foreach (var Player in GameManager.PlayerRelatedCharacters)
            {
                Player.transform.position = BasicFunctions.ToVector3(map.SectorMap[0, 0].RandomPoint);
            }
            GenerationThread = new Thread(ContiniousGeneration);
            GenerationThread.Start();
        }
        void ContiniousGeneration()
        {
            while (GameManager.GameIsRunning)
            {
                foreach (var Player in GameManager.PlayerRelatedCharacters)
                {
                    Sector PlayerSector = GetUnitSector(Player);
                    if (PlayerSector != null)
                    {
                        int GeneratedCount = 0;
                        Sector[] CurrentNeibSectors = Get4NeigbhourSectors(PlayerSector);
                        for (int i = 0; i < CurrentNeibSectors.Length; i++)
                        {
                            
                            if (CurrentNeibSectors[i] != null)
                            {
                                GeneratedCount = GenerataNeibghourSectors(CurrentNeibSectors[i]);
                            }
                            else
                            {
                                var Cords = Sector.JointPointCords.NumberToCords(i);
                                new Sector(PlayerSector.X + Cords.x, PlayerSector.Y + Cords.y, GenSettings.SectorRadius, GenSettings.PointsPerSector, map);
                                GeneratedCount = GenerataNeibghourSectors(map.SectorMap[PlayerSector.X + Cords.x, PlayerSector.Y + Cords.y]);
                            }
                        }
                        if (GeneratedCount > 0)
                        {
                            for (int n = 0; n < 1; n++)
                            {
                                CheckForUselessTiles();
                            }
                            FiltrateWallsList();
                            GameManager.MapGenerator.ToGenerateOrder = true;
                        }
                    }
                }
                Thread.Sleep(1500);
            }
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
                    new Sector(ReferenceSector.X + Cords.x, ReferenceSector.Y + Cords.y, GenSettings.SectorRadius, GenSettings.PointsPerSector, map);
                    GeneratedSectorsCount++;
                }
            }
            return GeneratedSectorsCount;
        }

        [System.Serializable]
        public struct GeneratorSettings
        {
            public byte PointsPerSector;
            public byte SectorRadius;

            public byte StartingSectorsCount;

            public int TileLayingSpeed;
        }
        public void FiltrateWallsList()
        {
            Stack<Vector3Int> ToRemove = new Stack<Vector3Int>();
            foreach (var tile in UnpassableToSet)
            {
                if (map.LandscapeMap[tile.x, tile.y].Land != LandType.Impassable) ToRemove.Push(tile);
            }
            foreach (var tile in ToRemove) UnpassableToSet.Remove(tile);
        }
        public void SpawnAllTiles_MainThread(Tilemap UnpassableTilemap, Tilemap PassableTilemap, RuleTile WallTile, RuleTile FloorTile)
        {
            Debug.Log("Tiles placing started");
            if (UnpassableToSet.Count > 0)
            {
                var TilesToSetArray = UnpassableToSet.ToArray();
                UnpassableToSet = new List<Vector3Int>();
                RuleTile[] RuleTileArray = new RuleTile[TilesToSetArray.Length];
                for (int i = 0; i < RuleTileArray.Length; i++) RuleTileArray[i] = WallTile; //unity does require that
                UnpassableTilemap.SetTiles(TilesToSetArray, RuleTileArray);
            }
            if (FloorsToSet.Count > 0)
            {
                var TilesToSetTemporalArray = FloorsToSet.ToArray();
                FloorsToSet = new List<Vector3Int>();
                RuleTile[] RuleTileArray = new RuleTile[TilesToSetTemporalArray.Length];
                for (int i = 0; i < RuleTileArray.Length; i++) RuleTileArray[i] = FloorTile;
                PassableTilemap.SetTiles(TilesToSetTemporalArray, RuleTileArray);
            }
        }
        public void CheckForUselessTiles()
        {
            foreach (var Sector in NewlyGeneratedSectors) Sector.CheckForUselessTiles(map);
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

        private Sector GetUnitSector(Unit MentionedUnit)
        {
            int SectorRadius = map.SectorMap[0, 0].RadiusValue;
            Vector3 UnitPos = BasicFunctions.GetPlayerTransformPositionFromMainthread(MentionedUnit);
            int SectorX = (int)(UnitPos.x / (SectorRadius * 2));
            int SectorY = (int)(UnitPos.y / (SectorRadius * 2));
            return map.SectorMap[SectorX, SectorY];
        }
    }
}
