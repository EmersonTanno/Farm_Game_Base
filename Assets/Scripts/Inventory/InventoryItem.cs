using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [HideInInspector] public Item item = null;
    public UnityEngine.UI.Image countBackground;
    public TextMeshProUGUI countText;

    [HideInInspector] public UnityEngine.UI.Image image;
    [HideInInspector] public int count = 1;
    [HideInInspector] public Transform parentAfterDrag;

    private bool isDragging;

    public static event Action OnItemDrop;

    void Awake()
    {
        RefreshCount();
    }

    public void InitializeItem(Item newItem)
    {
        item = newItem;
        image.sprite = newItem.image;
        count = 1;
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
        if(Time_Controll.Instance.timerPaused) return;
        isDragging = true;
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.parent.parent.parent.parent);
        transform.SetAsLastSibling();
        image.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(!isDragging) return;

        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(!isDragging) return;
        
        isDragging = false;
        transform.SetParent(parentAfterDrag);
        transform.localPosition = Vector3.zero;
        image.raycastTarget = true;
        OnItemDrop?.Invoke();
    }


}