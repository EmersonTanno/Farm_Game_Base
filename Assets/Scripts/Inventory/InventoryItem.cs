using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [HideInInspector] public Item item = null;
    public UnityEngine.UI.Image countBackground;
    public TextMeshProUGUI countText;

    [HideInInspector] public UnityEngine.UI.Image image;
    [HideInInspector] public int count = 1;
    [HideInInspector] public Transform parentAfterDrag;

    void Awake()
    {
        RefreshCount();
    }

    public void InitializeItem(Item newItem)
    {
        item = newItem;
        image.sprite = newItem.image;
        RefreshCount();
    }

    public void RemoveItem()
    {
        image.sprite = null;
        item = null;
    }

    public void RefreshCount()
    {
        countText.text = count.ToString();
        bool textActive = count > 1;
        countBackground.gameObject.SetActive(textActive);
        countText.gameObject.SetActive(textActive);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        image.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(parentAfterDrag);
        transform.localPosition = Vector3.zero;
        image.raycastTarget = true;
    }


}