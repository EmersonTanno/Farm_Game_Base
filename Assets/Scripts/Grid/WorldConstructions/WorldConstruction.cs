using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ConstructionTile
{
    public Vector2Int offset;
    public bool blocksMovement;
}

public class WorldConstruction : MonoBehaviour
{
    [SerializeField] private int id;
    [SerializeField] private ConstructionsType type;
    [SerializeField] private List<ConstructionTile> constructionPositions;
    [SerializeField] private GameObject prefab;

    public List<ConstructionTile> GetConstructionPositions()
    {
        return constructionPositions;
    }

    public ConstructionsType GetWorldObjectType()
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


