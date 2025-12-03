using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using UnityEngine.Tilemaps;

public class TestGrid : MonoBehaviour
{
    public static TestGrid Instance;
    //private Grid<int> grid;
    private TileMap tileMap;
    [SerializeField] PlantType plant;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        tileMap = new TileMap(20, 10, 1f, Vector3.zero);

        //testando
        tileMap.GetPlantGrid().SetValue(1, 1, new TileMapPlantData(plant, false));
        Debug.Log(tileMap.GetPlantGrid().GetGridObject(1, 1));
    }

    //teste
    public void GetPostionOnGrid(Vector2 position)
    {
        Debug.Log(tileMap.GetOriginalGrid().GetPositionOnGrid(position));
    }

    //teste
    public void GetValueOfGridPosition(Vector2 position)
    {
        Debug.Log("Original " + tileMap.GetOriginalGrid().GetGridObject(position));
        Debug.Log("Plant " + tileMap.GetPlantGrid().GetGridObject(position));
    }

    public void UpdateOriginalGrid(Vector2 position, int value)
    {
        tileMap.GetOriginalGrid().SetValue(tileMap.GetOriginalGrid().GetPositionOnGrid(position) + new Vector2(0, 1), value);
    }

    public void UpdatePlantGrid(Vector2 position, int value)
    {
        //tileMap.GetPlantGrid().SetValue(tileMap.GetOriginalGrid().GetPositionOnGrid(position) + new Vector2(0, 2), value);
    }

    // private void Update()
    // {
    //     if(Input.GetMouseButtonDown(0))
    //     {
    //         grid.SetValue(UtilsClass.GetMouseWorldPosition(), 56);
    //     }

    //     if(Input.GetMouseButtonDown(1))
    //     {
    //         Debug.Log(grid.GetGridObject(UtilsClass.GetMouseWorldPosition()));
    //     }
    // }
}
