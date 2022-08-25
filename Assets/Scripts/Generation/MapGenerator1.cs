using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Generation
{
    public class MapGenerator1
    {
        private GeneratorSettings GenSettings;
        private Map _map;
        private Tilemap _tileMap;
        private RuleTile[] WallTiles;


        public void GenerateMethod2()
        {
            _map.SectorMap[0, 0] = new Sector(0, 0, GenSettings.SectorRadius, GenSettings.PointsPerSector, _map);
            Generate9NeibghourSectors(_map.SectorMap[0, 0]);
            //     Generate9NeibghourSectors(_map.SectorMap[1, 1]);
            //     Generate9NeibghourSectors(_map.SectorMap[-1, -1]);
            _map.SectorMap.SpawnAllWalls(_map, _tileMap, WallTiles[0]);

            //    GameManager.tileFormPlacer.ReplaceAllTiles();
            GameManager.tileFormPlacer.DeleteUselessTiles(_map);
        }
        private void Generate9NeibghourSectors(Sector ReferenceSector)
        {
            var Neibs = GetNeigbhourSectors(ReferenceSector);
            for (int i = 0; i < Neibs.Length; i++)
            {
                if (Neibs[i] == null)
                {
                    Vector2Int NewSectorOffset = BasicFunctions.NumberToOffsetPosition(i);
                    _map.SectorMap[ReferenceSector.X + NewSectorOffset.x, ReferenceSector.Y + NewSectorOffset.y] = new Sector(ReferenceSector.X + NewSectorOffset.x, ReferenceSector.Y + NewSectorOffset.y, GenSettings.SectorRadius, GenSettings.PointsPerSector, _map, ReferenceSector.LastPoint);
                }
            }
        }


        public MapGenerator1(GeneratorSettings _settings, Map _map, Tilemap _tileMap, RuleTile[] WallTiles)
        {
            GenSettings = _settings;
            this._map = _map;
            this._tileMap = _tileMap;
            this.WallTiles = WallTiles;
        }
        [System.Serializable]
        public struct GeneratorSettings
        {
            public byte PointsPerSector;
            public byte SectorRadius;

            public byte StartingSectorsCount;
        }
        public class Sector
        {
            public int X = 0;
            public int Y = 0;
            private Vector2Int Center;
            private byte Radius = 50;
            private Vector2Int[] SectorPoints;
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
            public Sector(int X, int Y, byte Radius, byte SectorPointsCount, Map MapToGenerate, Vector2Int FirstPoint)
            {
                this.X = X;
                this.Y = Y;
                Center = new Vector2Int(X * Radius * 2, Y * Radius * 2);
                this.Radius = Radius;
                SectorPoints = new Vector2Int[SectorPointsCount];
                SectorPoints[0] = FirstPoint;

                Generate(MapToGenerate);
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
                        ReferenceMap.LandscapeMap[Center.x + x, Center.y + y] = new LandscapePoint(LandType.Impassable);
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
                    }
                }
            }
            public void SpawnSectorWalls(Map ReferenceMap, Tilemap ReferenceTilemap, RuleTile WallTile)
            {
                //Spawning walls representation
                for (int y = -Radius; y <= Radius; y++)
                {
                    for (int x = -Radius; x <= Radius; x++)
                    {
                        if (ReferenceMap.LandscapeMap[Center.x + x, Center.y + y].Land == LandType.Impassable)
                        {
                            // GameObject NewWall = GameObject.Instantiate(PrefabManager.Singleton.WallPrefabs[0], new Vector3(x, y) + BasicFunctions.Vector2IntToVector3(Center), Quaternion.identity);
                            ReferenceTilemap.SetTile(BasicFunctions.Vector2IntToVector3Int(Center + new Vector2Int(x, y)), WallTile);
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

            public void SpawnAllWalls(Map ReferenceMap, Tilemap ReferenceTilemap, RuleTile WallTile)
            {
                foreach (var _sector in SectorMapDictionary)
                {
                    _sector.Value.SpawnSectorWalls(ReferenceMap, ReferenceTilemap, WallTile);
                    GameManager.unitSpawner.SpawnUnitsInSector(_sector.Value);
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

    }
}
