using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soil_Controller : MonoBehaviour
{
    [SerializeField] Sprite withWater;
    [SerializeField] Sprite withPlant;
    [SerializeField] Sprite withPlantWater;
    [SerializeField] Sprite without;

    private bool isPlanted = false;
    private bool isWater = false;
    private SpriteRenderer mySprite;

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
}
