using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Dropped : MonoBehaviour
{
    private SpriteRenderer mySprite;

    private Item itemDrop;

    void Awake()
    {
        mySprite = GetComponent<SpriteRenderer>();
    }

    public void SetItem(Item item)
    {
        itemDrop = item;
        mySprite.sprite = itemDrop.image;
    }
}
