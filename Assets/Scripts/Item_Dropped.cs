using System;
using Dustopia.RewardSystem;
using UnityEngine;

public class Item_Dropped : MonoBehaviour
{
    private SpriteRenderer mySprite;

    [HideInInspector] public Item itemDrop;

    public static event Action<Item> OnItemPick;

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
        OnItemPick?.Invoke(itemDrop);
        ObjectPool.Instance.ReturnObject(gameObject);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
            CollectItem();
    }
}
