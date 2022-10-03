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
        public bool Generated = false;

        private GeneratorSettings GenSettings;
        private Map _map;
        private Tilemap _tileMap;
        private RuleTile[] WallRuleTiles;

        public List<Vector3Int> TilesToSet = new List<Vector3Int>();
        public Stack<Vector3Int> TilesAwaitingToBeRemoved = new Stack<Vector3Int>();
        public Stack<Sector> NewlyGeneratedSectors = new Stack<Sector>();

        public bool ToGenerateOrder = false;


        public MapGenerator1(GeneratorSettings _settings, Map _map, Tilemap _tileMap, RuleTile[] WallTiles)
        {
            GenSettings = _settings;
            this._map = _map;
            this._tileMap = _tileMap;
            this.WallRuleTiles = WallTiles;
            GameSettings.Singleton.StartCoroutine(GameManager.unitSpawner.IterateUnitSpawningQueue());
        }

        public void GenerateMap()
        {
            Generated = true;
            new Sector(0, 0, GenSettings.SectorRadius, GenSettings.PointsPerSector, _map);
         //   new Sector(-1, 0, GenSettings.SectorRadius, GenSettings.PointsPerSector, _map);
            foreach (var Player in GameManager.PlayerRelatedCharacters)
            {
                Player.transform.position = BasicFunctions.ToVector3(_map.SectorMap[0, 0].RandomPoint);
            }
            Thread ContiniousGenerationThread = new Thread(ContiniousGeneration);
            ContiniousGenerationThread.Start();
          //  ContiniousGeneration();
          //  GameSettings.Singleton.StartCoroutine(TilesPlacingCourotine(_tileMap, WallRuleTiles[0]));
            // GameSettings.Singleton.StartCoroutine(GameManager.tileFormPlacer.TilesCleaningCourotine(_map));
            Thread UselessTilesDetectionThread = new Thread(ContiniousDetectionOfUselessTiles);
            UselessTilesDetectionThread.Start();
          //  DeletingUselessTilesProcessAsync();
        }
        public async Task ContiniousGeneration(int CheckIntervalMiliseconds = 1500, int InitialAwait = 250)
        {
            await Task.Delay(InitialAwait);
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
                         //   Debug.Log("negovno " + CurrentNeibSectors[i].X);
                            if (CurrentNeibSectors[i] != null)
                            {
                                GeneratingNeigbhourSectors(CurrentNeibSectors[i]);
                            }
                            else
                            {
                                var Cords = Sector.JointPointCords.NumberToCords(i);
                                new Sector(PlayerSector.X + Cords.x, PlayerSector.Y + Cords.y, GenSettings.SectorRadius, GenSettings.PointsPerSector, _map);
                                GeneratingNeigbhourSectors(_map.SectorMap[PlayerSector.X + Cords.x, PlayerSector.Y + Cords.y]);
                            }
                        }
                    }
                }
                await Task.Delay(CheckIntervalMiliseconds);
            }
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
                            //   Debug.Log("negovno " + CurrentNeibSectors[i].X);
                            if (CurrentNeibSectors[i] != null)
                            {
                                int GeneratedCount = GeneratingNeigbhourSectors(CurrentNeibSectors[i]);
                                if (GeneratedCount > 0) GameManager.MapGenerator.ToGenerateOrder = true;
                            }
                            else
                            {
                                var Cords = Sector.JointPointCords.NumberToCords(i);
                                new Sector(PlayerSector.X + Cords.x, PlayerSector.Y + Cords.y, GenSettings.SectorRadius, GenSettings.PointsPerSector, _map);
                                int GeneratedCount = GeneratingNeigbhourSectors(_map.SectorMap[PlayerSector.X + Cords.x, PlayerSector.Y + Cords.y]);
                                if (GeneratedCount > 0) GameManager.MapGenerator.ToGenerateOrder = true;
                            }
                        }
                    }
                }
                Thread.Sleep(3000);
            }
        }
        private byte GeneratingNeigbhourSectors(Sector ReferenceSector)
        {
            byte GeneratedSectorsCount = 0;
            var Neibs = Get4NeigbhourSectors(ReferenceSector);
            for (int i = 0; i < Neibs.Length; i++)
            {
                if (Neibs[i] == null)
                {
                    var Cords = Sector.JointPointCords.NumberToCords(i);
                    new Sector(ReferenceSector.X + Cords.x, ReferenceSector.Y + Cords.y, GenSettings.SectorRadius, GenSettings.PointsPerSector, _map);
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
        public void TilesPlacingCourotine(Tilemap ReferenceTilemap, RuleTile WallTile)
        {
            if (TilesToSet.Count > 0)
            {
                Debug.Log("123");
                RuleTile[] t = new RuleTile[TilesToSet.Count];
                for (int i = 0; i < t.Length; i++) t[i] = WallTile;
                ReferenceTilemap.SetTiles(TilesToSet.ToArray(), t);
                Debug.Log("234");
            }
        }
        public IEnumerator TilesRemovingCourotine(Tilemap ReferenceTilemap)
        {
            int PlacedPerFrame = 3;
            int Counter = 0;
            while (TilesAwaitingToBeRemoved.Count > 0)
            {
                Vector3Int TilePosition = TilesAwaitingToBeRemoved.Pop();
                ReferenceTilemap.SetTile(TilePosition, null);
                if (Counter > PlacedPerFrame)
                {
                    Counter = 0;
                    yield return null;
                }
                else Counter++;
            }
            yield return null;
        }
        void ContiniousDetectionOfUselessTiles()
        {
            Thread.Sleep(4000);
            while (GameManager.GameIsRunning)
            {
                Debug.LogWarning("Iteration");
                Thread.Sleep(2500);
                //creating the copy of newly generated sectors stack to prevent collection modified exception
                Stack<Sector> CopyOfNewlyGeneratedSectors = new Stack<Sector>();
                foreach (var Sector in NewlyGeneratedSectors) CopyOfNewlyGeneratedSectors.Push(Sector);
                //checking sectors safely now
                foreach (var Sector in CopyOfNewlyGeneratedSectors)
                {
                    Sector.CheckForUselessTiles(_map);
                    Thread.Sleep(400);
                }
            }
        }
        public async Task DeletingUselessTilesProcessAsync()
        {
            await Task.Delay(4500);
            while (GameManager.GameIsRunning)
            {
                Debug.LogWarning("Iteration");
                await Task.Delay(2500);
                foreach (var Sector in NewlyGeneratedSectors)
                {
                    Sector.CheckForUselessTiles(_map);
                    await Task.Delay(400);
                }
            }
        }
        private int GetTilePlacingSpeed(int ToBePlacedCount, float SecondsToPlaceAll = 4, int MinimumTilesPerFrameSpeed = 5)
        {
            // return Mathf.Max((int)(ToBePlacedCount / SecondsToPlaceAll / 60), MinimumTilesPerFrameSpeed);
            return GameSettings.Singleton.MapGeneratorSettings.TileLayingSpeed;
        }
        
        public class Sector
        {
            public int X = 0;
            public int Y = 0;
            private bool _wallsSpawned = false;
            private Vector2Int Center;
            private byte Radius = 50;
            private Vector2Int[] SectorPoints;
            public Vector2Int[] JointPoints = new Vector2Int[4];

            public bool WallsSpawned { get { return _wallsSpawned; } }
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

              //  JointPoints[JointPointCords.GetAlterSideNumber(OriginSideID)] = PreviousSector.JointPoints[OriginSideID];
                if (MapToGenerate.SectorMap[X, Y + 1] != null) JointPoints[0] = MapToGenerate.SectorMap[X, Y + 1].JointPoints[2];
                if (MapToGenerate.SectorMap[X + 1, Y] != null) JointPoints[1] = MapToGenerate.SectorMap[X + 1, Y].JointPoints[3];
                if (MapToGenerate.SectorMap[X, Y - 1] != null) JointPoints[2] = MapToGenerate.SectorMap[X, Y - 1].JointPoints[0];
                if (MapToGenerate.SectorMap[X - 1, Y] != null) JointPoints[3] = MapToGenerate.SectorMap[X - 1, Y].JointPoints[1];
                Generate(MapToGenerate);
                //  PreviousSector.ReCheckSectorWalls(MapToGenerate, ReferenceTilemap);
                //   if (PreviousSector.WallsSpawned) PreviousSector.ReCheckSectorWalls(MapToGenerate, ReferenceTilemap);
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
                   //     Debug.Log("Randomed Point " + i + " = " + SectorPoints[i]);
                    }
               //     else Debug.Log("Point is a joint " + i + " = " + SectorPoints[i]);

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
            //    Debug.Log("Sector " + X + " " + Y + " is checked");
                for (int y = -Radius; y <= Radius; y++)
                {
                    for (int x = -Radius; x <= Radius; x++)
                    {
                        Vector3Int CurrentTilePosition = new Vector3Int(Center.x + x, Center.y + y, 0);
                        if (TileIsUseless(new Vector2Int(Center.x + x, Center.y + y), ReferenceMap) && GameManager.MapGenerator.TilesAwaitingToBeRemoved.Contains(CurrentTilePosition) == false)
                        {
                          //  Debug.Log("Tile " + CurrentTilePosition + " is added to removal queue");
                            GameManager.MapGenerator.TilesAwaitingToBeRemoved.Push(CurrentTilePosition);
                            ReferenceMap.LandscapeMap[CurrentTilePosition.x, CurrentTilePosition.y].Land = LandType.Passable;
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
        private Sector[] Get8NeigbhourSectors(Sector ReferenceSector)
        {
            Sector[] NeibSectors = new Sector[8];
            byte Count = 0;
            for (int y = -1; y <= 1; y++)
            {
                for (int x = -1; x <= 1; x++) 
                {
                    if (x != 0 || y != 0)
                    {
                        NeibSectors[Count] = _map.SectorMap[ReferenceSector.X + x, ReferenceSector.Y + y];
                        Count++;
                    }
                }
            }
            return NeibSectors;
        }
        private Sector[] Get4NeigbhourSectors(Sector ReferenceSector)
        {
            Sector[] NeibSectors = new Sector[4];
            NeibSectors[0] = _map.SectorMap[ReferenceSector.X, ReferenceSector.Y + 1];
            NeibSectors[1] = _map.SectorMap[ReferenceSector.X + 1, ReferenceSector.Y];
            NeibSectors[2] = _map.SectorMap[ReferenceSector.X, ReferenceSector.Y - 1];
            NeibSectors[3] = _map.SectorMap[ReferenceSector.X - 1, ReferenceSector.Y];
            return NeibSectors;
        }

        private Sector GetUnitSector(Unit MentionedUnit)
        {
            int SectorRadius = _map.SectorMap[0, 0].RadiusValue;
            Vector3 UnitPos = BasicFunctions.GetPlayerTransformPositionFromMainthread(MentionedUnit);
            int SectorX = (int)(UnitPos.x / (SectorRadius * 2));
            int SectorY = (int)(UnitPos.y / (SectorRadius * 2));
            return _map.SectorMap[SectorX, SectorY];
        }
    }
}
