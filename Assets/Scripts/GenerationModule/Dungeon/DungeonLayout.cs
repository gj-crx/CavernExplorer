using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Generation;

public class DungeonLayout 
{
    public bool[,] dungeonWallsMap;
    public Vector2Int dungeonCenter;

    private Quad[,] quadMap;

    private int quadsCount;
    private int quadSize;

    private DungeonGenerationSettings settings;
    private System.Random genRandom;

    public DungeonLayout(DungeonGenerationSettings settings, System.Random genRandom, int quadsCount, Vector2Int entryPoint)
    {
        this.settings = settings;
        this.genRandom = genRandom;
        this.quadsCount = quadsCount;

        quadSize = (int)Mathf.Sqrt(settings.XRadius * settings.YRadius / quadsCount);

        FormQuadsMap();
        GenerateWallsMap();

    }
    private void FormQuadsMap()
    {
        int quadCountSqrt = (int)Mathf.Sqrt(quadsCount);
        quadMap = new Quad[quadCountSqrt, quadCountSqrt];
        for (int y = 0; y <= quadCountSqrt; y++)
        {
            for (int x = 0; x <= quadCountSqrt; x++)
            {
                quadMap[x, y] = new Quad(new Vector2Int(x, y), new Vector2Int(x * quadSize, y * quadSize) + dungeonCenter, 5, this); 
            }
        }
    }

    private void GenerateWallsMap(Quad entryQuad)
    { //using wave algorithm to make corridors
        List<Quad>[] quadLists = new List<Quad>[2];
        quadLists[0] = new List<Quad>();
        quadLists[0].Add(entryQuad);

        List<Quad> currentQuadList = quadLists[0];
        List<Quad> NextQuadList = quadLists[1];

        foreach (var currentQuad in currentQuadList)
        { //operating every quad to make his corridors and add his neibghour to list
            if (currentQuad.GeneratedCorridors == false)
            {
                var neibQuads = GetNeigbhourQuads(currentQuad.positionOnMap);
                foreach (var neibQuad in neibQuads) NextQuadList.Add(neibQuad); //merging current found neibghours to main list
                currentQuad.GenerateCorridors(neibQuads);
            }
        }
    }
    private List<Quad> GetNeigbhourQuads(Vector2Int position)
    {
        List<Quad> neibQuads = new List<Quad>();
        for (int y = -1; y <= 1; y++)
        {
            for (int x = -1; x <= 1; x++)
            {
                Vector2Int currentPosition = position + new Vector2Int(x, y);
                if ((x == 0 || y == 0) && currentPosition.x < quadMap.GetLength(0) && currentPosition.y < quadMap.GetLength(1) && currentPosition.x >= 0 && currentPosition.y >= 0)
                    neibQuads.Add(quadMap[currentPosition.x, currentPosition.y]);
            }
        }
        return neibQuads;
    }
    private void MakeALine(Vector2Int target, Vector2Int from, int width)
    {
        Vector2Int direction = BasicFunctions.NormalizeVector2Int(target - from);
        int length;
        if (direction.x > 0) length = direction.x;
        else length = direction.y;

        for (int currentLength = 0; currentLength < length; currentLength++)
        {
            Vector2Int currentPosition = from + direction * currentLength + BasicFunctions.ReverseDirection(direction) * width;
            dungeonWallsMap[currentPosition.x, currentPosition.y] = true;

            currentPosition = from + direction * currentLength - BasicFunctions.ReverseDirection(direction) * width;
            dungeonWallsMap[currentPosition.x, currentPosition.y] = true;
        }
    }
    private class Quad
    {
        public Vector2Int positionOnMap;
        public Vector2Int RoomCenter;
        public int RoomRadius = 5;
        public bool GeneratedCorridors = false;

        private DungeonLayout layout;

        public Quad(Vector2Int positionOnMap, Vector2Int roomCenter, int roomRadius, DungeonLayout layout)
        {
            this.positionOnMap = positionOnMap;
            this.RoomCenter = roomCenter;
            this.RoomRadius = roomRadius;
            this.layout = layout;
        }
        
        public void GenerateCorridors(List<Quad> neibghourQuads)
        {
            foreach (var neibQuad in neibghourQuads)
            {
                if (GameManager.GenRandom.NextDouble() < 0.6f) layout.MakeALine(neibQuad.RoomCenter, RoomCenter, 1); //connecting 2 room centers with a line
            }
            GeneratedCorridors = true;
        }
    }
}
