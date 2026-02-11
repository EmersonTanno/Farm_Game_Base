using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "World/Plants Data Base")]
public class PlantTypeDataBase : ScriptableObject
{
    public List<PlantType> plants = new();

    public PlantType GetPlant(int id)
    {
        return plants.Find(t => t.id == id);
    }
}
