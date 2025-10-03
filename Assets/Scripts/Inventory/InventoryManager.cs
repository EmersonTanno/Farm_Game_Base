using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public GameObject inventoryItemPrefab;
    public InventorySlot[] inventorySlots;
    int selectedSlot = -1;
    [SerializeField] GameObject inventoryCanvas;
    [SerializeField] GameObject inventoryButton;
    private bool inventoryActive = false;

    void Awake()
    {
        Instance = this;    
    }

    void Start()
    {
        ChangeSelectedSlot(0);
    }

    #region InputSystem
    public void SetSlot(InputAction.CallbackContext value)
    {
        if (!value.performed) return;

        float input = value.ReadValue<float>();

        if (input > 0)
        {
            int nextSlot = (selectedSlot - 1 + 6) % 6;
            ChangeSelectedSlot(nextSlot);
        }
        else if (input < 0)
        {
            int nextSlot = (selectedSlot + 1) % 6;
            ChangeSelectedSlot(nextSlot);
        }
    }

    public void SetInventory(InputAction.CallbackContext value)
    {
        inventoryActive = !inventoryActive;
        inventoryCanvas.SetActive(inventoryActive);
        inventoryButton.SetActive(!inventoryActive);
    }

    public void SetInventoryButton()
    {
        inventoryActive = !inventoryActive;
        inventoryCanvas.SetActive(inventoryActive);
        inventoryButton.SetActive(!inventoryActive);
    }
    #endregion

    void ChangeSelectedSlot(int newValue)
    {
        if (selectedSlot >= 0)
        {
            inventorySlots[selectedSlot].Deselect();
        }

        inventorySlots[newValue].Select();
        selectedSlot = newValue;
    }

    public bool AddItem(Item item)
    {

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot.item != null &&
            itemInSlot.item == item &&
            itemInSlot.count < itemInSlot.item.maxStack &&
            itemInSlot.item.stackable)
            {
                itemInSlot.count++;
                itemInSlot.RefreshCount();
                return true;
            }
        }

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot.item == null)
            {
                SpawnNewItem(item, slot);
                return true;
            }
        }
        return false;
    }

    private void SpawnNewItem(Item item, InventorySlot slot)
    {
        InventoryItem inventoryItem = slot.GetComponentInChildren<InventoryItem>();
        inventoryItem.InitializeItem(item);
    }

    public Item UseSelectedItem()
    {
        InventorySlot slot = inventorySlots[selectedSlot];
        InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
        if (itemInSlot.item != null)
        {
            Item item = itemInSlot.item;
            if (item.consume == true)
            {
                itemInSlot.count--;
                if (itemInSlot.count <= 0)
                {
                    itemInSlot.RemoveItem();
                }
                else
                {
                    itemInSlot.RefreshCount();
                }
            }
            return item;
        }

        return null;
    }
}
