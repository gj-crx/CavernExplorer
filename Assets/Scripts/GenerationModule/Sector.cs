using UnityEngine;

namespace Generation
{
    public class Sector
    {
        public int X = 0;
        public int Y = 0;
        private Vector2Int center;
        private byte radius = 50;
        private Vector2Int[] SectorPoints;
        public Vector2Int[] JointPoints = new Vector2Int[4];
        public byte RadiusValue { get { return radius; } }
        public Vector2Int GetCentralPoint { get { return center; } }
        public Vector2Int RandomPoint { 
            get { //return non-joint point
                while (true) 
                {
                    bool suitablePoint = true;
                    Vector2Int randomPoint = SectorPoints[GameManager.GenRandom.Next(0, SectorPoints.Length)];
                    foreach (var jointPoint in JointPoints) if (jointPoint == randomPoint) suitablePoint = false; //if JointPoints constains this point it's not suitable
                    if (suitablePoint) return randomPoint;
                }
            } }

        public Sector(int X, int Y, byte radius, byte SectorPointsCount, Map MapToGenerate)
        {
            this.X = X;
            this.Y = Y;
            center = new Vector2Int(X * (radius * 2) + X, Y * (radius * 2) + Y);
            this.radius = radius;
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
            return new Vector2Int(GameManager.GenRandom.Next(-radius + 1, radius - 1), GameManager.GenRandom.Next(-radius + 1, radius - 1)) + center;
        }
        private Vector2Int GetRandomJointPoint(JointPointCords Side)
        {
            if (Side.x != 0) return new Vector2Int(radius * Side.x, GameManager.GenRandom.Next(-radius + 1, radius)) + center;
            else return new Vector2Int(GameManager.GenRandom.Next(-radius + 1, radius), radius * Side.y) + center;
        }
        private Vector2Int ConnectPoints(Vector2Int currentPoint, Vector2Int targetPoint, Map referenceMap, float directionRandomizeChance = 0.2f)
        {
            for (int width = 0; width < GameManager.MapGenerator.CurrentGenSettings.LinkNormalWidth; width++)
            {
                referenceMap.LandscapeMap[currentPoint.x + width, currentPoint.y] = new LandscapePoint(LandType.Passable);
                referenceMap.LandscapeMap[currentPoint.x, currentPoint.y + width] = new LandscapePoint(LandType.Passable);
            }
            do
            {
                Vector2Int currentDirection;
                if (GameManager.GenRandom.NextDouble() < directionRandomizeChance)
                {
                    currentDirection = BasicFunctions.GetRandomizedDirection(currentPoint, targetPoint);
                }
                else
                {
                    currentDirection = BasicFunctions.GetNormalizedDirectionBetween2Points(currentPoint, targetPoint);
                }
                currentPoint += currentDirection;
                int randomAdjustment = GameManager.GenRandom.Next(0, GameManager.MapGenerator.CurrentGenSettings.LinkWidthRandomAdjustment + 1);
                for (int width = 0; width < GameManager.MapGenerator.CurrentGenSettings.LinkNormalWidth + randomAdjustment; width++)
                {
                    if (Mathf.Abs(currentDirection.x) > 0) referenceMap.LandscapeMap[currentPoint.x, currentPoint.y + width] = new LandscapePoint(LandType.Passable);
                    else referenceMap.LandscapeMap[currentPoint.x + width, currentPoint.y] = new LandscapePoint(LandType.Passable);
                }
            }
            while (currentPoint != targetPoint);
            return currentPoint;
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
            for (int y = -radius + n; y <= radius - n; y++)
            {
                for (int x = -radius + n; x <= radius - n; x++)
                {
                    Vector3Int CurrentTilePos = new Vector3Int(center.x + x, center.y + y, 0);
                    if (ReferenceMap.LandscapeMap[CurrentTilePos.x, CurrentTilePos.y] != null)
                    {
                        if (ReferenceMap.LandscapeMap[CurrentTilePos.x, CurrentTilePos.y].Land == LandType.Impassable)
                        {
                            Debug.Log("Overlap impassable");
                        }
                        else if (ReferenceMap.LandscapeMap[CurrentTilePos.x, CurrentTilePos.y].Land == LandType.Passable)
                        {
                            Debug.Log("Overlap passable");
                        }

                    }
                    ReferenceMap.LandscapeMap[CurrentTilePos.x, CurrentTilePos.y] = new LandscapePoint(LandType.Impassable);
                }
            }
            //assigning points
            //adding joint points to the all points in random order
            for (int i = 0; i < JointPoints.Length; i++)
            {
                bool emptyIndexForPointFound = false;
                while (emptyIndexForPointFound == false)
                {
                    int randomPointIndex = GameManager.GenRandom.Next(0, SectorPoints.Length);
                    if (SectorPoints[randomPointIndex] == Vector2Int.zero)
                    {
                        SectorPoints[randomPointIndex] = JointPoints[i];
                        emptyIndexForPointFound = true;
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
               // CreateRoom(SectorPoints[i], 2, ReferenceMap); //clearing a bit of space at each point
                CurrentPoint = ConnectPoints(CurrentPoint, SectorPoints[i], ReferenceMap, GameManager.MapGenerator.CurrentGenSettings.LinkDirectionRandomizationChance);
            }
            QueueTilesToSet(ReferenceMap);
            //create positions to spawn units
            GameManager.unitSpawner.SpawnUnitsInSector(this, GameManager.GenRandom, GameManager.MapGenerator.CurrentGenSettings.unitSpawningPatterns);
        }
        public void CreateBorderLine(int XPart, int YPart, Map ReferenceMap, byte WallThickness = 3)
        {
            if (XPart != 0)
            {
                for (int y = -radius; y <= radius; y++)
                {
                    for (byte thickness = 0; thickness < WallThickness; thickness++)
                    {
                        Vector3Int CurrentTilePos = new Vector3Int(center.x + radius * XPart + thickness, center.y + y, 0);
                        ReferenceMap.LandscapeMap[CurrentTilePos.x, CurrentTilePos.y] = new LandscapePoint(LandType.Impassable);
                        GameManager.MapGenerator.UnpassableToSet.Add(CurrentTilePos);
                    }
                }
            }
            else if (YPart != 0)
            {
                for (int x = -radius; x <= radius; x++)
                {
                    for (byte thickness = 0; thickness < WallThickness; thickness++)
                    {
                        Vector3Int CurrentTilePos = new Vector3Int(center.x + x, center.y + radius * YPart + thickness, 0);
                        ReferenceMap.LandscapeMap[CurrentTilePos.x, CurrentTilePos.y] = new LandscapePoint(LandType.Impassable);
                        GameManager.MapGenerator.UnpassableToSet.Add(CurrentTilePos);
                    }
                }
            }
        }
        public void CheckForUselessTiles(Map ReferenceMap)
        {
            for (int y = -radius; y <= radius; y++)
            {
                for (int x = -radius; x <= radius; x++)
                {
                    Vector3Int CurrentTilePosition = new Vector3Int(center.x + x, center.y + y, 0);
                    if (TileIsUseless(BasicFunctions.ToVector2Int(CurrentTilePosition), ReferenceMap) && GameManager.MapGenerator.UnpassableToSet.Contains(CurrentTilePosition) == true)
                    {
                        //Debug.Log("Tile " + CurrentTilePosition + " is added to removal queue");
                        ReferenceMap.LandscapeMap[CurrentTilePosition.x, CurrentTilePosition.y].Land = LandType.Passable;
                        GameManager.MapGenerator.UnpassableToSet.Remove(CurrentTilePosition);

                        //and now we also checking neib tiles to removed one
                        FindNeibghourUselessTiles(CurrentTilePosition, ReferenceMap);
                    }
                }
            }
        }
        private void FindNeibghourUselessTiles(Vector3Int tilePosition, Map ReferenceMap)
        {
            for (int y = -1; y <= 1; y++)
            {
                for (int x = -1; x <= 1; x++)
                {
                    Vector3Int NeibTilePosition = new Vector3Int(tilePosition.x + x, tilePosition.y + y, 0);
                    if ((x != 0 || y != 0) && (x == 0 || y == 0)) //checking only 4 directly connected tiles
                        if (GameManager.MapGenerator.UnpassableToSet.Contains(NeibTilePosition) == true && TileIsUseless(BasicFunctions.ToVector2Int(NeibTilePosition), ReferenceMap))
                        {
                            ReferenceMap.LandscapeMap[NeibTilePosition.x, NeibTilePosition.y].Land = LandType.Passable;
                            GameManager.MapGenerator.UnpassableToSet.Remove(NeibTilePosition);
                        }
                }
            }
        }
        public void ManuallyRemoveUselessTiles(Map ReferenceMap, bool PhysicallyRemove)
        {
            for (int y = -radius; y <= radius; y++)
            {
                for (int x = -radius; x <= radius; x++)
                {
                    var currentTile = PrefabManager.Singleton.UnpassableTilemap.GetTile(new Vector3Int(center.x + x, center.y + y, 0));

                    if (currentTile != null)
                    {
                        Vector2Int CurrentTilePosition = new Vector2Int(center.x + x, center.y + y);
                        if (ReferenceMap.LandscapeMap[CurrentTilePosition.x, CurrentTilePosition.y].Land != LandType.Impassable)
                            PrefabManager.Singleton.UnpassableTilemap.SetTile(BasicFunctions.ToVector3Int(CurrentTilePosition), null);
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
            for (int y = -radius; y <= radius; y++)
            {
                for (int x = -radius; x <= radius; x++)
                {
                    Vector3Int CurrentTilePos = new Vector3Int(center.x + x, center.y + y, 0);
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
