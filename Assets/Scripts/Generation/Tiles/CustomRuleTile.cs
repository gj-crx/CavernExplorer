using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class CustomRuleTile 
{
    public Tile tile;
    public TileStatus[] RulesForPlacing = new TileStatus[9];


    public enum TileStatus : byte
    {
        NotAssigned = 0,
        TileExist = 1,
        EmptyTile = 2
    }

    public bool CheckRules(Vector3Int PositionToCheck, Map ReferenceMap)
    {
        int counter = 0;
        for (int y = -1; y <= 1; y++)
        {
            for (int x = -1; x <= 1; x++)
            {
                if (RulesForPlacing[counter] != TileStatus.NotAssigned)
                {
                    if (RulesForPlacing[counter] == TileStatus.TileExist)
                    {
                        if (ReferenceMap.LandscapeMap[PositionToCheck.x + x, PositionToCheck.y + y] == null) return false;
                    }
                    else
                    {
                        if (ReferenceMap.LandscapeMap[PositionToCheck.x + x, PositionToCheck.y + y] != null) return false;
                    }
                }
                counter++;
            }
        }
        return true;
    }
}
