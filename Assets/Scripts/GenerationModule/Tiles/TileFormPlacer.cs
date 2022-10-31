using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Threading.Tasks;

namespace Generation {
    public class TileFormPlacer
    {
        private Tilemap ActualTileMap;
        private MapGenerator1 MapGen;







        public TileFormPlacer(Tilemap ActualTileMap, MapGenerator1 MapGen)
        {
            this.ActualTileMap = ActualTileMap;
            this.MapGen = MapGen;


            //   CreateTilePatterns();
        }
        private bool[] GetTilesAround(Vector3Int TilePosition, Tilemap ReferenceTileMap)
        {
            bool[] NeibghourTiles = new bool[8];
            for (int i = 0; i < NeibghourTiles.Length; i++)
            {
                NeibghourTiles[i] = ReferenceTileMap.GetTile(TilePosition + BasicFunctions.ToVector3Int(BasicFunctions.NumberToOffsetPosition(i))) != null;
            }
            return NeibghourTiles;
        }
        public void ClearUselessTiles()
        {
            Debug.Log("test");
            foreach (var sector in GameManager.MapGenerator.NewlyGeneratedSectors)
            {
                Debug.Log("test3");
                sector.ManuallyRemoveUselessTiles(GameManager.map, true);
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
                OtherTilesPositions[i] = ReferenceTileMap.GetTile(TilePosition + BasicFunctions.ToVector3Int(BasicFunctions.NumberToOffsetPosition(i))) != null;
            }
        }

    }
}
