using UnityEngine;

[CreateAssetMenu]
public class ShopData : ScriptableObject
{
    public string id;
    [SerializeField] Item[] shopItems;
}