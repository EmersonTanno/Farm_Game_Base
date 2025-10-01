using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        InventoryItem draggableItem = dropped.GetComponent<InventoryItem>();

        GameObject current = transform.GetChild(0).gameObject;
        InventoryItem currentDraggable = current.GetComponent<InventoryItem>();

        currentDraggable.transform.SetParent(draggableItem.parentAfterDrag);
        draggableItem.parentAfterDrag = transform;
        
    }
}
