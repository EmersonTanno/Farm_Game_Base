using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class TestGrid : MonoBehaviour
{
    private Grid<int> grid;

    void Start()
    {
        grid = new Grid<int>(3, 4, 1f, new Vector3(0, 0));
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            grid.SetValue(UtilsClass.GetMouseWorldPosition(), 56);
        }

        if(Input.GetMouseButtonDown(1))
        {
            Debug.Log(grid.GetGridObject(UtilsClass.GetMouseWorldPosition()));
        }
    }
}
