using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapRenderer : MonoBehaviour
{
    [SerializeField] private WorldTileDataBase tilesDataBase;
    public Tilemap soilTilemap;
    public Tilemap plantTilemap;

    [Header("Plant Tiles (growth stages)")]
    public TileBase deadPlant;
    public TileBase plowSoil;
    public TileBase wateredSoil;

    private TileMap tileMap;

    public void Init(TileMap tileMap)
    {
        this.tileMap = tileMap;
        RenderAll();
    }

    public void RenderAll()
    {
        for(int x = 0; x < tileMap.GetGrid().GetWidth(); x++)
        {
            for(int y = 0; y < tileMap.GetGrid().GetHeight(); y++)
            {
                RenderTile(x, y);
            }
        }
    }

    private TileBase GetTile(int tileId)
    {
        if(tileId == 0)
            return null;

        var tile = tilesDataBase.GetTile(tileId);

        if(tile) return tile;
        
        Debug.LogWarning($"Nenhum tile encontrado para soilState {tileId}");
        return null;
    }

    public void RenderTile(int x, int y)
    {
        RenderSoil(x, y);
    }

    private void RenderSoil(int x, int y)
    {
        int tileId = tileMap.GetGrid().GetGridObject(x, y).baseTileId;
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
            tileToUse = GetTile(tileId);
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
