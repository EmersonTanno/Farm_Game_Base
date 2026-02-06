using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    #region Variables
    public GameObject inventoryItemPrefab;
    public InventorySlot[] inventorySlots;
    int selectedSlot = -1;
    [SerializeField] GameObject inventoryGroup;
    [SerializeField] GameObject inventoryCanvas;
    [SerializeField] GameObject inventoryButton;
    [SerializeField] TextMeshProUGUI itemNameText;
    [HideInInspector] public bool inventoryActive = false;
    private bool active = true;
    #endregion

    #region Core
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        ChangeSelectedSlot(0);
    }

        void OnEnable()
    {
        InventoryItem.OnItemDrop += ReloadSlot;
    }

    void OnDisable()
    {
        InventoryItem.OnItemDrop -= ReloadSlot;
    }
    #endregion

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

    #region Actions
    void ChangeSelectedSlot(int newValue)
    {
        if(!active || Shop_Manager.Instance.shopActive) return;
        if (selectedSlot >= 0)
        {
            inventorySlots[selectedSlot].Deselect();
        }

        inventorySlots[newValue].Select();
        selectedSlot = newValue;

        InventorySlot slot = inventorySlots[newValue];
        InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
        if (itemInSlot.item != null)
        {
            Item item = itemInSlot.item;
            itemNameText.text = item.itemName;
        }
        else
        {
            itemNameText.text = "";
        }
    }
    
    void ReloadSlot()
    {
        InventorySlot slot = inventorySlots[selectedSlot];
        InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
        if (itemInSlot.item != null)
        {
            Item item = itemInSlot.item;
            itemNameText.text = item.itemName;
        }
        else
        {
            itemNameText.text = "";
        }
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
            if (item.consume == true && item.type != ItemType.Harvest)
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

    public Item SellSelectedItem()
    {
        InventorySlot slot = inventorySlots[selectedSlot];
        InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
        if (itemInSlot.item != null)
        {
            Item item = itemInSlot.item;
            if (item.sell == true)
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
                return item;
            }
            return null;
        }

        return null;
    }
    #endregion

    #region Ui
    public void ControllInventoryGroup(bool setActive)
    {
        inventoryGroup.SetActive(setActive);
        active = setActive;
    }
    #endregion

    #region Save / Load
    public void Save(ref InventorySaveData data)
    {
        InventorySaveData newInventoryData = new InventorySaveData();

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();

            if (itemInSlot == null || itemInSlot.item == null)
                continue;

            ItemSaveData newItemData = new ItemSaveData
            {
                itemId = itemInSlot.item.id,
                quantity = itemInSlot.count,
                slot = i
            };

            newInventoryData.items.Add(newItemData);
        }

        data = newInventoryData;
    }

    public void Load(InventorySaveData data)
    {
        foreach(InventorySlot slot in inventorySlots)
        {
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            itemInSlot.RemoveItem();
            itemInSlot.count = 0;
            itemInSlot.RefreshCount();
        }
        foreach(ItemSaveData item in data.items)
        {
            InventorySlot slot = inventorySlots[item.slot];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();

            SpawnNewItem(ItemDataBaseController.Instance.GetItemById(item.itemId), slot);
            itemInSlot.count = item.quantity;
            itemInSlot.RefreshCount();
        }
    }
    #endregion
}
