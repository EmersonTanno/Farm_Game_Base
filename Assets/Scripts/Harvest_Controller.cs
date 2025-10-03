using System.Collections;
using System.Collections.Generic;
using Dustopia.RewardSystem;
using UnityEngine;

public class Harvest_Controller : MonoBehaviour
{
    public static Harvest_Controller Instance { get; private set; }

    [SerializeField] GameObject item_Dropped;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        ObjectPool.Instance.InstantiatePool(item_Dropped, 10);
    }

    public void SpawnHarvest(PlantType itemHarvest, Vector3 spawnPosition)
    {
        int spawnedHarvest = 0;
        int quantity = Random.Range(itemHarvest.harvestMin, itemHarvest.harvestMax + 1);

        while (quantity > spawnedHarvest)
        {
            Vector2 randomOffset = Random.insideUnitCircle * 0.5f;
            Vector3 finalPos = spawnPosition + new Vector3(randomOffset.x, randomOffset.y, 0f);

            GameObject drop = ObjectPool.Instance.GetObject(item_Dropped);
            drop.transform.position = finalPos;
            Item_Dropped harvest = drop.GetComponent<Item_Dropped>();
            harvest.SetItem(itemHarvest.harvest.harvestItem);

            spawnedHarvest++;
        }
    }

}
