using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PlantsDataBaseController : MonoBehaviour
{
    public static PlantsDataBaseController Instance;

    [SerializeField] private PlantTypeDataBase plantsDB;

    void Awake()
    {
        Instance = this;
        Debug.Log(GetPlantType(1).name);
    }

    public PlantType GetPlantType(int id)
    {
        return plantsDB.GetPlant(id);
    }
}