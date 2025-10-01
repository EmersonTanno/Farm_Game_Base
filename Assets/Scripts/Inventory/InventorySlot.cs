using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        DraggableItem draggableItem = dropped.GetComponent<DraggableItem>();

        GameObject current = transform.GetChild(0).gameObject;
        DraggableItem currentDraggable = current.GetComponent<DraggableItem>();

        currentDraggable.transform.SetParent(draggableItem.parentAfterDrag);
        draggableItem.parentAfterDrag = transform;
        
    }
}
