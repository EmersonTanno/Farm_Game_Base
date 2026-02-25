using System;
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
    [SerializeField] private GameObject shopHolder;
    [SerializeField] private GameObject warpHolder;

    public static event Action OnTileMapReady;

    static readonly Vector2Int[] directions =
    {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
    };

    List<Vector2Int> RetracePath(Node endNode)
    {
        List<Vector2Int> path = new();
        Node current = endNode;

        while (current.parent != null)
        {
            path.Add(current.pos);
            current = current.parent;
        }

        path.Reverse();
        return path;
    }

    #region Core
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }


    void Start()
    {

        CreateGridFromTilemap();
        LoadGroundAndWarpFromTilemap();
        ApplySavedLayout();
        renderer.Init(tileMap);

        SetAllGrids();
        OnTileMapReady?.Invoke();
    }

    void OnEnable()
    {
        Calendar_Controller.OnDayChange += GrowPlant;
        OnTileMapReady += SetNPCsInScene;
    }

    void OnDisable()
    {
        Calendar_Controller.OnDayChange -= GrowPlant;
        OnTileMapReady -= SetNPCsInScene;
    }

    #endregion

    #region Get
    public TileMap GetGrid()
    {
        return tileMap;
    }
    #endregion

    #region Apply Layout
    private void ApplySavedLayout()
    {
        if(PersistenceController.Instance.hasData == false || SceneInfo.Instance.location != SceneLocationEnum.FARM) return;

        GridSaveData saveData = PersistenceController.Instance.LoadGridSaveData();
        Grid<WorldTileData> grid = tileMap.GetGrid();
        Grid<TileMapPlantData> plantGrid = tileMap.GetPlantGrid();

        foreach (PlantSaveData data in saveData.plants)
        {
            grid.SetValue(data.x, data.y, grid.GetGridObject(data.x, data.y).WithBaseTileId(data.gridValue));

            if (data.plantData != null)
            {
                if(data.plantData.isPlown || data.plantData.plant != null)
                {
                    plantGrid.SetValue(data.x, data.y, data.plantData);
                }
            }
        }
    }
    #endregion

    #region Plow
    public void PlowSoil(Vector2 position)
    {
        Grid<WorldTileData> grid = tileMap.GetGrid();
        Grid<TileMapPlantData> plantGrid = tileMap.GetPlantGrid();

        if (!grid.GetGridObject(position).canBePlanted || !grid.GetGridObject(position).isWalkable || grid.GetGridObject(position).warp != null) return;

        TileMapPlantData plant = plantGrid.GetGridObject(position);
        if(plant != null)
        {
            if(plant.isDead)
            {
                plant.ResetTile();
            }
        }

        plantGrid.SetValue(position, new TileMapPlantData(null, false, true));

        if (WeatherController.Instance.GetWeather() == WeatherEnum.RAIN ||
            WeatherController.Instance.GetWeather() == WeatherEnum.TEMPEST)
        {
            plantGrid.GetGridObject(position).PutWater();
        }

        renderer.RenderTile((int)position.x, (int)position.y);
    }
    #endregion

    #region Water
    public void WaterSoil(Vector2 position)
    {
        TileMapPlantData plantValue = tileMap.GetPlantGrid().GetGridObject(position);
        if(plantValue != null)
        {
            if(plantValue.isDead) return;

            plantValue.PutWater();
            renderer.RenderTile((int)position.x, (int)position.y);
        }
    }

    public void WaterSoilWithRain()
    {
        WeatherEnum weather = WeatherController.Instance.GetWeather();

        if (weather != WeatherEnum.RAIN && weather != WeatherEnum.TEMPEST)
            return;

        Grid<TileMapPlantData> plantGrid = tileMap.GetPlantGrid();

        int width = plantGrid.GetWidth();
        int height = plantGrid.GetHeight();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                TileMapPlantData plantTile = plantGrid.GetGridObject(x, y);

                if (plantTile == null) continue;
                if (plantTile.isWater) continue;

                plantTile.PutWater();
                renderer.RenderTile(x, y);
            }
        }
    }

    #endregion

    #region Plant
    public void PlantSoil(Vector2 position, PlantType plant)
    {
        TileMapPlantData plantData = tileMap.GetPlantGrid().GetGridObject(position);

        if (plantData != null)
        {
            if(plantData.plant == null)
            {
                plantData.SetPlant(plant, plantData.isWater);
            }
            renderer.RenderTile((int)position.x, (int)position.y);
        }
    }

    private void GrowPlant()
    {
        for(int x = 0; x < tileMap.GetPlantGrid().GetWidth(); x++)
        {
            for(int y = 0; y < tileMap.GetPlantGrid().GetHeight(); y++)
            {
                TileMapPlantData plantTile = tileMap.GetPlantGrid().GetGridObject(x, y);
        
                if (plantTile == null)
                {
                    continue;
                }
                else
                {
                    plantTile.PassDay();
                    renderer.RenderTile(x, y);
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

        renderer.RenderTile((int)position.x, (int)position.y);
    }

    public bool CanHarvest(Vector2 position)
    {
        var plantTile = tileMap.GetPlantGrid().GetGridObject(position);

        if(!plantTile.CanHarvest()) return false;

        return true;
    }
    #endregion

    #region Objects
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

    private ShopObject[] GetShopsInScene()
    {
        if(shopHolder == null)
        { 
            return null;
        }

        ShopObject[] shops = shopHolder.GetComponentsInChildren<ShopObject>();
        return shops;
    }

    private void SetObjectsAndShopsInScene()
    {
        WorldObject[] objects = GetObjectsInScene();
        ShopObject[] shops = GetShopsInScene();

        foreach(WorldObject worldObject in objects)
        {
            List<ObjectTile> objectPositionData = worldObject.GetObjectPositions();
            foreach(ObjectTile tile in objectPositionData)
            {
                Vector2 position = new Vector2(worldObject.transform.position.x + tile.offset.x, worldObject.transform.position.y + tile.offset.y);

                SetMoveGrid((int)position.x, (int)position.y, !tile.blocksMovement);
                tileMap.GetGrid().SetValue(position, tileMap.GetGrid().GetGridObject(position).WithObjectId(worldObject.GetWorldObjectType()));
            }
        }

        if(shops != null && shops.Length > 0)
        {
            foreach(ShopObject shopObject in shops)
            {
                tileMap.GetGrid().SetValue(shopObject.transform.position, tileMap.GetGrid().GetGridObject(shopObject.transform.position).WithShopObject(shopObject));
            }
        }
    }
    #endregion

    #region Create and Load Grid/Warp from tilemap
    private void LoadGroundAndWarpFromTilemap()
    {
        BoundsInt bounds = groundTilemap.cellBounds;

        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            TileBase tile = groundTilemap.GetTile(pos);
            if (tile == null) continue;

            WorldTile worldTile = tile as WorldTile;
            if (worldTile == null) continue;

            SetGridTileData(pos.x, pos.y, WorldTileFactory.CreateDefault(worldTile));
        }
        LoadWarpsFromScene();
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


    #region SetGrids

    private void SetAllGrids()
    {
        SetConstructionsInSceneAndConstructionsWarps();
        SetObjectsAndShopsInScene();
    }

    private void SetGridTileData(int x, int y, WorldTileData tileData)
    {
        tileMap.GetGrid().SetValue(x, y, tileData);
    }
    #endregion


    #region MovementGrid
    private void SetMoveGrid(int x, int y, bool canWalk)
    {
        tileMap.GetGrid().SetValue(x, y, tileMap.GetGrid().GetGridObject(x, y).WithIsWalkable(canWalk));
    }

    public bool CanMoveInGrid(Vector2 position)
    {
        return tileMap.GetGrid().GetGridObject(position).isWalkable;
    }
    #endregion

    #region Warp Grid
    private void SetWarpGrid(int x, int y, WarpTile warpTile)
    {
        tileMap.GetGrid().SetValue(x, y, tileMap.GetGrid().GetGridObject(x, y).WithWarp(warpTile));
    }

    private void LoadWarpsFromScene()
    {
        Warp[] warps = warpHolder.GetComponentsInChildren<Warp>(true);

        foreach (Warp warp in warps)
        {
            Vector3 worldPos = warp.transform.position;

            int x = Mathf.RoundToInt(worldPos.x);
            int y = Mathf.RoundToInt(worldPos.y);

            WarpTile warpTile = new WarpTile
            {
                scene = warp.toScene.ToString(),
                x = warp.toX,
                y = warp.toY,
                transitionType = warp.transitionType
            };

            SetWarpGrid(x, y, warpTile);
        }
    }

    #endregion

    #region Construction
    private WorldConstruction[] GetConstructionsInScene()
    {
        WorldConstruction[] constructions = constructionsMap.GetComponentsInChildren<WorldConstruction>();

        return constructions;
    }

    private void SetConstructionsInSceneAndConstructionsWarps()
    {
        WorldConstruction[] constructions = GetConstructionsInScene();

        foreach(WorldConstruction construction in constructions)
        {
            List<ConstructionTile> constructionPositionData = construction.GetConstructionPositions();
            foreach(ConstructionTile tile in constructionPositionData)
            {
                Vector2 position = new Vector2(construction.transform.position.x + tile.offset.x, construction.transform.position.y + tile.offset.y);

                SetMoveGrid((int)position.x, (int)position.y, !tile.blocksMovement);
                tileMap.GetGrid().SetValue(position, tileMap.GetGrid().GetGridObject(position).WithConstructionId(construction.GetWorldObjectType()));
            }
        }
    }
    #endregion

    #region NPC
    public void SetNPC(int x, int y, int id)
    {
        tileMap.GetGrid().SetValue(x, y, tileMap.GetGrid().GetGridObject(x, y).WithNPCId(id));
    }

    private void SetNPCsInScene()
    {
        NPCController.Instance.SetNPCsInScene();
    }

    public List<Vector2Int> FindPath(Vector2Int start, Vector2Int target, SceneLocationEnum targetScene, SceneLocationEnum currentScene,  List<SceneLocationEnum> scenesList)
    {
        List<Node> openSet = new();
        HashSet<Vector2Int> closedSet = new();

        Node startNode = new(start);
        openSet.Add(startNode);

        if(targetScene != SceneInfo.Instance.location)
        {
            target = GetWarpLocation(currentScene, scenesList);
        }

        while (openSet.Count > 0)
        {
            Node current = openSet[0];

            foreach (Node n in openSet)
                if (n.fCost < current.fCost)
                    current = n;

            openSet.Remove(current);
            closedSet.Add(current.pos);

            if (current.pos == target)
                return RetracePath(current);

            foreach (Vector2Int dir in directions)
            {
                Vector2Int nextPos = current.pos + dir;

                if (!IsWalkable(nextPos) || closedSet.Contains(nextPos))
                    continue;

                int newCost = current.gCost + GetMoveCost(nextPos);

                Node neighbor = openSet.Find(n => n.pos == nextPos);
                if (neighbor == null)
                {
                    neighbor = new Node(nextPos);
                    neighbor.gCost = newCost;
                    neighbor.hCost = Manhattan(nextPos, target);
                    neighbor.parent = current;
                    openSet.Add(neighbor);
                }
                else if (newCost < neighbor.gCost)
                {
                    neighbor.gCost = newCost;
                    neighbor.parent = current;
                }
            }
        }

        return null;
    }

    private int Manhattan(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    private bool IsWalkable(Vector2Int pos)
    {
        return tileMap.GetGrid().GetGridObject(new Vector3(pos.x, pos.y, 0)).isWalkable;
    }

    private bool IsNPCOnWay(Vector2Int pos)
    {
        if(tileMap.GetGrid().GetGridObject(new Vector3(pos.x, pos.y, 0)).npcId != 0)
        {
            return true;
        }
        return false;
    }

    private Vector2Int GetWarpLocation(SceneLocationEnum currentScene, List<SceneLocationEnum> scenesList)
    {
        if (scenesList == null || scenesList.Count < 2)
            return Vector2Int.zero;

        int index = scenesList.FindIndex(p => p == currentScene);
        SceneLocationEnum nextScene = scenesList[index + 1];

        Grid<WorldTileData> grid = tileMap.GetGrid();
        int width = grid.GetWidth();
        int height = grid.GetHeight();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                WarpTile warp = grid.GetGridObject(x, y).warp;
                if (warp == null) continue;

                if (warp.scene.Equals(nextScene.ToString(),
                    StringComparison.OrdinalIgnoreCase))
                {
                    return new Vector2Int(x, y);
                }
            }
        }

        Debug.LogWarning($"Warp não encontrado para {nextScene}");
        return Vector2Int.zero;
    }

    public Vector2Int GetWarpLocationInScene(SceneLocationEnum scene)
    {
        Grid<WorldTileData> grid = tileMap.GetGrid();
        int width = grid.GetWidth();
        int height = grid.GetHeight();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                WarpTile warp = grid.GetGridObject(x, y).warp;
                if (warp == null) continue;

                if (warp.scene.Equals(scene.ToString(),
                    StringComparison.OrdinalIgnoreCase))
                {
                    return new Vector2Int(x, y);
                }
            }
        }

        Debug.LogWarning($"Warp não encontrado para {scene}");
        return Vector2Int.zero;
    }

    private int GetMoveCost(Vector2Int pos)
    {
        int cost = 10;

        if (tileMap.GetGrid().GetGridObject(new Vector3(pos.x, pos.y, 0)).isPath)
        {
            cost = 1;
        }

        return cost;
    }
    #endregion

    #region Debug
    [ContextMenu("Debug/Print Ground Grid")]
    public void PrintGroundGrid()
    {
        Grid<WorldTileData> grid = tileMap.GetGrid();

        int width = grid.GetWidth();
        int height = grid.GetHeight();

        string result = "Default \n";
        string result2 = "Move \n";
        string result3 = "Object \n";
        string result4 = "Construction \n";
        string result5 = "NPC \n";

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int value = grid.GetGridObject(x, y).baseTileId;
                result += value.ToString().PadLeft(3) + " ";
            }
            result += "\n";
            result2 += "\n";
            result3 += "\n";
            result4 += "\n";
            result5 += "\n";
        }

        Debug.Log(result);
        Debug.Log(result3);
        Debug.Log(result4);
        Debug.Log(result2);
        Debug.Log(result5);
    }
    #endregion
}
