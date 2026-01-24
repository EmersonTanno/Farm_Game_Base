using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGraphEditor : EditorWindow
{
    private MapGraph mapGraph;
    private SceneLocationEnum currentScene;
    private Tilemap sourceTilemap;
    private GameObject constructionsHolder;
    private GameObject objectsHolder;

    [MenuItem("Tools/World/Map Graph Builder")]
    public static void Open()
    {
        GetWindow<MapGraphEditor>("Map Graph Builder");
    }

    private void OnGUI()
    {
        GUILayout.Label("Map Graph Builder", EditorStyles.boldLabel);

        mapGraph = (MapGraph)EditorGUILayout.ObjectField(
            "Map Graph",
            mapGraph,
            typeof(MapGraph),
            false
        );

        currentScene = (SceneLocationEnum)EditorGUILayout.EnumPopup(
            "Scene",
            currentScene
        );

        sourceTilemap = (Tilemap)EditorGUILayout.ObjectField(
            "Source Tilemap",
            sourceTilemap,
            typeof(Tilemap),
            true
        );

        constructionsHolder = (GameObject)EditorGUILayout.ObjectField(
            "Constructions",
            constructionsHolder,
            typeof(GameObject),
            true
        );

        objectsHolder = (GameObject)EditorGUILayout.ObjectField(
            "Objects",
            objectsHolder,
            typeof(GameObject),
            true
        );

        GUILayout.Space(10);

        if (GUILayout.Button("Generate Map From Scene"))
        {
            if (mapGraph == null || sourceTilemap == null)
            {
                Debug.LogError("MapGraph ou Tilemap não atribuídos.");
                return;
            }

            GenerateMap();
        }
    }

    private void GenerateMap()
    {
        BoundsInt bounds = sourceTilemap.cellBounds;

        MapData map = mapGraph.maps.Find(m => m.scene == currentScene);
        if (map == null)
        {
            map = new MapData
            {
                scene = currentScene
            };
            mapGraph.maps.Add(map);
        }

        int width = bounds.size.x;
        int height = bounds.size.y;

        map.Init(width, height);

        for(int x = 0; x < width - 1; x++)
        {
            for(int y = 0; y < height - 1; y++)
            {
                TileBase tile = sourceTilemap.GetTile(new Vector3Int(x, y, 0));
                if (tile == null) continue;

                WorldTile worldTile = tile as WorldTile;
                if (worldTile == null) continue;

                map.Set(x, y, ConvertTile(worldTile));
            }
        }

        if(constructionsHolder != null)
        {
            WorldConstruction[] constructions = constructionsHolder.GetComponentsInChildren<WorldConstruction>();
            foreach(WorldConstruction construction in constructions)
            {
                List<ConstructionTile> constructionPositionData = construction.GetConstructionPositions();
                foreach(ConstructionTile tile in constructionPositionData)
                {
                    Vector2 position = new Vector2(construction.transform.position.x + tile.offset.x, construction.transform.position.y + tile.offset.y);

                    if(map.Get((int)position.x, (int)position.y) == 2) continue;

                    if(tile.blocksMovement)
                    map.Set((int)position.x, (int)position.y, tile.blocksMovement ? 0 : 1);
                }
            }
        }

        if(objectsHolder != null)
        {
            WorldObject[] objects = objectsHolder.GetComponentsInChildren<WorldObject>();
            foreach(WorldObject worldObject in objects)
            {
                List<ObjectTile> objectPositionData = worldObject.GetObjectPositions();
                foreach(ObjectTile tile in objectPositionData)
                {
                    Vector2 position = new Vector2(worldObject.transform.position.x + tile.offset.x, worldObject.transform.position.y + tile.offset.y);

                    if(map.Get((int)position.x, (int)position.y) == 2) continue;

                    if(tile.blocksMovement)
                    map.Set((int)position.x, (int)position.y, tile.blocksMovement ? 0 : 1);
                }
            }
        }

        EditorUtility.SetDirty(mapGraph);

        Debug.Log($"Mapa da cena {currentScene} gerado com sucesso.");

        LogMap(width, height, map);
    }

    private int ConvertTile(WorldTile tile)
    {
        if (tile.isPath) return 2;
        if (tile.isWalkable) return 1;
        return 0;
    }

    private void LogMap(int width, int height, MapData map)
    {
        string result = "";

        for(int x = 0; x < width - 1; x++)
        {
            for(int y = 0; y < height - 1; y++)
            {
                result += map.Get(x, y) + " ";
            }
            result += "\n";
        }

        Debug.Log(result);
    }
}

