using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Generation;

public class Map
{
    public LandscapeMapHolder LandscapeMap = new LandscapeMapHolder();
    public MapGenerator1.SectorMapHolder SectorMap = new MapGenerator1.SectorMapHolder();




    public Map()
    {
        LandscapeMap = new LandscapeMapHolder();
    }



    public void ApplyObstacle(Unit Obstacle)
    {
        if (Obstacle.Stats.CollisionRadius == 0) return;
        for (int y = -Obstacle.Stats.CollisionRadius; y <= Obstacle.Stats.CollisionRadius; y++)
            for (int x = -Obstacle.Stats.CollisionRadius; x <= Obstacle.Stats.CollisionRadius; x++)
            {
                LandscapeMap[(int)Obstacle.transform.position.x + x, (int)Obstacle.transform.position.z + y] = new LandscapePoint(LandType.Impassable);
                // Debug.Log(Obstacle.position.x + x + " " + Obstacle.position.z + y + " is obstacle by " + Obstacle.UnitName);
            }
    }


}

public class LandscapePoint
{
    public LandType Land { get; set; } = LandType.Passable;
    public LandscapePoint(LandType landType)
    {
        Land = landType;
    }
}
public class LandscapeMapHolder
{
    Dictionary<Tuple<int, int>, LandscapePoint> MapDictionary = new Dictionary<Tuple<int, int>, LandscapePoint>();


    public LandscapePoint this [int x, int y]
    {
        get
        {
            var t = Tuple.Create(x, y);
            if (MapDictionary.ContainsKey(t)) return MapDictionary[t];
            return null;
        }
        set
        {
            var t = Tuple.Create(x, y);
            MapDictionary[t] = value;
        }
    }
}
public enum LandType : byte
{
    Passable = 0,
    Impassable = 1,
    DestructibleImpassable = 2,
    WaterLow = 3,
    WaterDeep = 4
}