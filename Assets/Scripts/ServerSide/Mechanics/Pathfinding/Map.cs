using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ServerSideLogic
{
    public class Map
    {
        public Vector3 StartingPosition = new Vector3(50, 0, 50);
        public Vector3 CenterOfTheMap;
        public int MapSizeX = 500;
        public int MapSizeY = 500;
        public bool[,] ObstaclesMap;


        public Map()
        {
            ObstaclesMap = new bool[MapSizeX + (int)StartingPosition.x + 100, MapSizeY + (int)StartingPosition.z + 100];
            CenterOfTheMap = StartingPosition + new Vector3(MapSizeX / 2, 1, MapSizeY / 2);

            CreateMapBorders();
        }

        private void CreateMapBorders()
        {
            //generating invisible map borders
            for (int x = 0; x <= MapSizeX; x++)
            {
                for (int WallWidth = 0; WallWidth < 2; WallWidth++)
                {
                    ObstaclesMap[x, WallWidth] = true;
                    ObstaclesMap[x, MapSizeY + WallWidth] = true;
                }
            }

            for (int y = 0; y <= MapSizeY; y++)
            {
                for (int WallWidth = 0; WallWidth < 2; WallWidth++)
                {
                    ObstaclesMap[WallWidth, y] = true;
                    ObstaclesMap[MapSizeX + WallWidth, y] = true;
                }
            }
        }
        public void ApplyObstacle(Unit Obstacle)
        {
            if (Obstacle.Type.Stats.ObstacleRadius == 0) return;
            for (int y = -Obstacle.Type.Stats.ObstacleRadius; y <= Obstacle.Type.Stats.ObstacleRadius; y++)
                for (int x = -Obstacle.Type.Stats.ObstacleRadius; x <= Obstacle.Type.Stats.ObstacleRadius; x++)
                {
                    ObstaclesMap[(int)Obstacle.position.x + x, (int)Obstacle.position.z + y] = true;
                    // Debug.Log(Obstacle.position.x + x + " " + Obstacle.position.z + y + " is obstacle by " + Obstacle.UnitName);
                }
        }




    }
}
