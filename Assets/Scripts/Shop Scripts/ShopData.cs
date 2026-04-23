using UnityEngine;

[CreateAssetMenu]
public class ShopData : ScriptableObject
{
    public string id;
    [SerializeField] public Item[] shopItems;
}