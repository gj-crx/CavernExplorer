using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Generation
{
    public class Sector
    {
        public int X = 0;
        public int Y = 0;
        private Vector2Int Center;
        private byte Radius = 50;
        private Vector2Int[] SectorPoints;
        public Vector2Int[] JointPoints = new Vector2Int[4];
        public byte RadiusValue { get { return Radius; } }
        public Vector2Int RandomPoint { get { return SectorPoints[GameManager.GenRandom.Next(0, SectorPoints.Length)]; } }

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
            return new Vector2Int(GameManager.GenRandom.Next(-Radius, Radius), GameManager.GenRandom.Next(-Radius, Radius)) + Center;
        }
        private Vector2Int GetRandomJointPoint(JointPointCords Side)
        {
            if (Side.x != 0) return new Vector2Int(Radius * Side.x, GameManager.GenRandom.Next(-Radius + 1, Radius)) + Center;
            else return new Vector2Int(GameManager.GenRandom.Next(-Radius + 1, Radius), Radius * Side.y) + Center;
        }
        private Vector2Int ConnectPoints(Vector2Int CurrentPoint, Vector2Int TargetPoint, Map ReferenceMap)
        {
            ReferenceMap.LandscapeMap[CurrentPoint.x, CurrentPoint.y] = new LandscapePoint(LandType.Passable);
            do
            {
                CurrentPoint += BasicFunctions.GetDirectionBetween2Points(CurrentPoint, TargetPoint);
                ReferenceMap.LandscapeMap[CurrentPoint.x, CurrentPoint.y] = new LandscapePoint(LandType.Passable);
            }
            while (CurrentPoint != TargetPoint);
            return CurrentPoint;
        }
        private void CreateRoom(Vector2Int CenterOfRoom, int RoomRadius, Map ReferenceMap)
        {
            for (int y = -RoomRadius; y <= RoomRadius; y++)
            {
                for (int x = -RoomRadius; x <= RoomRadius; x++)
                {
                    ReferenceMap.LandscapeMap[CenterOfRoom.x + x, CenterOfRoom.y + y] = new LandscapePoint(LandType.Passable);
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
            int n = 0;
            for (int y = -Radius + n; y <= Radius - n; y++)
            {
                for (int x = -Radius + n; x <= Radius - n; x++)
                {
                    Vector3Int CurrentTilePos = new Vector3Int(Center.x + x, Center.y + y, 0);
                    ReferenceMap.LandscapeMap[CurrentTilePos.x, CurrentTilePos.y] = new LandscapePoint(LandType.Impassable);
                }
            }
            //assigning points
            //adding joint points to the all points in random order
            for (int i = 0; i < JointPoints.Length; i++)
            {
                bool Found = false;
                while (Found == false)
                {
                    int rnd = GameManager.GenRandom.Next(0, SectorPoints.Length);
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
                CreateRoom(SectorPoints[i], 2, ReferenceMap); //clearing a bit of space at each point
                CurrentPoint = ConnectPoints(CurrentPoint, SectorPoints[i], ReferenceMap);
            }
            QueueTilesToSet(ReferenceMap);
            //create positions to spawn units
            GameManager.unitSpawner.SpawnUnitsInSector(this, GameManager.GenRandom);
        }
        public void CheckForUselessTiles(Map ReferenceMap)
        {
            for (int y = -Radius; y <= Radius; y++)
            {
                for (int x = -Radius; x <= Radius; x++)
                {
                    Vector3Int CurrentTilePosition = new Vector3Int(Center.x + x, Center.y + y, 0);
                    if (TileIsUseless(new Vector2Int(Center.x + x, Center.y + y), ReferenceMap) && GameManager.MapGenerator.UnpassableToSet.Contains(CurrentTilePosition) == true)
                    {
                        //Debug.Log("Tile " + CurrentTilePosition + " is added to removal queue");
                        ReferenceMap.LandscapeMap[CurrentTilePosition.x, CurrentTilePosition.y].Land = LandType.Passable;
                        GameManager.MapGenerator.UnpassableToSet.Remove(CurrentTilePosition);

                        //and now we also checking neib tiles to removed one
                        for (int Y = -1; Y <= 1; Y++)
                            for (int X = -1; X <= 1; X++)
                            {
                                Vector3Int NeibTilePosition = CurrentTilePosition + new Vector3Int(X, Y, 0);
                                if ((X != 0 || Y != 0) && (X == 0 || Y == 0)) //checking only 4 directly connected tiles
                                    if (GameManager.MapGenerator.UnpassableToSet.Contains(NeibTilePosition) == true && TileIsUseless(BasicFunctions.ToVector2Int(NeibTilePosition), ReferenceMap))
                                    {
                                        ReferenceMap.LandscapeMap[NeibTilePosition.x, NeibTilePosition.y].Land = LandType.Passable;
                                        GameManager.MapGenerator.UnpassableToSet.Remove(NeibTilePosition);
                                    }
                            }
                    }
                }
            }
        }
        public void ManuallyRemoveUselessTiles(Map ReferenceMap, bool PhysicallyRemove)
        {
            for (int y = -Radius; y <= Radius; y++)
            {
                for (int x = -Radius; x <= Radius; x++)
                {
                    var currentTile = GameSettings.Singleton.unpassableTilemap.GetTile(new Vector3Int(Center.x + x, Center.y + y, 0));

                    if (currentTile != null)
                    {
                        Vector2Int CurrentTilePosition = new Vector2Int(Center.x + x, Center.y + y);
                        if (ReferenceMap.LandscapeMap[CurrentTilePosition.x, CurrentTilePosition.y].Land != LandType.Impassable)
                            GameSettings.Singleton.unpassableTilemap.SetTile(BasicFunctions.ToVector3Int(CurrentTilePosition), null);
                        Debug.Log("Manually removed");
                    }
                }
            }
        }

        private bool TileIsUseless(Vector2Int TilePos, Map ReferenceMap)
        {
            if (ReferenceMap.LandscapeMap[TilePos.x, TilePos.y] != null && ReferenceMap.LandscapeMap[TilePos.x, TilePos.y].Land == LandType.Impassable
                && TileHasConnections(TilePos, ReferenceMap) == false) return true;
            else return false;
        }
        private bool TileHasConnections(Vector2Int TilePos, Map ReferenceMap)
        {
            if (ReferenceMap.LandscapeMap[TilePos.x + 1, TilePos.y] == null || ReferenceMap.LandscapeMap[TilePos.x + 1, TilePos.y].Land != LandType.Impassable)
            {
                if (ReferenceMap.LandscapeMap[TilePos.x - 1, TilePos.y] == null || ReferenceMap.LandscapeMap[TilePos.x - 1, TilePos.y].Land != LandType.Impassable)
                {
                    return false;
                }
            }

            if (ReferenceMap.LandscapeMap[TilePos.x, TilePos.y + 1] == null || ReferenceMap.LandscapeMap[TilePos.x, TilePos.y + 1].Land != LandType.Impassable)
            {
                if (ReferenceMap.LandscapeMap[TilePos.x, TilePos.y - 1] == null || ReferenceMap.LandscapeMap[TilePos.x, TilePos.y - 1].Land != LandType.Impassable)
                {
                    return false;
                }
            }


            return true;
        }
        private void QueueTilesToSet(Map ReferenceMap)
        {
            for (int y = -Radius; y <= Radius; y++)
            {
                for (int x = -Radius; x <= Radius; x++)
                {
                    Vector3Int CurrentTilePos = new Vector3Int(Center.x + x, Center.y + y, 0);
                    GameManager.MapGenerator.FloorsToSet.Add(CurrentTilePos);
                    if (ReferenceMap.LandscapeMap[CurrentTilePos.x, CurrentTilePos.y].Land == LandType.Impassable && GameManager.MapGenerator.UnpassableToSet.Contains(CurrentTilePos) == false)
                    {
                        GameManager.MapGenerator.UnpassableToSet.Add(CurrentTilePos);
                    }
                }
            }
        }

    }
}
