using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Harvest_Controller : MonoBehaviour
{
    public static Harvest_Controller Instance { get; private set; }

    [SerializeField] Item_Dropped item_Dropped;

    void Awake()
    {
        Instance = this;
    }

    public void SpawnHarvest(PlantType itemHarvest, Vector3 spawnPosition)
    {
        int spawnedHarvest = 0;
        int quantity = Random.Range(itemHarvest.harvestMin, itemHarvest.harvestMax + 1); 
        Debug.Log(quantity);

        while (quantity > spawnedHarvest)
        {
            Vector2 randomOffset = Random.insideUnitCircle * 0.5f;
            Vector3 finalPos = spawnPosition + new Vector3(randomOffset.x, randomOffset.y, 0f);

            Item_Dropped harvest = Instantiate(item_Dropped, finalPos, Quaternion.identity);
            harvest.SetItem(itemHarvest.harvest.harvestItem);
            
            spawnedHarvest++;
        }
    }

}
