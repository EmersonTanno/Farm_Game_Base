using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Dropped : MonoBehaviour
{
    private SpriteRenderer mySprite;

    [HideInInspector] public Item itemDrop;

    void Awake()
    {
        mySprite = GetComponent<SpriteRenderer>();
    }

    public void SetItem(Item item)
    {
        itemDrop = item;
        mySprite.sprite = itemDrop.image;
    }

    public void CollectItem()
    {
        InventoryManager.Instance.AddItem(itemDrop);
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
            CollectItem();
    }
}
