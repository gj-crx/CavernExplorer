using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Types;

namespace ServerSideLogic
{
    public static class MapGenerator
    {
        public static void GenerateTrees(Game GameToGenerate, int AmountOfTrees)
        {
            for (int i = 0; i < AmountOfTrees; i++)
            {
                new Unit(GameToGenerate, GameToGenerate.map.StartingPosition + new Vector3(Random.Range(0, GameToGenerate.map.MapSizeX), 0, Random.Range(0, GameToGenerate.map.MapSizeX)), null,
                    TypesData.TreeTypes[0].UnitTypeID);
            }
        }
    }
}
