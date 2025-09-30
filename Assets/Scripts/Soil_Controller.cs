using UnityEngine;

public class Soil_Controller : MonoBehaviour
{
    #region Variables
    [SerializeField] private Sprite without;
    [SerializeField] Sprite withWater;

    private SpriteRenderer mySprite;
    private bool isPlanted = false;
    private bool isWater = false;
    private int days = 0;
    private int daysWithoutWater = 0;

    private PlantType currentPlant;
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

        if (isWater)
        {
            days++;
            daysWithoutWater = 0;
            isWater = false;
        }
        else
        {
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

        if (!isWater)
        {
            stageIndex = 0;
        }
        else if (isWater)
        {
            stageIndex = 1;
        }
        mySprite.sprite = currentPlant.growthStages[stageIndex];
    }
    #endregion
}
