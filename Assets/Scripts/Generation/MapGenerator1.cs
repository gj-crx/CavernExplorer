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

        public List<Vector3Int> TilesToSet = new List<Vector3Int>();
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
                        Sector[] CurrentNeibSectors = Get4NeigbhourSectors(PlayerSector);
                        for (int i = 0; i < CurrentNeibSectors.Length; i++)
                        {
                            int GeneratedCount = 0;
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
                            if (GeneratedCount > 0)
                            {
                                for (int n = 0; n < 1; n++)
                                {
                                    CheckForUselessTiles();
                                }
                                GameManager.MapGenerator.ToGenerateOrder = true;
                            }
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
        public void SpawnAllTiles_MainThread(Tilemap ReferenceTilemap, RuleTile WallTile)
        {
            if (TilesToSet.Count > 0)
            {
                var TilesToSetArray = TilesToSet.ToArray();
                Debug.Log(TilesToSetArray.Length);
                TilesToSet = new List<Vector3Int>();
                Debug.Log(TilesToSetArray.Length);


                RuleTile[] t = new RuleTile[TilesToSetArray.Length];
                for (int i = 0; i < t.Length; i++) t[i] = WallTile;
                ReferenceTilemap.SetTiles(TilesToSetArray, t);
            }
        }
        public void CheckForUselessTiles()
        {
            foreach (var Sector in NewlyGeneratedSectors) Sector.CheckForUselessTiles(map);
        }
        
        public class Sector
        {
            public int X = 0;
            public int Y = 0;
            private Vector2Int Center;
            private byte Radius = 50;
            private Vector2Int[] SectorPoints;
            public Vector2Int[] JointPoints = new Vector2Int[4];
            public byte RadiusValue { get { return Radius; } }
            public Vector2Int RandomPoint { get { return SectorPoints[GameManager.random.Next(0, SectorPoints.Length)]; } }

            public Sector(int X, int Y, byte Radius, byte SectorPointsCount, Map MapToGenerate)
            {
                this.X = X;
                this.Y = Y;
                Center = new Vector2Int(X * (Radius * 2), Y * (Radius * 2));
                this.Radius = Radius;
                SectorPoints = new Vector2Int[SectorPointsCount + 4];
                //creating joint points
                JointPoints[0] = GetRandomJointPoint(JointPointCords.Top);
                JointPoints[1] = GetRandomJointPoint(JointPointCords.Right);
                JointPoints[2] = GetRandomJointPoint(JointPointCords.Bottom);
                JointPoints[3] = GetRandomJointPoint(JointPointCords.Left);

                if (MapToGenerate.SectorMap[X, Y + 1] != null) JointPoints[0] = MapToGenerate.SectorMap[X, Y + 1].JointPoints[2];
                if (MapToGenerate.SectorMap[X + 1, Y] != null) JointPoints[1] = MapToGenerate.SectorMap[X + 1, Y].JointPoints[3];
                if (MapToGenerate.SectorMap[X, Y - 1] != null) JointPoints[2] = MapToGenerate.SectorMap[X, Y - 1].JointPoints[0];
                if (MapToGenerate.SectorMap[X - 1, Y] != null) JointPoints[3] = MapToGenerate.SectorMap[X - 1, Y].JointPoints[1];

                Generate(MapToGenerate);
                MapToGenerate.SectorMap[X, Y] = this;
                GameManager.MapGenerator.NewlyGeneratedSectors.Push(this);
            }
            private Vector2Int GetRandomPointInSector()
            {
                return new Vector2Int(GameManager.random.Next(-Radius, Radius), GameManager.random.Next(-Radius, Radius)) + Center;
            }
            private Vector2Int GetRandomJointPoint(JointPointCords Side)
            {
                if (Side.x != 0) return new Vector2Int(Radius * Side.x, GameManager.random.Next(-Radius + 1, Radius)) + Center;
                else return new Vector2Int(GameManager.random.Next(-Radius + 1, Radius), Radius * Side.y) + Center;
            }
            private Vector2Int ConnectPoints(Vector2Int CurrentPoint, Vector2Int TargetPoint, Map ReferenceMap, List<Vector3Int> SectorTilePositions)
            {
                Vector3Int _currentPoint = BasicFunctions.ToVector3Int(CurrentPoint);
                if (SectorTilePositions.Contains(_currentPoint))
                {
                    ReferenceMap.LandscapeMap[CurrentPoint.x, CurrentPoint.y] = new LandscapePoint(LandType.Passable);
                    SectorTilePositions.Remove(_currentPoint);
                }
                do
                {
                    CurrentPoint += BasicFunctions.GetDirectionBetween2Points(CurrentPoint, TargetPoint);
                    ReferenceMap.LandscapeMap[CurrentPoint.x, CurrentPoint.y] = new LandscapePoint(LandType.Passable);
                    SectorTilePositions.Remove(BasicFunctions.ToVector3Int(CurrentPoint));
                }
                while (CurrentPoint != TargetPoint);
                return CurrentPoint;
            }
            private void CreateRoom(Vector2Int CenterOfRoom, int RoomRadius, Map ReferenceMap, List<Vector3Int> SectorTilePositions)
            {
                for (int y = -RoomRadius; y <= RoomRadius; y++)
                {
                    for (int x = -RoomRadius; x <= RoomRadius; x++)
                    {
                        ReferenceMap.LandscapeMap[CenterOfRoom.x + x, CenterOfRoom.y + y] = new LandscapePoint(LandType.Passable);
                        SectorTilePositions.Remove(new Vector3Int(CenterOfRoom.x + x, CenterOfRoom.y + y, 0));
                    }
                }
            }
            public class JointPointCords
            {
                public int x;
                public int y;
                private JointPointCords(int x, int y)
                {
                    this.x = x;
                    this.y = y;
                }
                public static JointPointCords NumberToCords(int Number)
                {
                    switch (Number)
                    {
                        case 0: return Top;
                        case 1: return Right;
                        case 2: return Bottom;
                        case 3: return Left;
                    }
                    return Top;
                }
                public static JointPointCords GetAlterSide(int Number)
                {
                    switch (Number)
                    {
                        case 0: return Bottom;
                        case 1: return Left;
                        case 2: return Top;
                        case 3: return Right;
                    }
                    return Top;
                }
                public static int GetAlterSideNumber(int Number)
                {
                    switch (Number)
                    {
                        case 0: return 2;
                        case 1: return 3;
                        case 2: return 0;
                        case 3: return 1;
                    }
                    return 0;
                }
                public static JointPointCords Top = new JointPointCords(0, 1);
                public static JointPointCords Right = new JointPointCords(1, 0);
                public static JointPointCords Bottom = new JointPointCords(0, -1);
                public static JointPointCords Left = new JointPointCords(-1, 0);
            }

            public void Generate(Map ReferenceMap)
            {
                List<Vector3Int> SectorTilePositions = new List<Vector3Int>();
                for (int y = -Radius; y <= Radius; y++)
                {
                    for (int x = -Radius; x <= Radius; x++)
                    {
                        Vector3Int CurrentTilePos = new Vector3Int(Center.x + x, Center.y + y, 0);
                        ReferenceMap.LandscapeMap[CurrentTilePos.x, CurrentTilePos.y] = new LandscapePoint(LandType.Impassable);
                        if (GameManager.MapGenerator.TilesToSet.Contains(CurrentTilePos) == false)
                        {
                            SectorTilePositions.Add(CurrentTilePos);
                        }
                    }
                }
                //assigning points
                //adding joint points to the all points in random order
                for (int i = 0; i < JointPoints.Length; i++)
                {
                    bool Found = false;
                    while (Found == false)
                    {
                        int rnd = GameManager.random.Next(0, SectorPoints.Length);
                        if (SectorPoints[rnd] == Vector2Int.zero)
                        {
                            SectorPoints[rnd] = JointPoints[i];
                            Found = true;
                        //    Debug.Log("Joint " + i + " assigned to SectorPoint " + rnd);
                        }
                    }
                }
                for (int i = 0; i < SectorPoints.Length; i++)
                {
                    if (SectorPoints[i] == Vector2Int.zero)
                    {
                        SectorPoints[i] = GetRandomPointInSector();
                    }
                }
                //connecting points in order
                Vector2Int CurrentPoint = SectorPoints[0];
                for (int i = 1; i < SectorPoints.Length; i++)
                {
                    CreateRoom(SectorPoints[i], 2, ReferenceMap, SectorTilePositions); //clearing a bit of space at each point
                    CurrentPoint = ConnectPoints(CurrentPoint, SectorPoints[i], ReferenceMap, SectorTilePositions);
                }
                foreach (var TilePosition in SectorTilePositions) GameManager.MapGenerator.TilesToSet.Add(TilePosition);
                //create positions to spawn units
                GameManager.unitSpawner.SpawnUnitsInSector(this, GameManager.random);
            }
            public void CheckForUselessTiles(Map ReferenceMap)
            {
                for (int y = -Radius; y <= Radius; y++)
                {
                    for (int x = -Radius; x <= Radius; x++)
                    {
                        Vector3Int CurrentTilePosition = new Vector3Int(Center.x + x, Center.y + y, 0);
                        if (TileIsUseless(new Vector2Int(Center.x + x, Center.y + y), ReferenceMap) && GameManager.MapGenerator.TilesToSet.Contains(CurrentTilePosition) == true)
                        {
                             Debug.Log("Tile " + CurrentTilePosition + " is added to removal queue");
                            ReferenceMap.LandscapeMap[CurrentTilePosition.x, CurrentTilePosition.y].Land = LandType.Passable;
                            GameManager.MapGenerator.TilesToSet.Remove(CurrentTilePosition);
                        }
                    }
                }
            }
            private bool TileIsUseless(Vector2Int TilePos, Map ReferenceMap)
            {
                if ((ReferenceMap.LandscapeMap[TilePos.x, TilePos.y] != null && ReferenceMap.LandscapeMap[TilePos.x, TilePos.y].Land == LandType.Impassable) 
                    && GetNeigbhourPassableTilesCount(TilePos, ReferenceMap) < 3) return true;
                else return false;
            }
            private int GetNeigbhourPassableTilesCount(Vector2Int TilePos, Map ReferenceMap)
            {
                int count = 0;
                for (int y = -1; y <= 1; y++)
                {
                    for (int x = -1; x <= 1; x++)
                    {
                        if ((ReferenceMap.LandscapeMap[TilePos.x + x, TilePos.y + y] != null && ReferenceMap.LandscapeMap[TilePos.x + x, TilePos.y + y].Land == LandType.Impassable) 
                            && (x != 0 || y != 0))
                        {
                            count++;
                        }
                    }
                }
                return count;
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
