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
        {1,2,1,2,1,2,1,2,1,2,1,2,1,2,1,2,1,2,1,2},
        {2,1,2,1,2,1,2,1,2,1,2,1,2,1,2,1,2,1,2,2},
        {1,2,1,2,1,2,1,2,1,2,1,2,1,2,1,2,1,2,1,2},
        {2,1,2,1,2,1,2,1,2,1,2,1,2,1,2,1,2,1,2,2},
        {1,2,1,2,1,2,1,2,1,2,1,2,1,2,1,2,1,2,1,2},
        {2,1,2,1,2,1,2,1,2,1,2,1,2,1,2,1,2,1,2,2},
        {1,2,1,2,1,2,1,2,1,2,1,2,1,2,1,2,1,2,1,2},
        {2,1,2,1,2,1,2,1,2,1,2,1,2,1,2,1,2,1,2,2},
        {1,2,1,2,1,2,1,2,1,2,1,2,1,2,1,2,1,2,1,2},
        {2,1,2,1,2,1,2,1,2,1,2,1,2,1,2,1,2,1,2,2}
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
        Grid<int> farmGrid = tileMap.GetOriginalGrid();

        if(farmGrid.GetGridObject(position) != 1 && farmGrid.GetGridObject(position) != 2) return;

        farmGrid.SetValue(position, 10);
        renderer.RenderTile((int)position.x, (int)position.y);
    }

    public void WaterSoil(Vector2 position)
    {
        int tileValue = tileMap.GetOriginalGrid().GetGridObject(position);
        if(tileValue == 10 || tileValue == 20)
        {
            switch(tileValue)
            {
                case 10:
                {
                    tileMap.GetOriginalGrid().SetValue(position, 11);
                    renderer.RenderTile((int)position.x, (int)position.y);
                    break;
                }
                case 20:
                {
                    tileMap.GetPlantGrid().GetGridObject(position).PutWater();
                    renderer.RenderTile((int)position.x, (int)position.y);
                    break;
                }
            }
        }
    }

    public void PlantSoil(Vector2 position, PlantType plant)
    {
        int tileValue = tileMap.GetOriginalGrid().GetGridObject(position);
        
        if (tileValue == 10 || tileValue == 11)
        {
            switch(tileValue)
            {
                case 10: 
                {
                    var plantTile = tileMap.GetPlantGrid().GetGridObject(position);
        
                    if (plantTile == null)
                    {
                        tileMap.GetOriginalGrid().SetValue(position, 20);
                        tileMap.GetPlantGrid().SetValue((int)position.x, (int)position.y, new TileMapPlantData(plant, false));
                    }
                    else
                    {
                        tileMap.GetOriginalGrid().SetValue(position, 20);
                        plantTile.SetPlant(plant, false);
                    }

                    break;
                }

                case 11:
                {
                    var plantTile = tileMap.GetPlantGrid().GetGridObject(position);
        
                    if (plantTile == null)
                    {
                        tileMap.GetOriginalGrid().SetValue(position, 20);
                        tileMap.GetPlantGrid().SetValue((int)position.x, (int)position.y, new TileMapPlantData(plant, true));
                    }
                    else
                    {
                        tileMap.GetOriginalGrid().SetValue(position, 20);
                        plantTile.SetPlant(plant, true);
                    }

                    break;
                }
            }

            renderer.RenderTile((int)position.x, (int)position.y);
        }
    }



}
