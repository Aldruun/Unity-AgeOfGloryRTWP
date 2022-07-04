using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Flags]
public enum InventorySlotFlags
{
    None = 0,
    PartySlot = 1,
    QuickSlot = 2,
    ArmorSlot = 4,
    WeaponSlot = 8,
    EquipmentSlot = ArmorSlot | WeaponSlot
}

public class UIInventorySlot : MonoBehaviour, /*IPointerDownHandler,*/ IPointerEnterHandler, IPointerExitHandler
{
    private InventorySlotFlags inventorySlotFlags;

    [ReadOnly] public InventoryItem InventoryItem;
    public Toggle EquippedToggle;
    public string Identifier;
    public int StackSize { get; set; }
    public int CellIndex;
    public RectTransform RectTransform;
    public System.Action<UIInventorySlot> OnCursorEnter;
    public System.Action<UIInventorySlot> OnCursorExit;

    private TMPro.TextMeshProUGUI stackSizeInfo;
    private Image icon;

    private void Awake()
    {

        icon = GetComponent<Image>();
        EquippedToggle = icon.transform.parent.GetComponentInChildren<Toggle>();
        stackSizeInfo = transform.parent.Find("Stack").GetComponent<TMPro.TextMeshProUGUI>();

        RectTransform = GetComponent<RectTransform>();

        SetStackSizeText(1);

        if(transform.parent.parent.name == "Party Slots")
        {
            inventorySlotFlags = InventorySlotFlags.PartySlot;
        }
        else if(transform.parent.parent.name == "Quick Slots")
        {
            inventorySlotFlags = InventorySlotFlags.QuickSlot;
        }
        else if(transform.parent.parent.name == "Equipment Slots")
        {
            inventorySlotFlags = InventorySlotFlags.EquipmentSlot;
        }

        Color c = icon.color;
        c.a = 0;
        icon.color = c;
    }

    private void OnDisable()
    {
        //if(UIInventory.startSlot != null)
        //{
        //    UIInventory.DisableGhostItem();
        //    UIInventory.startSlot.GetComponent<Image>().color = new Color(1, 1, 1, 1);
        //    UIInventory.startSlot = null;
        //    UIInventory.dragging = false;
        //    StopAllCoroutines();
        //}
    }

    internal void Init()
    {

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnCursorEnter?.Invoke(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnCursorExit?.Invoke(this);
    }

    //public void OnPointerDown(PointerEventData eventData)
    //{
    //}

    public void AddItem(InventoryItem item, int stackSize)
    {
        this.Identifier = item.itemData.identifier;

        this.InventoryItem = item; // ItemDatabase.GetItemFromJSON(identifier);
        InventoryItem.slotIndex = CellIndex;
        SetStackSizeText(stackSize);
        GetComponent<Image>().sprite = ResourceManager.GetItemSprite(item.itemData.identifier);
        if(GetComponent<Image>().sprite == null)
        {
            Debug.LogError("slot sprite null");
        }

        SetVisible(true);
    }

    public void ClearSlot()
    {
        SetStackSizeText(0);
        this.Identifier = "";
        this.InventoryItem = null;
        SetVisible(false);
        icon.sprite = null;
    }

    public void SetStackSizeText(int newStackSize)
    {
        StackSize = newStackSize;

        if(newStackSize < 2)
        {
            this.stackSizeInfo.text = "";
        }
        else
        {
            this.stackSizeInfo.text = newStackSize.ToString();
        }
    }

    public void AdjustStackSize(int amount)
    {

        int newValue = this.StackSize + amount;
        StackSize = newValue;
        this.stackSizeInfo.text = newValue.ToString();
    }

    public void SetVisible(bool on)
    {
        icon.color = new Color(1, 1, 1, on ? 1 : 0);
    }

    internal Image GetIcon()
    {
        return icon;
    }

    internal bool HasSlotFlag(InventorySlotFlags equipmentSlot)
    {
        return inventorySlotFlags.HasFlag(equipmentSlot);
    }
}
