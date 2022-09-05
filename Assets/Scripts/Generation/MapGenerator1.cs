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

        public List<Vector3Int>[] TilesAwaitingToBetSet = new List<Vector3Int>[2];
        public List<Vector3Int>[] TilesAwaitingToBeRemoved = new List<Vector3Int>[2];
        public Stack<Sector> NewlyGeneratedSectors = new Stack<Sector>();
        public byte CurrentListTilesToPlace = 0;
        public byte CurrentListTilesToRemove = 0;

        public MapGenerator1(GeneratorSettings _settings, Map _map, Tilemap _tileMap, RuleTile[] WallTiles)
        {
            GenSettings = _settings;
            this._map = _map;
            this._tileMap = _tileMap;
            this.WallTiles = WallTiles;
            TilesAwaitingToBetSet[0] = new List<Vector3Int>();
            TilesAwaitingToBetSet[1] = new List<Vector3Int>();
            TilesAwaitingToBeRemoved[0] = new List<Vector3Int>();
            TilesAwaitingToBeRemoved[1] = new List<Vector3Int>();
        }

        public void GenerateMap()
        {
            Generated = true;
            _map.SectorMap[0, 0] = new Sector(0, 0, GenSettings.SectorRadius, GenSettings.PointsPerSector, _map);
            TryGenerateUpTo9NeibghourSectors(_map.SectorMap[0, 0]);
            //     Generate9NeibghourSectors(_map.SectorMap[1, 1]);
            //     Generate9NeibghourSectors(_map.SectorMap[-1, -1]);
            //  _map.SectorMap.WallSpawningProcess(_map, _tileMap, WallTiles[0]);
            foreach (var Player in GameManager.PlayerRelatedCharacters)
            {
                Player.transform.position = BasicFunctions.Vector2IntToVector3(_map.SectorMap[0, 0].RandomPoint);
            }
            
            ContiniousGeneration();
            GameSettings.Singleton.StartCoroutine(TilesPlacingCourotine(_tileMap, WallTiles[0]));
            GameSettings.Singleton.StartCoroutine(TilesRemovingCourotine(_tileMap, WallTiles[0]));
            GameSettings.Singleton.StartCoroutine(GameManager.tileFormPlacer.TilesCleaningCourotine(_map));
        }
        public async Task ContiniousGeneration(int CheckIntervalMiliseconds = 2000)
        {
            while (GameManager.GameIsRunning)
            {
                await Task.Delay(CheckIntervalMiliseconds);
                foreach (var Player in GameManager.PlayerRelatedCharacters)
                {
                    Sector PlayerSector = GetUnitSector(Player);
                    if (PlayerSector != null)
                    {
                        foreach (var Sector in Get4NeigbhourSectors(PlayerSector))
                        {
                            if (Sector == null)
                            {
                                TryGenerateUpTo9NeibghourSectors(PlayerSector);
                                break;
                            }
                            else
                            {
                                byte GeneratedSectorsCount = TryGenerateUpTo9NeibghourSectors(Sector);
                                //if we actually generated some sectors we also need to actually make walls for them
                               // if (GeneratedSectorsCount > 0) _map.SectorMap.WallSpawningProcess(_map, _tileMap, WallTiles[0]);
                            }
                        }
                    }
                }
            }
        }
        private byte TryGenerateUpTo9NeibghourSectors(Sector ReferenceSector)
        {
            byte GeneratedSectorsCount = 0;
            var Neibs = GetNeigbhourSectors(ReferenceSector);
            for (int i = 0; i < Neibs.Length; i++)
            {
                if (Neibs[i] == null)
                {
                    Vector2Int NewSectorOffset = BasicFunctions.NumberToOffsetPosition(i);
                    _map.SectorMap[ReferenceSector.X + NewSectorOffset.x, ReferenceSector.Y + NewSectorOffset.y] = 
                        new Sector(ReferenceSector.X + NewSectorOffset.x, ReferenceSector.Y + NewSectorOffset.y, GenSettings.SectorRadius, GenSettings.PointsPerSector, _map, _tileMap, ReferenceSector);
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
            while (GameManager.GameIsRunning)
            {
                if (TilesAwaitingToBetSet[CurrentListTilesToPlace].Count > 0)
                {
                    List<Vector3Int> CurrentOperatedList = TilesAwaitingToBetSet[CurrentListTilesToPlace]; //saving the pointer to the operated list
                    CurrentListTilesToPlace = GetNextCounter(CurrentListTilesToPlace);  //redirecting adding new elements to other list
                    int PlacedPerFrame = GetTilePlacingSpeed(CurrentOperatedList.Count);
                    int Counter = 0;
                    foreach (var TilePosition in CurrentOperatedList)
                    {
                       // Debug.Log("iteration with total to place " + TilesBeingPlaced.Count);
                        ReferenceTilemap.SetTile(TilePosition, WallTile);
                        Counter++;
                        if (Counter > PlacedPerFrame)
                        {
                            Counter = 0;
                            yield return null;
                        }
                    }
                    CurrentOperatedList.Clear();
                }
                else yield return null;
            }
        }
        public IEnumerator TilesRemovingCourotine(Tilemap ReferenceTilemap, RuleTile WallTile)
        {
            while (GameManager.GameIsRunning)
            {
                if (TilesAwaitingToBeRemoved[CurrentListTilesToRemove].Count > 0)
                {
                    List<Vector3Int> CurrentOperatedList = TilesAwaitingToBeRemoved[CurrentListTilesToRemove]; //saving the pointer to the operated list
                    CurrentListTilesToRemove = GetNextCounter(CurrentListTilesToRemove); //redirecting adding new elements to other list
                    int PlacedPerFrame = GetTilePlacingSpeed(TilesAwaitingToBeRemoved[CurrentListTilesToRemove].Count);
                    int Counter = 0;
                    foreach (var TilePosition in CurrentOperatedList)
                    {
                        // Debug.Log("iteration with total to place " + TilesBeingPlaced.Count);
                        ReferenceTilemap.SetTile(TilePosition, null);
                        Counter++;
                        if (Counter > PlacedPerFrame)
                        {
                            Counter = 0;
                            yield return null;
                        }
                    }
                    CurrentOperatedList.Clear();
                }
                else yield return null;
            }
        }
        private byte GetNextCounter(byte Counter)
        {
            if (Counter == 0) return 1;
            else return 0;
        }
        private int GetTilePlacingSpeed(int ToBePlacedCount, float SecondsToPlaceAll = 4, int MinimumTilesPerFrameSpeed = 5)
        {
            return Mathf.Max((int)(ToBePlacedCount / SecondsToPlaceAll / 60), MinimumTilesPerFrameSpeed);
        }
        public class Sector
        {
            public int X = 0;
            public int Y = 0;
            private bool _wallsSpawned = false;
            private Vector2Int Center;
            private byte Radius = 50;
            private Vector2Int[] SectorPoints;

            public bool WallsSpawned { get { return _wallsSpawned; } }
            public byte RadiusValue { get { return Radius; } }
            public Vector2Int LastPoint { get { return SectorPoints[SectorPoints.Length - 1]; } }

            public Vector2Int RandomPoint { get { return SectorPoints[UnityEngine.Random.Range(0, SectorPoints.Length)]; } }


            public Sector(int X, int Y, byte Radius, byte SectorPointsCount, Map MapToGenerate)
            {
                this.X = X;
                this.Y = Y;
                Center = new Vector2Int(X * Radius * 2, Y * Radius * 2);
                this.Radius = Radius;
                SectorPoints = new Vector2Int[SectorPointsCount];

                Generate(MapToGenerate);
            }
            public Sector(int X, int Y, byte Radius, byte SectorPointsCount, Map MapToGenerate, Tilemap ReferenceTilemap, Sector PreviousSector)
            {
                this.X = X;
                this.Y = Y;
                Center = new Vector2Int(X * Radius * 2, Y * Radius * 2);
                this.Radius = Radius;
                SectorPoints = new Vector2Int[SectorPointsCount];
                SectorPoints[0] = PreviousSector.LastPoint;

                Generate(MapToGenerate);
                PreviousSector.ReCheckSectorWalls(MapToGenerate, ReferenceTilemap);
             //   if (PreviousSector.WallsSpawned) PreviousSector.ReCheckSectorWalls(MapToGenerate, ReferenceTilemap);
            }

            private Vector2Int GetRandomPointInSector()
            {
                return new Vector2Int(UnityEngine.Random.Range(-Radius, Radius), UnityEngine.Random.Range(-Radius, Radius)) + Center;
            }

            public void Generate(Map ReferenceMap)
            {
                for (int y = -Radius; y <= Radius; y++)
                {
                    for (int x = -Radius; x <= Radius; x++)
                    {
                        Vector3Int CurrentTilePos = new Vector3Int(Center.x + x, Center.y + y, 0);
                        ReferenceMap.LandscapeMap[CurrentTilePos.x, CurrentTilePos.y] = new LandscapePoint(LandType.Impassable);
                        if (GameManager.MapGenerator.TilesAwaitingToBetSet[GameManager.MapGenerator.CurrentListTilesToPlace].Contains(CurrentTilePos) == false)
                        {
                            GameManager.MapGenerator.TilesAwaitingToBetSet[GameManager.MapGenerator.CurrentListTilesToPlace].Add(CurrentTilePos);
                        }
                    }
                }
                //assigning points
                int StartingPoint = 0;
                if (SectorPoints[0] != Vector2Int.zero) StartingPoint = 1;
                for (int i = StartingPoint; i < SectorPoints.Length; i++)
                {
                    SectorPoints[i] = GetRandomPointInSector();
                }
                //connecting this points in order
                //...
                Vector2Int CurrentPoint = SectorPoints[0];
                for (int i = 0; i < SectorPoints.Length; i++)
                {
                    while (CurrentPoint != SectorPoints[i])
                    {
                        CurrentPoint += BasicFunctions.GetDirectionBetween2Points(CurrentPoint, SectorPoints[i]);
                        ReferenceMap.LandscapeMap[CurrentPoint.x, CurrentPoint.y] = new LandscapePoint(LandType.Passable);
                        GameManager.MapGenerator.TilesAwaitingToBetSet[GameManager.MapGenerator.CurrentListTilesToPlace].Remove(BasicFunctions.Vector2IntToVector3Int(CurrentPoint));
                    }
                }
            }
            public void ReCheckSectorWalls(Map ReferenceMap, Tilemap ReferenceTilemap)
            {
                for (int y = -Radius; y <= Radius; y++)
                {
                    for (int x = -Radius; x <= Radius; x++)
                    {
                        Vector3Int CurrentTilePosition = new Vector3Int(Center.x + x, Center.y + y, 0);
                        if (ReferenceTilemap.GetTile(CurrentTilePosition) != null && BasicFunctions.PassableTile(Center + new Vector2Int(x, y), ReferenceMap) &&
                            GameManager.MapGenerator.TilesAwaitingToBeRemoved[GameManager.MapGenerator.CurrentListTilesToRemove].Contains(CurrentTilePosition) == false)
                        {
                            // GameObject NewWall = GameObject.Instantiate(PrefabManager.Singleton.WallPrefabs[0], new Vector3(x, y) + BasicFunctions.Vector2IntToVector3(Center), Quaternion.identity);
                            GameManager.MapGenerator.TilesAwaitingToBeRemoved[GameManager.MapGenerator.CurrentListTilesToRemove].Add(CurrentTilePosition);
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
        private Sector[] GetNeigbhourSectors(Sector ReferenceSector)
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
            Sector[] NeibSectors = new Sector[8];
            byte Count = 0;
            for (int y = -1; y <= 1; y++)
            {
                for (int x = -1; x <= 1; x++)
                {
                    if ((x != 0 || y != 0) && (x == 0 || y == 0))
                    {
                        NeibSectors[Count] = _map.SectorMap[ReferenceSector.X + x, ReferenceSector.Y + y];
                        Count++;
                    }
                }
            }
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
