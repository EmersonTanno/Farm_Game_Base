using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapController : MonoBehaviour
{

    public static TileMapController Instance;
    public new TileMapRenderer renderer;
    private TileMap tileMap;
    [SerializeField] WorldObjectDatabase database;

    //TESTANDO
    [SerializeField] private Tilemap groundTilemap;


    private int[,] defaultLayoutOriginalGrid = new int[,]
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

    private int[,] staticObjectLayoutGrid = new int[,]
    {
        {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        {0,2,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        {0,0,0,3,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
    };

    #region Core
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        //tileMap = new TileMap(20, 10, 1f, Vector3.zero);

        CreateGridFromTilemap();
        LoadGroundFromTilemap();
        renderer.Init(tileMap);


        //ApplyDefaultLayout();
        //renderer.Init(tileMap);

        //SpawnObjects(staticObjectLayoutGrid);
    }

    void OnEnable()
    {
        Calendar_Controller.OnDayChange += GrowPlant;
    }

    void OnDisable()
    {
        Calendar_Controller.OnDayChange -= GrowPlant;
    }

    #endregion

    #region Get
    public TileMap GetGrid()
    {
        return tileMap;
    }
    #endregion

    #region Apply Layout
    private void ApplyDefaultLayout()
    {
        for(int y = 0; y < defaultLayoutOriginalGrid.GetLength(0); y++)
        {
            for(int x = 0; x < defaultLayoutOriginalGrid.GetLength(1); x++)
            {
                tileMap.GetOriginalGrid().SetValue(x, y, defaultLayoutOriginalGrid[y, x]);
                tileMap.GetObjectGrid().SetValue(x, y, staticObjectLayoutGrid[y, x]);
            }
        }
    }
    #endregion

    #region Plow
    public void PlowSoil(Vector2 position)
    {
        Grid<int> farmGrid = tileMap.GetOriginalGrid();

        if(farmGrid.GetGridObject(position) != 1 && farmGrid.GetGridObject(position) != 2 && farmGrid.GetGridObject(position) != 20) return;

        if(farmGrid.GetGridObject(position) == 20)
        {
            var plantDead = tileMap.GetPlantGrid().GetGridObject(position).isDead;
            if(plantDead)
            {
                tileMap.GetPlantGrid().GetGridObject(position).ResetTile();
            } else
            {
                return;
            }
        }
        farmGrid.SetValue(position, 10);
        renderer.RenderTile((int)position.x, (int)position.y);
    }
    #endregion

    #region Water
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
    #endregion

    #region Plant
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

    private void GrowPlant()
    {
        for(int x = 0; x < tileMap.GetOriginalGrid().GetWidth(); x++)
        {
            for(int y = 0; y < tileMap.GetOriginalGrid().GetHeight(); y++)
            {
                var plantTile = tileMap.GetPlantGrid().GetGridObject(x, y);
        
                if (plantTile == null)
                {
                    continue;
                }
                else
                {
                    if(plantTile.plant)
                    {
                        plantTile.PassDay();
                        renderer.RenderTile(x, y);
                    }
                }
            }
        }
       
    }
    #endregion

    #region Harvest
    public void Harvest(Vector2 position)
    {
        var plantTile = tileMap.GetPlantGrid().GetGridObject(position);

        if(!plantTile.CanHarvest()) return;
        
        Harvest_Controller.Instance.SpawnHarvest(plantTile.plant, position);

        plantTile.ResetTile();

        tileMap.GetOriginalGrid().SetValue(position, 10);
        renderer.RenderTile((int)position.x, (int)position.y);
    }

    public bool CanHarvest(Vector2 position)
    {
        var plantTile = tileMap.GetPlantGrid().GetGridObject(position);
        var globalTile = tileMap.GetOriginalGrid().GetGridObject(position);

        if(globalTile != 20 || !plantTile.CanHarvest()) return false;

        return true;
    }
    #endregion

    #region  Spawn Objects
    public void SpawnObjects(int[,] grid)
    {
        int height = grid.GetLength(0);
        int width = grid.GetLength(1);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                WorldObjectID id = (WorldObjectID)grid[y, x];

                if (id != WorldObjectID.None)
                {
                    Instantiate(database.GetPrefab(id), new Vector2(x, y), Quaternion.identity);
                }
            }
        }
    }
    #endregion

    #region TESTES
    private void LoadGroundFromTilemap()
    {
        BoundsInt bounds = groundTilemap.cellBounds;

        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            TileBase tile = groundTilemap.GetTile(pos);
            if (tile == null) continue;

            WorldTile worldTile = tile as WorldTile;
            if (worldTile == null) continue;

            tileMap.GetOriginalGrid().SetValue(
                pos.x,
                pos.y,
                worldTile.id
            );
        }
    }


    [ContextMenu("Debug/Print Ground Grid")]
    public void PrintGroundGrid()
    {
        Grid<int> grid = tileMap.GetOriginalGrid();

        int width = grid.GetWidth();
        int height = grid.GetHeight();

        string result = "";

        for (int y = height - 1; y >= 0; y--)
        {
            for (int x = 0; x < width; x++)
            {
                int value = grid.GetGridObject(x, y);
                result += value.ToString().PadLeft(3) + " ";
            }
            result += "\n";
        }

        Debug.Log(result);
    }

    private void CreateGridFromTilemap()
    {
        BoundsInt bounds = groundTilemap.cellBounds;
        Debug.Log(bounds);
        int width = bounds.size.x;
        int height = bounds.size.y;

        Vector3 origin = groundTilemap.CellToWorld(bounds.min);

        tileMap = new TileMap(
            width,
            height,
            groundTilemap.layoutGrid.cellSize.x,
            Vector2.zero
        );
    }


    #endregion
}
