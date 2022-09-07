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
        private RuleTile[] WallTiles;

        public Stack<Vector3Int> TilesAwaitingToBetSet = new Stack<Vector3Int>();
        public Stack<Vector3Int> TilesAwaitingToBeRemoved = new Stack<Vector3Int>();
        public Stack<Sector> NewlyGeneratedSectors = new Stack<Sector>();

        public MapGenerator1(GeneratorSettings _settings, Map _map, Tilemap _tileMap, RuleTile[] WallTiles)
        {
            GenSettings = _settings;
            this._map = _map;
            this._tileMap = _tileMap;
            this.WallTiles = WallTiles;
        }

        public void GenerateMap()
        {
            Generated = true;
            new Sector(0, 0, GenSettings.SectorRadius, GenSettings.PointsPerSector, _map);
         //   new Sector(-1, 0, GenSettings.SectorRadius, GenSettings.PointsPerSector, _map);
            foreach (var Player in GameManager.PlayerRelatedCharacters)
            {
                Player.transform.position = BasicFunctions.Vector2IntToVector3(_map.SectorMap[0, 0].RandomPoint);
            }
            
            ContiniousGeneration();
            GameSettings.Singleton.StartCoroutine(TilesPlacingCourotine(_tileMap, WallTiles[0]));
            GameSettings.Singleton.StartCoroutine(TilesRemovingCourotine(_tileMap, WallTiles[0]));
            GameSettings.Singleton.StartCoroutine(GameManager.tileFormPlacer.TilesCleaningCourotine(_map));
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
        }
        public IEnumerator TilesPlacingCourotine(Tilemap ReferenceTilemap, RuleTile WallTile)
        {
            int Counter = 0;
            int PlacedPerFrame = GetTilePlacingSpeed(TilesAwaitingToBetSet.Count);
            while (GameManager.GameIsRunning)
            {
                if (TilesAwaitingToBetSet.Count > 0)
                {
                    Vector3Int TilePosition = TilesAwaitingToBetSet.Pop();
                    // Debug.Log("iteration with total to place " + TilesBeingPlaced.Count);
                    ReferenceTilemap.SetTile(TilePosition, WallTile);
                    if (Counter > PlacedPerFrame)
                    {
                        Counter = 0;
                        yield return null;
                    }
                    else Counter++;
                }
                else yield return new WaitForSeconds(1);
            }
        }
        public IEnumerator TilesRemovingCourotine(Tilemap ReferenceTilemap, RuleTile WallTile)
        {
            int PlacedPerFrame = GetTilePlacingSpeed(TilesAwaitingToBeRemoved.Count);
            int Counter = 0;
            while (GameManager.GameIsRunning)
            {
                if (TilesAwaitingToBeRemoved.Count > 0)
                {
                    Vector3Int TilePosition = TilesAwaitingToBeRemoved.Pop();
                    // Debug.Log("iteration with total to place " + TilesBeingPlaced.Count);
                    ReferenceTilemap.SetTile(TilePosition, null);
                    if (Counter > PlacedPerFrame)
                    {
                        Counter = 0;
                        yield return null;
                    }
                    else Counter++;
                }
                else yield return new WaitForSeconds(1);
            }
        }
        private int GetTilePlacingSpeed(int ToBePlacedCount, float SecondsToPlaceAll = 4, int MinimumTilesPerFrameSpeed = 5)
        {
            // return Mathf.Max((int)(ToBePlacedCount / SecondsToPlaceAll / 60), MinimumTilesPerFrameSpeed);
            return 25;
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

            public Vector2Int RandomPoint { get { return SectorPoints[UnityEngine.Random.Range(0, SectorPoints.Length)]; } }

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

                for (int i = 0; i < JointPoints.Length; i++)
                {
                    Debug.Log(JointPoints[i] + " " + i);
                }

                Generate(MapToGenerate);
                //  PreviousSector.ReCheckSectorWalls(MapToGenerate, ReferenceTilemap);
                //   if (PreviousSector.WallsSpawned) PreviousSector.ReCheckSectorWalls(MapToGenerate, ReferenceTilemap);
                MapToGenerate.SectorMap[X, Y] = this;
            }
            private Vector2Int GetRandomPointInSector()
            {
                return new Vector2Int(UnityEngine.Random.Range(-Radius, Radius), UnityEngine.Random.Range(-Radius, Radius)) + Center;
            }
            private Vector2Int GetRandomJointPoint(JointPointCords Side)
            {
                if (Side.x != 0) return new Vector2Int(Radius * Side.x, UnityEngine.Random.Range(-Radius + 1, Radius)) + Center;
                else return new Vector2Int(UnityEngine.Random.Range(-Radius + 1, Radius), Radius * Side.y) + Center;
            }
            private void ConnectPoints(Vector2Int CurrentPoint, Vector2Int TargetPoint, Map ReferenceMap, List<Vector3Int> SectorTilePositions)
            {
                Debug.Log(TargetPoint + " started");
                int debugiteration = 0;
                while (CurrentPoint != TargetPoint)
                {
                    //   Debug.Log("step " + debugiteration + " started");
                    CurrentPoint += BasicFunctions.GetDirectionBetween2Points(CurrentPoint, TargetPoint);
                    //   Debug.Log("got direction");
                    ReferenceMap.LandscapeMap[CurrentPoint.x, CurrentPoint.y] = new LandscapePoint(LandType.Passable);
                    //   Debug.Log("new landscape pos");
                    SectorTilePositions.Remove(BasicFunctions.Vector2IntToVector3Int(CurrentPoint));
                  //  Debug.Log("removed");
                    debugiteration++;
                }
                Debug.Log(TargetPoint + " completed");
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
                        if (GameManager.MapGenerator.TilesAwaitingToBetSet.Contains(CurrentTilePos) == false)
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
                        int rnd = UnityEngine.Random.Range(0, SectorPoints.Length);
                        if (SectorPoints[rnd] == Vector2Int.zero)
                        {
                            SectorPoints[rnd] = JointPoints[i];
                            Found = true;
                            Debug.Log("Joint " + i + " assigned to SectorPoint " + rnd);
                        }
                    }
                }
                for (int i = 0; i < SectorPoints.Length; i++)
                {
                    if (SectorPoints[i] == Vector2Int.zero)
                    {
                        SectorPoints[i] = GetRandomPointInSector();
                        Debug.Log("Randomed Point " + i + " = " + SectorPoints[i]);
                    }
                    else Debug.Log("Point is a joint " + i + " = " + SectorPoints[i]);

                }
                //connecting points in order
                Vector2Int CurrentPoint = SectorPoints[0];
                Debug.Log("total points " + SectorPoints.Length);
                for (int i = 1; i < SectorPoints.Length; i++)
                {
                    while (CurrentPoint != SectorPoints[i])
                    {
                        CurrentPoint += BasicFunctions.GetDirectionBetween2Points(CurrentPoint, SectorPoints[i]);
                        ReferenceMap.LandscapeMap[CurrentPoint.x, CurrentPoint.y] = new LandscapePoint(LandType.Passable);
                        SectorTilePositions.Remove(BasicFunctions.Vector2IntToVector3Int(CurrentPoint));
                    }
                }
                foreach (var TilePosition in SectorTilePositions) GameManager.MapGenerator.TilesAwaitingToBetSet.Push(TilePosition);
            }
            public void ReCheckSectorWalls(Map ReferenceMap, Tilemap ReferenceTilemap)
            {
                Debug.Log(Thread.CurrentThread.Name);
                for (int y = -Radius; y <= Radius; y++)
                {
                    for (int x = -Radius; x <= Radius; x++)
                    {
                        Vector3Int CurrentTilePosition = new Vector3Int(Center.x + x, Center.y + y, 0);
                        if (ReferenceTilemap.GetTile(CurrentTilePosition) != null && BasicFunctions.PassableTile(Center + new Vector2Int(x, y), ReferenceMap))
                        {
                            // GameObject NewWall = GameObject.Instantiate(PrefabManager.Singleton.WallPrefabs[0], new Vector3(x, y) + BasicFunctions.Vector2IntToVector3(Center), Quaternion.identity);
                            GameManager.MapGenerator.TilesAwaitingToBeRemoved.Push(CurrentTilePosition);
                        }
                    }
                }
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
            int SectorX = (int)(MentionedUnit.transform.position.x / (SectorRadius * 2));
            int SectorY = (int)(MentionedUnit.transform.position.y / (SectorRadius * 2));
            return _map.SectorMap[SectorX, SectorY];
        }
    }
}
