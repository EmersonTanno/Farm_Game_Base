using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soil_Controller : MonoBehaviour
{
    #region Variables
    [SerializeField] Sprite withWater;
    [SerializeField] Sprite withPlant;
    [SerializeField] Sprite withPlantWater;
    [SerializeField] Sprite without;

    private bool isPlanted = false;
    private bool isWater = false;
    private int days = 0;
    private int daysWhithoutWater = 0;
    private SpriteRenderer mySprite;
    #endregion

    #region Core
    void Awake()
    {
        mySprite = GetComponent<SpriteRenderer>();
    }

    void Start()
    {

    }

    void Update()
    {

    }
    #endregion

    #region Animation
    public void SetSprite()
    {
        if (isWater && isPlanted)
        {
            mySprite.sprite = withPlantWater;
        }
        else if (isWater)
        {
            mySprite.sprite = withWater;
        }
        else if (isPlanted)
        {
            mySprite.sprite = withPlant;
        }
        else
        {
            mySprite.sprite = without;
        }
    }
    #endregion

    #region Soil State
    public void SetWater(bool state)
    {
        isWater = state;
        SetSprite();
    }

    public void SetPlanted(bool state)
    {
        isPlanted = state;
        SetSprite();
    }
    #endregion

    #region Grow 
    public void GrowPlant()
    {
        if (isPlanted && isWater)
        {
            days++;
            daysWhithoutWater = 0;
        }
        else if (isPlanted)
        {
            daysWhithoutWater++;
        }
    }
    #endregion
}
