using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Image = UnityEngine.UI.Image;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public Image image;
    public Color selectedColor, notSelectedColor;

    void Awake()
    {
        Deselect();
    }

    public void Select()
    {
        image.color = selectedColor;
    }

    public void Deselect()
    {
        image.color = notSelectedColor;
    }

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
