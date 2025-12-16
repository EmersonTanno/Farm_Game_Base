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

    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private GameObject constructionsMap;
    [SerializeField] private GameObject objectsMap;

    #region Core
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {

        CreateGridFromTilemap();
        LoadGroundFromTilemap();
        renderer.Init(tileMap);

        //ApplyDefaultLayout();
        //SpawnObjects(tileMap.GetObjectGrid());

        SetMovementGrid(tileMap.GetOriginalGrid(), tileMap.GetObjectGrid(), tileMap.GetConstructionGrid());
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
        // tileMap.GetObjectGrid().SetValue(1, 1, 1);
        // tileMap.GetObjectGrid().SetValue(2, 1, 2);
        //tileMap.GetObjectGrid().SetValue(4, 1, 3);
    }

    //Implementação de load futuro
    // private void ApplyDefaultLayout()
    // {
    //     for(int y = 0; y < tileMap.GetOriginalGrid().GetHeight(); y++)
    //     {
    //         for(int x = 0; x < tileMap.GetOriginalGrid().GetWidth(); x++)
    //         {
    //             //tileMap.GetOriginalGrid().SetValue(x, y, defaultLayoutOriginalGrid[y, x]);
    //             tileMap.GetObjectGrid().SetValue(x, y, staticObjectLayoutGrid[y, x]);
    //         }
    //     }
    // }
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
    public void SpawnObjects(Grid<int> grid)
    {
        int height = grid.GetHeight();
        int width = grid.GetWidth();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                WorldObjectID id = (WorldObjectID)grid.GetGridObject(x, y);

                if (id != WorldObjectID.None)
                {
                    Instantiate(database.GetPrefab(id), new Vector2(x, y), Quaternion.identity);
                }
            }
        }
    }


    private WorldObject[] GetObjectsInScene()
    {
        WorldObject[] objects = objectsMap.GetComponentsInChildren<WorldObject>();

        return objects;
    }

    private void SetObjectsInScene()
    {
        WorldObject[] objects = GetObjectsInScene();

        foreach(WorldObject worldObject in objects)
        {
            List<ObjectTile> objectPositionData = worldObject.GetObjectPositions();
            foreach(ObjectTile tile in objectPositionData)
            {
                Vector2 position = new Vector2(worldObject.transform.position.x + tile.offset.x, worldObject.transform.position.y + tile.offset.y);
                tileMap.GetMovementGrid().SetValue(position, !tile.blocksMovement);
                tileMap.GetObjectGrid().SetValue(position, worldObject.GetWorldObjectType());
            }
        }
    }
    #endregion

    #region Create and Load Grid from tilemap
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

    private void CreateGridFromTilemap()
    {
        groundTilemap.CompressBounds();
        BoundsInt bounds = groundTilemap.cellBounds;
        int width = bounds.size.x;
        int height = bounds.size.y;

        tileMap = new TileMap(
            width,
            height,
            groundTilemap.layoutGrid.cellSize.x,
            Vector2.zero
        );
    }
    #endregion

    #region MovementGrid
    private void SetMovementGrid(Grid<int> originalGrid, Grid<WorldObjectID> objectGrid, Grid<ConstructionsType> constructionGrid)
    {
        int height = originalGrid.GetHeight();
        int width = originalGrid.GetWidth();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int originalGridPositionValue = originalGrid.GetGridObject(x, y);
                WorldObjectID objectGridPositionValue = (WorldObjectID)objectGrid.GetGridObject(x, y);

                if(objectGridPositionValue == WorldObjectID.None)
                {
                    if(originalGridPositionValue == 0)
                    {
                        continue;
                    }

                    if
                    (
                        originalGridPositionValue == 1 || 
                        originalGridPositionValue == 2 || 
                        originalGridPositionValue == 10 || 
                        originalGridPositionValue == 11 || 
                        originalGridPositionValue == 20
                    )
                    {
                        tileMap.GetMovementGrid().SetValue(x, y, true);
                        continue;
                    }
                } else
                {
                    if(objectGridPositionValue == WorldObjectID.Bed)
                    {
                        tileMap.GetMovementGrid().SetValue(x, y, true);
                        continue;
                    }
                    if(objectGridPositionValue == WorldObjectID.ShippingBox)
                    {
                        continue;
                    }
                }
            }
        }

        SetConstructionsInScene();
        SetObjectsInScene();
    }

    public bool CanMoveInGrid(Vector2 position)
    {
        return tileMap.GetMovementGrid().GetGridObject(position);
    }
    #endregion


    #region Construction
    private WorldConstruction[] GetConstructionsInScene()
    {
        WorldConstruction[] constructions = constructionsMap.GetComponentsInChildren<WorldConstruction>();

        return constructions;
    }

    private void SetConstructionsInScene()
    {
        WorldConstruction[] constructions = GetConstructionsInScene();

        foreach(WorldConstruction construction in constructions)
        {
            List<ConstructionTile> constructionPositionData = construction.GetConstructionPositions();
            foreach(ConstructionTile tile in constructionPositionData)
            {
                Vector2 position = new Vector2(construction.transform.position.x + tile.offset.x, construction.transform.position.y + tile.offset.y);
                tileMap.GetMovementGrid().SetValue(position, !tile.blocksMovement);
                tileMap.GetConstructionGrid().SetValue(position, construction.GetWorldObjectType());
            }
        }
    }
    #endregion

    #region Debug
    [ContextMenu("Debug/Print Ground Grid")]
    public void PrintGroundGrid()
    {
        Grid<int> grid = tileMap.GetOriginalGrid();
        Grid<WorldObjectID> objectgrid = tileMap.GetObjectGrid();
        Grid<ConstructionsType> constructionGrid = tileMap.GetConstructionGrid();
        Grid<bool> movegrid = tileMap.GetMovementGrid();

        int width = grid.GetWidth();
        int height = grid.GetHeight();

        string result = "Default \n";
        string result2 = "Move \n";
        string result3 = "Object \n";
        string result4 = "Construction \n";

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int value = grid.GetGridObject(x, y);
                WorldObjectID value3 = objectgrid.GetGridObject(x, y);
                bool value2 = movegrid.GetGridObject(x, y);
                ConstructionsType value4 = constructionGrid.GetGridObject(x, y);
                result += value.ToString().PadLeft(3) + " ";
                result2 += value2.ToString().PadLeft(3) + " ";
                result3 += value3.ToString().PadLeft(3) + " ";
                result4 += value4.ToString().PadLeft(3) + " ";
            }
            result += "\n";
            result2 += "\n";
            result3 += "\n";
            result4 += "\n";
        }

        Debug.Log(result);
        Debug.Log(result3);
        Debug.Log(result4);
        Debug.Log(result2);
    }
    #endregion
}
