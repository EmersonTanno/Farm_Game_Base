using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Item))]
public class ItemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Item item = (Item)target;

        // --- Sempre visíveis ---
        item.itemName = EditorGUILayout.TextField("Item Name", item.itemName);
        item.type = (ItemType)EditorGUILayout.EnumPopup("Type", item.type);
        item.image = (Sprite)EditorGUILayout.ObjectField("Image", item.image, typeof(Sprite), false);
        item.stackable = EditorGUILayout.Toggle("Stackable", item.stackable);
        item.consume = EditorGUILayout.Toggle("Consume", item.consume);

        // --- Se stackable for true ---
        if (item.stackable)
        {
            item.maxStack = EditorGUILayout.IntField("Max Stack", item.maxStack);
        }

        // --- Lógica por tipo ---
        switch (item.type)
        {
            case ItemType.Tool:
                item.action = (ActionType)EditorGUILayout.EnumPopup("Action", item.action);
                break;

            case ItemType.Seed:
                item.plant = (PlantType)EditorGUILayout.ObjectField("Plant", item.plant, typeof(PlantType), false);
                item.sell = EditorGUILayout.Toggle("Can Sell", item.sell);
                item.buyValue = EditorGUILayout.IntField("Buy Value", item.buyValue);
                item.sellValue = EditorGUILayout.IntField("Sell Value", item.sellValue);
                break;


            case ItemType.Harvest:
                item.sell = EditorGUILayout.Toggle("Can Sell", item.sell);
                item.buyValue = EditorGUILayout.IntField("Buy Value", item.buyValue);
                item.sellValue = EditorGUILayout.IntField("Sell Value", item.sellValue);
                break;
        }

        // --- Atualizar caso algo mude ---
        if (GUI.changed)
        {
            EditorUtility.SetDirty(item);
        }
    }
}
