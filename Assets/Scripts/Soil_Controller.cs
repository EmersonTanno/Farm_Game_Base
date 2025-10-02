using UnityEditor.SceneManagement;
using UnityEngine;

public class Soil_Controller : MonoBehaviour
{
    #region Variables

    //Sprites
    [SerializeField] private Sprite without;
    [SerializeField] Sprite withWater;
    private SpriteRenderer mySprite;

    //Conditions
    private bool isPlanted = false;
    private bool isWater = false;
    private bool dead = false;

    //Days Count
    private int days = 0;
    private int daysWithoutWater = 0;

    //Plant
    [HideInInspector] public PlantType currentPlant;
    #endregion

    void OnEnable()
    {
        Calendar_Controller.OnDayChange += GrowPlant;
    }

    void OnDisable()
    {
        Calendar_Controller.OnDayChange -= GrowPlant;
    }

    void Awake()
    {
        mySprite = GetComponent<SpriteRenderer>();
        mySprite.sprite = without;
    }

    #region Soil State
    public void SetWater(bool state)
    {
        isWater = state;
        UpdateSprite();
    }

    public void PlantSeed(PlantType plant)
    {
        if (isPlanted) return;

        currentPlant = plant;
        isPlanted = true;
        days = 0;
        daysWithoutWater = 0;
        UpdateSprite();
    }
    #endregion

    #region Grow 
    private void GrowPlant()
    {
        if (!isPlanted || currentPlant == null) return;

        if (isWater && !dead)
        {
            days++;
            daysWithoutWater = 0;
            isWater = false;
        }
        else
        {
            if (daysWithoutWater > currentPlant.maxDaysWWithoutWater)
            {
                dead = true;
            }
            daysWithoutWater++;
        }

        UpdateSprite();
    }
    #endregion

    #region Visual
    private void UpdateSprite()
    {
        if (!isPlanted && !isWater)
        {
            mySprite.sprite = without;
            return;
        }
        else if (isWater && !isPlanted)
        {
            mySprite.sprite = withWater;
            return;
        }

        int stageIndex = -1;

        if (!dead)
        {
            if (days == currentPlant.growthTimeInDays)
            {
                stageIndex = 4;
            }
            else if (!isWater && days < currentPlant.growthTimeInDays / 2)
            {
                stageIndex = 0;
            }
            else if (isWater && days < currentPlant.growthTimeInDays / 2)
            {
                stageIndex = 1;
            }
            else if (!isWater && days >= currentPlant.growthTimeInDays / 2)
            {
                stageIndex = 2;
            }
            else if (isWater && days >= currentPlant.growthTimeInDays / 2)
            {
                stageIndex = 3;
            }
        }
        else
        {
            stageIndex = 5;
        }
        mySprite.sprite = currentPlant.growthStages[stageIndex];
    }
    #endregion

    #region Harvest
    public void Harvest()
    {
        Debug.Log($"Harvest: {currentPlant.harvest.harvestName}");
    }
    #endregion
}
