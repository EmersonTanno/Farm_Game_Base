using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapRenderer : MonoBehaviour
{
    public Tilemap soilTilemap;
    public Tilemap plantTilemap;

    [SerializeField] WorldTile[] worldTiles;
    private Dictionary<int, TileBase> tileLookup;

    [Header("Plant Tiles (growth stages)")]
    public TileBase deadPlant;
    public TileBase plowSoil;
    public TileBase wateredSoil;

    private TileMap tileMap;

    public void Init(TileMap tileMap)
    {
        this.tileMap = tileMap;
        BuildTileLookup();
        RenderAll();
    }

    private void BuildTileLookup()
    {
        tileLookup = new Dictionary<int, TileBase>();

        foreach (var worldTile in worldTiles)
        {
            if (!tileLookup.ContainsKey(worldTile.id))
                tileLookup.Add(worldTile.id, worldTile);
            else
                Debug.LogWarning($"Tile duplicado com id {worldTile.id}");
        }
    }

    public void RenderAll()
    {
        for(int x = 0; x < tileMap.GetOriginalGrid().GetWidth(); x++)
        {
            for(int y = 0; y < tileMap.GetOriginalGrid().GetHeight(); y++)
            {
                RenderTile(x, y);
            }
        }
    }

    private TileBase GetTile(int soilState)
    {
        if(soilState == 0)
            return null;

        if (tileLookup.TryGetValue(soilState, out var tile))
            return tile;
        
        Debug.LogWarning($"Nenhum tile encontrado para soilState {soilState}");
        return null;
    }

    public void RenderTile(int x, int y)
    {
        RenderSoil(x, y);
    }

    private void RenderSoil(int x, int y)
    {
        int soilState = tileMap.GetOriginalGrid().GetGridObject(x, y);
        TileMapPlantData plantTile = tileMap.GetPlantGrid().GetGridObject(x, y);
        TileBase tileToUse = null;

        if(plantTile != null)
        {
            if(plantTile.plant != null || plantTile.isPlown)
            {
                RenderPlant(x, y, plantTile);
            } 
        }
        else
        {
            tileToUse = GetTile(soilState);
        }

        soilTilemap.SetTile(new Vector3Int(x, y), tileToUse);
    }

    private void RenderPlant(int x, int y, TileMapPlantData plantData)
    {
        if(!plantData.plant)
        {
            if(plantData.isPlown && plantData.isWater)
            {
                plantTilemap.SetTile(new Vector3Int(x, y), wateredSoil);
                return;
            }
            else if(plantData.isPlown)
            {
                plantTilemap.SetTile(new Vector3Int(x, y), plowSoil);
                return;
            }
        }

        var plantTile = plantData.GetStageTile();

        if(plantTile == null)
        {
            plantTilemap.SetTile(new Vector3Int(x, y), null);
            return;
        }

        if(tileMap.GetPlantGrid().GetGridObject(x, y).isDead)
        {
            plantTilemap.SetTile(new Vector3Int(x, y), deadPlant);
            return;
        }

        plantTilemap.SetTile(new Vector3Int(x, y), plantTile);
    }
}
