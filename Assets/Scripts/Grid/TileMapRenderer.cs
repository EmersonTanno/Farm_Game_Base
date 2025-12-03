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
        RenderPlant(x, y);
    }

    private void RenderSoil(int x, int y)
    {
        int soilState = tileMap.GetOriginalGrid().GetGridObject(x, y);
        TileBase tileToUse = null;

        switch(soilState)
        {
            case 0: tileToUse = soilNormal1; break;
            case 1: tileToUse = soilNormal2; break;
            case 10: tileToUse = soilPlowed; break;
            case 11: tileToUse = soilWatered; break;
        }

        soilTilemap.SetTile(new Vector3Int(x, y), tileToUse);
    }

    private void RenderPlant(int x, int y)
    {
        var plantData = tileMap.GetPlantGrid().GetGridObject(x, y);

        if(plantData == null || plantData.plant == null)
        {
            plantTilemap.SetTile(new Vector3Int(x, y), null);
            return;
        }

        int stage = Mathf.Clamp(plantData.growthDays, 0, plantGrowthStages.Length - 1);

        plantTilemap.SetTile(new Vector3Int(x, y), plantGrowthStages[stage]);
    }
}
