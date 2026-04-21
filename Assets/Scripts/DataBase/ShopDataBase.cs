using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "World/Shop Data Base")]
public class ShopDataBase : ScriptableObject
{
    public List<ShopData> shops = new List<ShopData>();


    public ShopData GetShopData(string id)
    {
        ShopData foundedShop = shops.Find(i => i.id == id);

        if(!foundedShop)
        {
            return null;
        }

        return foundedShop;
    }
}
