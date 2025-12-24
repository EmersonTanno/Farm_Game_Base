using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ConstructionTile
{
    public Vector2Int offset;
    public bool blocksMovement;
    public bool isWarp;
    public WarpTile warp;
}

public class WorldConstruction : MonoBehaviour
{
    [SerializeField] private int id;
    [SerializeField] private ConstructionsType type;
    [SerializeField] private List<ConstructionTile> constructionPositions;
    [SerializeField] private GameObject prefab;

    [SerializeField] private SpriteRenderer sprite;
    private Player_Controller player;

    void Awake()
    {
        player = FindObjectOfType<Player_Controller>();
    }

    void Update()
    {
        if (Mathf.Abs(player.transform.position.y) < Mathf.Abs(transform.position.y)+1)
        {
            sprite.sortingLayerName = "ConstructionBelow";
        }
        else
        {
            sprite.sortingLayerName = "ConstructionAbove";
        }
    }

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


