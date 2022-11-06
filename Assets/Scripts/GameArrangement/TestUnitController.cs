using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Diagnostics;

public class TestUnitController : MonoBehaviour
{
    public Unit unit;
    public GameObject testtarget;
    public Tilemap TestingTilemap;
    public Tile tiletotest;
    public RuleTile anothertile;
    public int amount = 100;

    public bool tt = false;

    public bool AnimationTest;
    void Start()
    {
        if (AnimationTest)
        {
            GetComponent<UnityEngine.Animation>().Play("minatour run");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Vector3Int s = new Vector3Int((int)Camera.main.ScreenToWorldPoint(Input.mousePosition).x, (int)Camera.main.ScreenToWorldPoint(Input.mousePosition).y, 0);
            UnityEngine.Debug.Log(GameManager.map.LandscapeMap[(int)s.x, (int)s.y].Land + " " + s.x + " : " + s.y);
            TileBase currentTile = GameSettings.Singleton.UnpassableTilemap.GetTile(s);
            UnityEngine.Debug.Log(currentTile);
        }

        return;
        if (tt)
        {
            if (Input.GetKeyDown(KeyCode.Q)) Test(true);
            if (Input.GetKeyDown(KeyCode.W)) Test(false);
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Q)) Test2(true);
            if (Input.GetKeyDown(KeyCode.W)) Test2(false);
        }
    }
    public void Test(bool b)
    {
        Stopwatch s = new Stopwatch();
        s.Start();
        int i = 0;
        Vector3Int[] positions = new Vector3Int[amount * amount * amount];
        Tile[] tiles = new Tile[amount * amount * amount];
        RuleTile[] ruletiles = new RuleTile[amount * amount * amount];

        for (int y = -amount; y <= amount; y++)
        {
            for (int x = -amount; x <= amount; x++)
            {
                positions[i] = new Vector3Int(x, y, 0);

                if (b) tiles[i] = tiletotest;
                else ruletiles[i] = anothertile;

                i++;
            }
        }
        if (b) TestingTilemap.SetTiles(positions, tiles);
        else TestingTilemap.SetTiles(positions, ruletiles);

        s.Stop();
        UnityEngine.Debug.Log(s.ElapsedMilliseconds);
    }
    public void Test2(bool b)
    {
        Stopwatch s = new Stopwatch();
        s.Start();
        for (int y = -amount; y <= amount; y++)
        {
            for (int x = -amount; x <= amount; x++)
            {
                if (b) TestingTilemap.SetTile(new Vector3Int(x, y, 0), tiletotest);
                else TestingTilemap.SetTile(new Vector3Int(x, y, 0), anothertile);
            }
        }
        

        s.Stop();
        UnityEngine.Debug.Log(s.ElapsedMilliseconds);
    }

}
