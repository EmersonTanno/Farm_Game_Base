using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapRenderer : MonoBehaviour
{
    public Tilemap soilTilemap;
    public Tilemap plantTilemap;

    [Header("Soil Tiles")]
    public TileBase soilNormal1;
    public TileBase soilNormal2;
    public TileBase soilPlowed;
    public TileBase soilWatered;

    [Header("Plant Tiles (growth stages)")]
    public TileBase deadPlant;
    public TileBase[] plantGrowthStages;

    private TileMap tileMap;

    public void Init(TileMap tileMap)
    {
        this.tileMap = tileMap;
        RenderAll();
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

    public void RenderTile(int x, int y)
    {
        RenderSoil(x, y);
    }

    private void RenderSoil(int x, int y)
    {
        int soilState = tileMap.GetOriginalGrid().GetGridObject(x, y);
        TileBase tileToUse = null;

        switch(soilState)
        {
            case 1: tileToUse = soilNormal1; break;
            case 2: tileToUse = soilNormal2; break;
            case 10: tileToUse = soilPlowed; break;
            case 11: tileToUse = soilWatered; break;
            case 20: RenderPlant(x, y); break;
        }

        soilTilemap.SetTile(new Vector3Int(x, y), tileToUse);
    }

    private void RenderPlant(int x, int y)
    {
        var plantObject = tileMap.GetPlantGrid().GetGridObject(x, y);
        var plantTile = plantObject.GetStageTile();

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
