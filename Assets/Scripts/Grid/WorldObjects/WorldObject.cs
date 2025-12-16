using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class ObjectTile
{
    public Vector2Int offset;
    public bool blocksMovement;
}

public class WorldObject : MonoBehaviour
{

    [SerializeField] private int id;
    [SerializeField] private WorldObjectID type;
    [SerializeField] private List<ObjectTile> objectPositions;
    [SerializeField] private GameObject prefab;

    public List<ObjectTile> GetConstructionPositions()
    {
        return objectPositions;
    }

    public WorldObjectID GetWorldObjectType()
    {
        return type;
    }

    public int GetTypeId()
    {
        return (int)type;
    }

    public int GetConstructionId()
    {
        return id;
    }

    public GameObject GetConstructionPrefab()
    {
        return prefab;
    }
}
