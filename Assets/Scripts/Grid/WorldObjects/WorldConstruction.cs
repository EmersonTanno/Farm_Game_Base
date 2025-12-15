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
    [SerializeField] private WorldObjectID type;
    [SerializeField] private List<ConstructionTile> constructionPositions;

    public List<ConstructionTile> GetConstructionPositions()
    {
        return constructionPositions;
    }

    public WorldObjectID GetWorldObjectType()
    {
        return type;
    }

    public int GetTypeId()
    {
        return (int)type;
    }
}


