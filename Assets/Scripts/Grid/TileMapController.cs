using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMapController : MonoBehaviour
{

    public static TileMapController Instance;
    public new TileMapRenderer renderer;

    private TileMap tileMap;

    private int[,] defaultLayout = new int[,]
    {
        {0,1,0,1,0,1,0,1,0,1,0,1,0,1,0,1,0,1,0,1},
        {1,0,1,0,1,0,1,0,1,0,1,0,1,0,1,0,1,0,1,0},
        {0,1,0,1,0,1,0,1,0,1,0,1,0,1,0,1,0,1,0,1},
        {1,0,1,0,1,0,1,0,1,0,1,0,1,0,1,0,1,0,1,0},
        {0,1,0,1,0,1,0,1,0,1,0,1,0,1,0,1,0,1,0,1},
        {1,0,1,0,1,0,1,0,1,0,1,0,1,0,1,0,1,0,1,0},
        {0,1,0,1,0,1,0,1,0,1,0,1,0,1,0,1,0,1,0,1},
        {1,0,1,0,1,0,1,0,1,0,1,0,1,0,1,0,1,0,1,0},
        {0,1,0,1,0,1,0,1,0,1,0,1,0,1,0,1,0,1,0,1},
        {1,0,1,0,1,0,1,0,1,0,1,0,1,0,1,0,1,0,1,0}
    };

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        tileMap = new TileMap(20, 10, 1f, Vector3.zero);

        ApplyDefaultLayout();

        renderer.Init(tileMap);
    }

    private void ApplyDefaultLayout()
    {
        for(int y = 0; y < defaultLayout.GetLength(0); y++)
        {
            for(int x = 0; x < defaultLayout.GetLength(1); x++)
            {
                tileMap.GetOriginalGrid().SetValue(x, y, defaultLayout[y, x]);
            }
        }
    }

    public void PlowSoil(Vector2 position)
    {
        tileMap.GetOriginalGrid().SetValue(position, 10);
        renderer.RenderTile((int)position.x, (int)position.y);
    }

    public void WaterSoil(Vector2 position)
    {
        if(tileMap.GetOriginalGrid().GetGridObject(position) == 10)
        {
            tileMap.GetOriginalGrid().SetValue(position, 11);
            renderer.RenderTile((int)position.x, (int)position.y);
        }
    }


}
