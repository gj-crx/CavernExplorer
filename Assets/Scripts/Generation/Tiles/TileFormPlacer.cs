using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Threading.Tasks;

public class TileFormPlacer
{
    private Tilemap PatternTileMap;
    private Tilemap ActualTileMap;



    private List<TilePattern> TilePatterns = new List<TilePattern>();

   



    public TileFormPlacer(Tilemap PatternTileMap, Tilemap ActualTileMap)
    {
        this.PatternTileMap = PatternTileMap;
        this.ActualTileMap = ActualTileMap;
        

     //   CreateTilePatterns();
    }


    public async void DeleteUselessTiles(Map ReferenceMap)
    {
        await Task.Delay(1000);

        for (int y = ActualTileMap.cellBounds.yMin; y <= ActualTileMap.cellBounds.yMax; y++)
        {
            for (int x = ActualTileMap.cellBounds.xMin; x <= ActualTileMap.cellBounds.xMax; x++)
            {
                var _tile = ActualTileMap.GetSprite(new Vector3Int(x, y, 0));
                if (_tile != null && _tile.name == GameSettings.Singleton.TileNameToRemove)
                {
                  //  Debug.Log(_tile.name + " detected and removed in " + new Vector3Int(x, y, 0));
                    ActualTileMap.SetTile(new Vector3Int(x, y, 0), null);
                    ReferenceMap.LandscapeMap[x, y] = new LandscapePoint(LandType.Passable);
                }
            }
        }
    }


    private void CreateTilePatterns()
    {
        int PatternBorderMinX = PatternTileMap.cellBounds.xMin;
        int PatternBorderMinY = PatternTileMap.cellBounds.yMin;
        int PatternBorderMaxX = PatternTileMap.cellBounds.xMax;
        int PatternBorderMaxY = PatternTileMap.cellBounds.yMax;

        for (int y = PatternTileMap.cellBounds.yMin; y <= PatternTileMap.cellBounds.yMax; y++)
        {
            for (int x = PatternTileMap.cellBounds.xMin; x <= PatternTileMap.cellBounds.xMax; x++)
            {
                var _tile = PatternTileMap.GetTile(new Vector3Int(x, y, 0));
                if (_tile != null && PatternsListContainsTile(GetTilesAround(new Vector3Int(x, y, 0), PatternTileMap)) == false)
                {
                    TilePatterns.Add(new TilePattern(new Vector3Int(x, y, 0), PatternTileMap));
                }
            }
        }
        Debug.Log(TilePatterns.Count);
    }
    private TileBase GetProperTileType(bool[] TilesAround)
    {
        foreach (var Pattern in TilePatterns)
        {
            if (BasicFunctions.ArrayOfBoolEquals(Pattern.OtherTilesPositions, TilesAround))
            {
                Debug.Log("found type");
                return Pattern._tile;
            }
        }
        Debug.Log("pattern for tile not found");
        return null;
    }
    private bool PatternsListContainsTile(bool[] SurroundingTiles)
    {
        foreach (var Pattern in TilePatterns)
        {
            if (BasicFunctions.ArrayOfBoolEquals(SurroundingTiles, Pattern.OtherTilesPositions)) return true;
        }
        return false;
    }
    private bool[] GetTilesAround(Vector3Int TilePosition, Tilemap ReferenceTileMap)
    {
        bool[] NeibghourTiles = new bool[8];
        for (int i = 0; i < NeibghourTiles.Length; i++)
        {
            NeibghourTiles[i] = ReferenceTileMap.GetTile(TilePosition + BasicFunctions.Vector2IntToVector3Int(BasicFunctions.NumberToOffsetPosition(i))) != null;
        }
        return NeibghourTiles;
    }
    public void ReplaceAllTiles()
    {
        for (int y = ActualTileMap.cellBounds.yMin; y <= ActualTileMap.cellBounds.yMax; y++)
        {
            for (int x = ActualTileMap.cellBounds.xMin; x <= ActualTileMap.cellBounds.xMax; x++)
            {
                var _tile = ActualTileMap.GetTile(new Vector3Int(x, y, 0));
                if (_tile != null)
                {
                    ActualTileMap.SetTile(new Vector3Int(x, y, 0), GetProperTileType(GetTilesAround(new Vector3Int(x, y, 0), ActualTileMap)));

                }
            }
        }
    }
}
public class TilePattern
{
    public bool[] OtherTilesPositions = new bool[8];
    public TileBase _tile = null;

    public TilePattern(Vector3Int TilePosition, Tilemap ReferenceTileMap)
    {
        _tile = ReferenceTileMap.GetTile(TilePosition);
        for (int i = 0; i < OtherTilesPositions.Length; i++)
        {
            OtherTilesPositions[i] = ReferenceTileMap.GetTile(TilePosition + BasicFunctions.Vector2IntToVector3Int(BasicFunctions.NumberToOffsetPosition(i))) != null;
        }
    }
    
}
