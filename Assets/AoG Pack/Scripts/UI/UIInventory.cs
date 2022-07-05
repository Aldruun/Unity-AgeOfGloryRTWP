using AoG.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIInventory : MonoBehaviour, IPointerDownHandler
{
    public bool debug;
    public static InventorySlotFlags SlotTypeBelow;
    public static Inventory PlayerInventory;
    public static UIInventory Instance;
    public static GhostItem ItemGhostPreview;
    public static bool Dragging;

    private RectTransform inventoryPanel;
    private Transform quickbarSlotHolder;
    private Transform partySlotHolder;
    private Transform equipmentSlotHolder;
    private Transform stackSplitPanel;

    private Slider _splitSlider;
    private TMPro.TextMeshProUGUI splitNum;
    private TMPro.TextMeshProUGUI splitMax;
    private TMPro.TextMeshProUGUI splitItemName;
    public static bool splitting;

    public static UIInventorySlot slotBelow;
    public static UIInventorySlot startSlot;
    private static Dictionary<int, UIInventorySlot> slots;
    private static List<UIInventorySlot> quickSlots;
    private static List<UIInventorySlot> equipmentSlots;
    //static UIInventorySlot helmetSlot;
    //static UIInventorySlot capeSlot;
    //static UIInventorySlot necklaceSlot;
    //static UIInventorySlot dressSlot;
    //static UIInventorySlot glovesSlot;
    //static UIInventorySlot ringLeftSlot;
    //static UIInventorySlot ringRightSlot;
    //static UIInventorySlot beltSlot;
    //static UIInventorySlot bootsSlot;

    //public static Item equippedWeapon;
    public static InventoryItem equippedArrowSlot;

    public float mainPanelScrollTime = 0.2f;
    private static bool _uiScrolling;

    private static Camera _previewCamera;
    //static ActorInput _selectedHero;

    public bool Activated => inventoryPanel.GetComponent<RectTransform>().anchoredPosition.y <= 1;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Debug.LogError("UIInventory: Only one instance allowed");
            Debug.LogError("Destroyed Component from gameobject '" + gameObject.name + "'");
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        //partyMemberEquipment = new Dictionary<ActorInput, Equipment>();

        _previewCamera = GameObject.FindWithTag("InventoryPreviewCamera").GetComponent<Camera>();
        _previewCamera.enabled = false;

        ItemGhostPreview = new GhostItem();
        ItemGhostPreview.image = transform.Find("Drag Preview").GetComponent<Image>();
        ItemGhostPreview.Init(ItemGhostPreview.image.GetComponentInChildren<TMPro.TextMeshProUGUI>());
        //GameStateManager.Instance.OnInitInventories -= Init;
        //GameStateManager.Instance.OnInitInventories += Init;
        //GameEventSystem.RefreshUI_PlayerPickedUpItem -= AddNewItem;
        //GameEventSystem.RefreshUI_PlayerPickedUpItem += AddNewItem;
        GameEventSystem.OnPartyMemberSelected -= SetInventoryOwner;
        GameEventSystem.OnPartyMemberSelected += SetInventoryOwner;
        //GameEventSystem.OnPartyMemberAdded -= OnPartyMemberAdded;
        //GameEventSystem.OnPartyMemberAdded += OnPartyMemberAdded;
        //GameEventSystem.OnPlayerItemAdded += AddItem;
        //GameEventSystem.OnPlayerItemRemoved += RemoveItem;
        //GameEventSystem.OnPlayerDropItem += DropItem;
        Init();
    }

    private void OnDisable()
    {
        //GameStateManager.Instance.OnInitInventories -= Init;
        //GameEventSystem.RefreshUI_PlayerPickedUpItem -= AddNewItem;
        GameEventSystem.OnPartyMemberSelected -= SetInventoryOwner;
        //GameEventSystem.OnPartyMemberAdded -= OnPartyMemberAdded;
        //GameEventSystem.OnPlayerItemAdded -= AddItem;
        //GameEventSystem.OnPlayerItemRemoved -= RemoveItem;
        //GameEventSystem.OnPlayerDropItem -= DropItem;
    }

    private void Init()
    {
        Debug.Log("<color=grey>UIInventory Init()</color>");

        //playerInventory = GameObject.Find("_Player Components").GetComponentInChildren<Inventory>();

        inventoryPanel = transform.Find("Inventory Panel").GetComponent<RectTransform>();

        Vector2 pos = inventoryPanel.anchoredPosition;
        pos.y = 435;
        inventoryPanel.anchoredPosition = pos;

        partySlotHolder = inventoryPanel.Find("Party Slots");
        quickbarSlotHolder = inventoryPanel.Find("Quick Slots");
        equipmentSlotHolder = inventoryPanel.Find("Equipment Slots");
        inventoryPanel.gameObject.SetActive(false);
        stackSplitPanel = inventoryPanel.Find("Stack Split Panel");
        _splitSlider = stackSplitPanel.GetComponentInChildren<Slider>();
        _splitSlider.minValue = 1;
        splitNum = stackSplitPanel.Find("Num").GetComponent<TMPro.TextMeshProUGUI>();
        splitMax = stackSplitPanel.Find("Max").GetComponent<TMPro.TextMeshProUGUI>();
        splitItemName = stackSplitPanel.Find("Name").GetComponent<TMPro.TextMeshProUGUI>();

        HideSplitSlider();

        InitUISlots();

        //foreach(var item in inventory.items)
        //{
        //    current.AddItem(item);
        //}
    }

    private void InitUISlots()
    {
        slots = new Dictionary<int, UIInventorySlot>();
        quickSlots = new List<UIInventorySlot>();
        equipmentSlots = new List<UIInventorySlot>();

        int index = 0;
        foreach(Transform itemSlot in partySlotHolder)
        {
            UIInventorySlot slotUI = itemSlot.Find("Icon").GetComponent<UIInventorySlot>();
            //if(uiItem == null)
            //    Debug.LogError("null");

            slotUI.CellIndex = index;
            //uiItem.ClearItem();
            slots.Add(index, slotUI);
            index++;

            slotUI.Init();
            slotUI.OnCursorEnter = OnCursorEntersSlot;
            slotUI.OnCursorExit = OnCursorExistsSlot;
        }
        if(Instance.debug)
        {
            Debug.Log("<color=grey>" + index + " party slots found</color>");
        }

        index = 0;
        foreach(Transform equipmentSlot in equipmentSlotHolder)
        {
            UIInventorySlot slotUI = equipmentSlot.Find("Icon").GetComponent<UIInventorySlot>();

            slotUI.CellIndex = index;
            //uiItem.ClearItem();
            equipmentSlots.Add(slotUI);
            index++;

            slotUI.Init();
        }
        if(Instance.debug)
        {
            Debug.Log("<color=grey>" + index + " equipment slots found</color>");
        }

        index = 0;
        foreach(Transform quickbarSlot in quickbarSlotHolder)
        {
            UIInventorySlot slotUI = quickbarSlot.Find("Icon").GetComponent<UIInventorySlot>();
            slotUI.CellIndex = index;
            //uiItem.ClearItem();
            quickSlots.Add(slotUI);
            index++;

            slotUI.Init();
        }
        if(Instance.debug)
        {
            Debug.Log("<color=grey>" + index + " quick slots found</color>");
        }
    }
    private void OnCursorEntersSlot(UIInventorySlot slot)
    {
        //SlotTypeBelow = slot.InventorySlotFlags;
        slotBelow = slot;
    }

    private void OnCursorExistsSlot(UIInventorySlot obj)
    {
        SlotTypeBelow = InventorySlotFlags.None;
        slotBelow = null;
    }


    /// <summary>
    /// Is assigned to the global PartyMemberAdded event and is used for the UIInventory to populate new equipment data for the hero
    /// </summary>
    /// <param name="newMember"></param>
    //void OnPartyMemberAdded(ActorInput newMember)
    //{
    //    if(Instance.debug)
    //        Debug.Log("<color=grey>UIInventory OnPartyMemberAdded</color>");

    //    Equipment newEquipment = new Equipment(new InventoryItem[9], new InventoryItem[3]);

    //    ActorGearData gearData = newMember.Equipment;

    //    if(gearData.equippedHeadwear.Value != null)
    //    {
    //        newEquipment.SetEquipment(gearData.equippedHeadwear.Key, out InventoryItem none);
    //    }
    //    if(gearData.equippedCape.Value != null)
    //    {
    //        newEquipment.SetEquipment(gearData.equippedCape.Key, out InventoryItem none);
    //    }
    //    if(gearData.equippedNecklace != null)
    //    {
    //        newEquipment.SetEquipment(gearData.equippedNecklace, out InventoryItem none);
    //    }
    //    if(gearData.equippedDress.Value != null)
    //    {
    //        newEquipment.SetEquipment(gearData.equippedDress.Key, out InventoryItem none);
    //    }
    //    if(gearData.equippedRingLeft != null)
    //    {
    //        newEquipment.SetEquipment(gearData.equippedRingLeft, out InventoryItem none);
    //    }
    //    if(gearData.equippedRingRight != null)
    //    {
    //        newEquipment.SetEquipment(gearData.equippedRingRight, out InventoryItem none);
    //    }
    //    if(gearData.equippedBelt != null)
    //    {
    //        newEquipment.SetEquipment(gearData.equippedBelt, out InventoryItem none);
    //    }
    //    if(gearData.equippedBoots.Value != null)
    //    {
    //        newEquipment.SetEquipment(gearData.equippedBoots.Key, out InventoryItem none);
    //    }

    //    //partyMemberEquipment.Add(newMember, newEquipment);
    //}
    private void Update()
    {

        if(Input.GetMouseButtonDown(0))
        {
            //switch(SlotTypeBelow)
            //{
            //    case InventorySlotFlags.None:
            //        break;

            //    case InventorySlotFlags.PartySlot:

            //        break;
            //    case InventorySlotFlags.QuickSlot:

            //        break;
            //    case InventorySlotFlags.EquipmentSlot:

            //        break;
            //}

            if(slotBelow != null)
            {
                if(Dragging == false)
                {

                    //InventoryItem invItemUnderCursor = playerInventory.inventoryItems.Where(i => i.slotIndex == slotBelow.cellIndex).FirstOrDefault();

                    //if(invItemUnderCursor == null)
                    //{
                    //    return;
                    //}

                    if(slotBelow.InventoryItem != null)
                    {
                        startSlot = slotBelow;
                        if(Input.GetKey(KeyCode.LeftShift))
                        {
                            if(slotBelow.StackSize > 1) //! Split
                            {
                                ShowSplitSlider(slotBelow.InventoryItem.itemData.Name, slotBelow.StackSize);
                                splitting = true;
                            }
                            else
                            {
                            }
                        }
                        else if(Input.GetKey(KeyCode.LeftControl)) //! Click drop item into world
                        {
                            //dragging = false;
                            GameEventSystem.OnPlayerDropItem?.Invoke(slotBelow.InventoryItem.itemData.identifier, slotBelow.StackSize);
                            DropItem(slotBelow.InventoryItem, slotBelow.StackSize);
                            startSlot.ClearSlot();
                            DisableGhostItem();
                            slotBelow = null;
                        }
                        else //! Lift item
                        {
                            EnableGhostItem(slotBelow, slotBelow.GetIcon(), slotBelow.StackSize, false);
                            startSlot.GetIcon().color = new Color(0.5f, 0.5f, 0.5f, 1);
                        }
                    }
                }
                else if(Dragging) //! Place object in slot below
                {
                    if(slotBelow.InventoryItem != null && slotBelow.InventoryItem.itemData.identifier == ItemGhostPreview.inventoryItem.itemData.identifier)
                    {
                        Debug.LogError("Item below identical");
                        RevertDragAction();
                        return;
                    }

                    if(slotBelow.HasSlotFlag(InventorySlotFlags.EquipmentSlot))
                    {
                        if(ItemGhostPreview.inventoryItem.itemData.equippable == false)
                        {
                            return;
                        }
                        else
                        {
                            if(ItemGhostPreview.inventoryItem.itemData is Armor armor)
                            {
                                if(slotBelow.CellIndex != (int)armor.bodySlot) //! Not the correct armor slot
                                {
                                    return;
                                }
                                else //! Equip armor
                                {
                                    ToggleEquipArmor(armor, slotBelow, startSlot, true);
                                }
                            }
                        }
                    }
                    else if(startSlot.HasSlotFlag(InventorySlotFlags.EquipmentSlot))
                    {
                        if(slotBelow.HasSlotFlag(InventorySlotFlags.PartySlot)) //! Drop from equipment slot to party slot
                        {
                            if(slotBelow.InventoryItem != null)
                            {
                                if(slotBelow.InventoryItem.itemData.equippable && slotBelow.InventoryItem.itemData is Armor a) //! Party slot below also has armor
                                {
                                    //TODO Swap armor if hero can wear it

                                    if(ItemGhostPreview.inventoryItem.itemData is Armor armor) //! Identical armor slot type?
                                    {
                                        if(a.bodySlot == armor.bodySlot)
                                        {
                                            ToggleEquipArmor(a, startSlot, slotBelow, true);

                                            //ToggleEquipArmor(armor, startSlot, slotBelow, true);
                                            RevertDragAction();
                                        }

                                    }
                                    else
                                    {
                                        Debug.LogError("Item below has wrong armor slot type");
                                        RevertDragAction();
                                        return;
                                    }
                                }
                                else
                                {
                                    Debug.LogError("Item below is not of type armor");
                                    RevertDragAction();
                                    return;
                                }
                            }
                            else
                            {
                                //TODO Drop item
                            }
                        }

                        //if(itemGhostPreview.inventoryItem.itemData.equippable == false)
                        //{
                        //    Debug.LogError("Dragged item not equippable");
                        //    return;
                        //}
                        //else
                        //{


                        //}
                    }

                    if(slotBelow.InventoryItem == null) //! Dragging, slot below EMPTY
                    {
                        slotBelow.InventoryItem = ItemGhostPreview.inventoryItem;
                        slotBelow.StackSize = ItemGhostPreview.stackSize;
                        //Debug.Log("Adding sprite");
                        slotBelow.GetIcon().sprite = ItemGhostPreview.image.sprite;
                        slotBelow.GetIcon().color = new Color(1, 1, 1, 1);
                        slotBelow.SetStackSizeText(ItemGhostPreview.stackSize);
                        slotBelow.AddItem(ItemGhostPreview.inventoryItem, ItemGhostPreview.stackSize);
                        //SetSlotData(slotBelow.inventorySlotType, slotBelow.cellIndex, itemGhostPreview.itemData, itemGhostPreview.stackSize);

                        if(ItemGhostPreview.isSplitStack == false || (ItemGhostPreview.isSplitStack && ItemGhostPreview.stackSize == startSlot.StackSize))
                        {
                            ClearSlotData(startSlot);
                        }
                        else //! Split stack on empty slot
                        {
                            slotBelow.AddItem(ItemGhostPreview.inventoryItem, startSlot.StackSize - ItemGhostPreview.stackSize);
                            startSlot.GetIcon().color = new Color(1, 1, 1, 1);
                            //SetSlotData(slotBelow.cellIndex, itemGhostPreview.itemData, itemGhostPreview.stackSize);
                        }

                        DisableGhostItem();
                    }
                    else //! Dragging, slot below FILLED
                    {
                        if(slotBelow == startSlot)
                        {
                            RevertDragAction();
                            return;
                        }

                        if(ItemGhostPreview.isSplitStack)//! Splitting procedure
                        {
                            if(ItemGhostPreview.inventoryItem.itemData.identifier != slotBelow.InventoryItem.itemData.identifier)
                            {
                                return;
                            }
                            else //! Split stack on slot with same item
                            {
                                int surplus = ItemGhostPreview.stackSize + slotBelow.StackSize - slotBelow.InventoryItem.itemData.maxStackSize;
                                if(surplus > 0) //! Only part of ghost stack fits into this slot
                                {
                                    ItemGhostPreview.SetStackSize(surplus);
                                    SetStackSize(startSlot.CellIndex, startSlot.StackSize - surplus);
                                    SetStackSizeToMax(slotBelow.CellIndex);
                                }
                                else if(surplus <= 0)
                                {
                                    if(slotBelow != startSlot && startSlot.StackSize == ItemGhostPreview.stackSize)
                                    {
                                        ClearSlotData(startSlot);
                                    }
                                    else
                                    {
                                        slots[startSlot.CellIndex].GetIcon().color = new Color(1, 1, 1, 1);
                                    }

                                    SetStackSize(slotBelow.CellIndex, slotBelow.StackSize + ItemGhostPreview.stackSize);
                                    SetStackSize(startSlot.CellIndex, slotBelow.StackSize - ItemGhostPreview.stackSize);

                                    DisableGhostItem();
                                }
                            }
                        }
                        //else //! Normal procedure
                        //{
                        //    if(itemGhostPreview.inventoryItem.itemData.identifier != slotBelow.inventoryItem.itemData.identifier || slotBelow.stackSize == slotBelow.inventoryItem.itemData.maxStackSize) //! Swap items
                        //    {


                        //        startSlot.GetIcon().sprite = slotBelow.GetIcon().sprite;
                        //        startSlot.GetIcon().color = new Color(1, 1, 1, 1);
                        //        startSlot.AddItem(slotBelow.inventoryItem, slotBelow.stackSize);

                        //        slotBelow.GetIcon().sprite = itemGhostPreview.image.sprite;
                        //        slotBelow.GetIcon().color = new Color(1, 1, 1, 1);
                        //        slotBelow.AddItem(itemGhostPreview.inventoryItem, itemGhostPreview.stackSize);

                        //        DisableGhostItem();
                        //    }
                        //    else
                        //    {
                        //        int surplus = (itemGhostPreview.stackSize + slotBelow.stackSize) - slotBelow.inventoryItem.itemData.maxStackSize;
                        //        if(surplus > 0) //! Only part of ghost stack fits into this slot
                        //        {
                        //            itemGhostPreview.SetStackSize(surplus);
                        //            SetStackSize(startSlot.cellIndex, surplus);
                        //            SetStackSizeToMax(slotBelow.cellIndex);
                        //        }
                        //        else if(surplus <= 0)
                        //        {
                        //            if(startSlot.stackSize == itemGhostPreview.stackSize)
                        //            {
                        //                ClearSlotData(startSlot);
                        //            }

                        //            //SetSlotData(dataSlotBelow.inventorySlotType, dataSlotBelow.cellIndex, dataSlotBelow.itemData, dataSlotBelow.stackSize + itemGhostPreview.stackSize);
                        //            startSlot.AddItem(slotBelow.inventoryItem, slotBelow.stackSize + itemGhostPreview.stackSize);
                        //            //itemGhostPreview.SetStackSize(surplus);
                        //            DisableGhostItem();
                        //        }
                        //    }
                        //}
                    }
                }
            }
            else
            {
                if(Dragging && IsPointerOverUIObject() == false)
                {
                    if(ItemGhostPreview.isSplitStack)
                    {
                        if(startSlot.StackSize == ItemGhostPreview.stackSize)
                        {
                            ClearSlotData(startSlot);
                        }
                        else
                        {
                            SetStackSize(startSlot.CellIndex, startSlot.StackSize - ItemGhostPreview.stackSize);
                            startSlot.GetIcon().color = new Color(1, 1, 1, 1);
                        }

                        GameEventSystem.OnPlayerDropItem?.Invoke(ItemGhostPreview.inventoryItem.itemData.identifier, ItemGhostPreview.stackSize);
                        DropItem(ItemGhostPreview.inventoryItem, ItemGhostPreview.stackSize);
                    }
                    else
                    {
                        GameEventSystem.OnPlayerDropItem?.Invoke(ItemGhostPreview.inventoryItem.itemData.identifier, startSlot.StackSize);
                        DropItem(ItemGhostPreview.inventoryItem, startSlot.StackSize);
                        ClearSlotData(startSlot);
                    }

                    DisableGhostItem();
                }
            }
        }
        else if(Input.GetMouseButtonDown(1))
        {
            if(Dragging == false)
            {
                if(slotBelow != null)
                {
                    if(slotBelow.InventoryItem != null/* && dataSlotBelow.itemData.equippable*/)
                    {
                        if(slotBelow.InventoryItem.itemData is Armor armor)
                        {
                            //Debug.Log("Click armor");

                            if(slotBelow.HasSlotFlag(InventorySlotFlags.EquipmentSlot))
                            {
                                //Debug.Log("Unequip");
                                UIInventorySlot uiSlot = slots.Where(kvp => kvp.Value.InventoryItem == null).FirstOrDefault().Value;

                                ToggleEquipArmor(armor, uiSlot, slotBelow, false);

                                //uiSlot.AddItem(slotBelow.inventoryItem, 1);
                                //playerInventory.AddItem(slotBelow.identifier, 1);
                                //Debug.Log("Destroying armor of type " + armor.bodySlot.ToString());
                                //partyMemberEquipment[_selectedHero].ClearEquipmentSlot(armor.bodySlot);
                                //_selectedHero.Execute_UnequipArmor(armor);
                                //slotBelow.ClearSlot();
                            }
                            else if(slotBelow.HasSlotFlag(InventorySlotFlags.PartySlot))
                            {
                                UIInventorySlot equipmentSlot = equipmentSlots[(int)armor.bodySlot];
                                //EquipArmor(equipmentSlot, slotBelow, armor);
                                ToggleEquipArmor(armor, equipmentSlot, slotBelow, true);

                                //InventoryItem swappedArmor;
                                ////InventorySlot heroTargetSlot = partyMemberEquipment[_selectedHero].GetEquipmentSlots()[(int)armor.bodySlot];
                                //partyMemberEquipment[_selectedHero].SetEquipment(armor, out swappedArmor);
                                //equipmentSlot.AddItem(slotBelow.inventoryItem, 1);
                                //playerInventory.inventoryItems.Remove(slotBelow.inventoryItem);
                                //if(swappedArmor != null)
                                //{
                                //    slotBelow.AddItem(swappedArmor, 1);
                                //}
                                //else
                                //{
                                //    slotBelow.ClearSlot();
                                //}

                                //_selectedHero.Execute_EquipArmor(armor);
                            }
                        }
                        else
                        {
                            Debug.LogError("ItemData not armor");
                        }
                    }
                    else
                    {
                        Debug.LogError("ItemData null");
                    }
                }
            }
        }
    }

    private static void RevertDragAction()
    {
        startSlot.GetIcon().color = new Color(1, 1, 1, 1);
        DisableGhostItem();
        startSlot = null;
    }

    private static void ToggleEquipArmor(Armor armor, UIInventorySlot newSlot, UIInventorySlot oldSlot, bool on)
    {
        if(on)
        {
            //EquipArmor(equipmentSlot, slotBelow, armor);

            ArmorData swappedArmor;
            //InventorySlot heroTargetSlot = partyMemberEquipment[_selectedHero].GetEquipmentSlots()[(int)armor.bodySlot];
            SelectionManager.selected[0].Equipment.EquipmentSlots.EquipArmor(armor, out swappedArmor);
            newSlot.AddItem(oldSlot.InventoryItem, 1);
            _ = PlayerInventory.inventoryItems.Remove(oldSlot.InventoryItem);
            if(swappedArmor != null)
            {
                oldSlot.AddItem(new InventoryItem(swappedArmor.Armor), 1);
            }
            else
            {
                oldSlot.ClearSlot();
            }


            SelectionManager.selected[0].Equipment.EquipArmor(armor);
        }
        else
        {
            //Debug.Log("Unequip");
            UIInventorySlot uiSlot = slots.Where(kvp => kvp.Value.InventoryItem == null).FirstOrDefault().Value;
            uiSlot.AddItem(slotBelow.InventoryItem, 1);
            PlayerInventory.AddItem(slotBelow.Identifier, 1);
            if(Instance.debug)
            {
                Debug.Log("Destroying armor of type " + armor.bodySlot.ToString());
            }

            SelectionManager.selected[0].Equipment.EquipmentSlots.ClearArmorSlot(armor.bodySlot);
            slotBelow.ClearSlot();


            SelectionManager.selected[0].Combat.Execute_UnequipArmor(armor);
        }
    }

    private static void PopulateEquipmentAndQuickSlots()
    {
        if(Instance.debug)
        {
            Debug.Log("Repopulating equipment slots");
        }

        Equipment equipment = SelectionManager.selected[0].Equipment.EquipmentSlots;

        if(equipment == null)
        {
            Debug.LogError("Either no pc selected or actor not in party");
        }

        for(int i = 0; i < equipmentSlots.Count; i++)
        {
            if(Instance.debug)
            {
                Debug.Log("Iterating equipment slots");
            }

            equipmentSlots[i].ClearSlot();
            if(equipment.GetArmorSlots()[i].Armor != null)
            {
                if(Instance.debug)
                {
                    Debug.Log("Found armor slot");
                }

                equipmentSlots[i].AddItem(new InventoryItem(equipment.GetArmorSlots()[i].Armor), 1);
            }
        }
        for(int i = 0; i < quickSlots.Count; i++)
        {
            if(Instance.debug)
            {
                Debug.Log("Iterating quick slots");
            }

            quickSlots[i].ClearSlot();
            if(equipment.GetQuickSlots()[i].itemData != null)
            {
                if(Instance.debug)
                {
                    Debug.Log("Found quick slot");
                }

                quickSlots[i].AddItem(equipment.GetQuickSlots()[i], 1);
            }
        }
    }

    /// <summary>
    /// Called when showing the UIInventory to make all items visible on the appropriate slots
    /// </summary>
    /// <param name="inventory"></param>
    public static void PopulateUISlotsFromInventory(Inventory inventory)
    {
        if(inventory.inventoryItems == null)
        {
            if(Instance.debug)
            {
                Debug.LogError("null");
            }
        }

        foreach(var slot in slots)
        {
            slot.Value.ClearSlot();
        }
        foreach(var slot in equipmentSlots)
        {
            slot.ClearSlot();
        }
        foreach(var slot in quickSlots)
        {
            slot.ClearSlot();
        }

        PopulateEquipmentAndQuickSlots();
        List<InventoryItem> newItems = new List<InventoryItem>();
        foreach(InventoryItem invItem in inventory.inventoryItems)
        {
            UIInventorySlot uiSlot = null;

            if(invItem.slotIndex == -1) //Hack New Item - get first free slot
            {
                newItems.Add(invItem);
                continue;
            }

            uiSlot = slots[invItem.slotIndex];

            Image icon = uiSlot/*.transform.parent.Find("Icon")*/.GetComponent<Image>();
            if(Instance.debug)
            {
                Debug.Log("Populating inventory UI with " + invItem.stackSize + " x " + invItem.ID);
            }

            if(invItem.itemData != null)
            {
                //AddNewItem(invItem.ID, invItem.stackSize);

                slots[invItem.slotIndex].AddItem(invItem, invItem.stackSize);
                //icon.sprite = ResourceManager.GetSprite(slot.itemData.identifier);

                //Color c = icon.color;
                //c.a = 1;
                //icon.color = c;

                //uiSlot.SetStackSizeText(slot.stackSize);

            }
            else
            {
                Debug.LogError("Item may not be null at this point");
                //icon.sprite = null;

                //Color c = icon.color;
                //c.a = 0;
                //icon.color = c;

                //uiSlot.SetStackSizeText(0);
            }
        }

        foreach(InventoryItem invItem in newItems)
        {
            UIInventorySlot uiSlot = slots.Where(kvp => kvp.Value.InventoryItem == null).FirstOrDefault().Value;
            uiSlot.AddItem(invItem, invItem.stackSize);
        }
    }

    private void SetStackSize(int index, int stackSize)
    {
        PlayerInventory.inventoryItems[index].stackSize = stackSize;
        slots[index - 1].SetStackSizeText(stackSize);
    }

    private void SetStackSizeToMax(int index)
    {
        int max = PlayerInventory.inventoryItems[index].itemData.maxStackSize;
        PlayerInventory.inventoryItems[index].stackSize = max;
        slots[index - 1].SetStackSizeText(max);
    }

    //void SetSlotData(InventorySlotType inventorySlotType, int index, Item itemData, int stackSize)
    //{
    //    switch(inventorySlotType)
    //    {
    //        case InventorySlotType.PartySlot:
    //            _partySlots[index].SetStackSizeText(stackSize);
    //            playerInventory.inventoryItems[index].stackSize = stackSize;
    //            break;
    //        case InventorySlotType.QuickSlot:
    //            _quickSlots[index].SetStackSizeText(stackSize);
    //            //playerInventory.equipmentSlots[index].stackSize = stackSize;
    //            break;
    //        case InventorySlotType.EquipmentSlot:
    //            _equipmentSlots[index].SetStackSizeText(stackSize);
    //            playerInventory.equipmentSlots[index].stackSize = stackSize;
    //            break;
    //    }

    //    Debug.Log("<color=grey>SetSlotData: Adding item '" + itemData.identifier + "' to slot '" + index + "'</color>");

    //}

    private void ClearSlotData(UIInventorySlot slot)
    {
        //playerInventory.inventoryItems.Remove(slot.inventoryItem);
        //Debug.Log("<color=grey>ClearSlotData: Slot '" + slot.cellIndex + "'</color>");
        slot.GetIcon().sprite = null;
        slot.GetIcon().color = new Color(1, 1, 1, 0);
        slot.SetStackSizeText(0);
        slot.ClearSlot();
    }

    public static void EquipArmor(UIInventorySlot equipmentSlot, UIInventorySlot originalSlot, Armor armor)
    {
        Debug.Log("<color=cyan>Adding equipment to slot</color>");

        //InventoryItem swappedArmor;
        ////InventorySlot heroTargetSlot = partyMemberEquipment[_selectedHero].GetEquipmentSlots()[(int)armor.bodySlot];
        //partyMemberEquipment[_selectedHero].SetEquipment(armor, out swappedArmor);
        //equipmentSlot.AddItem(originalSlot.inventoryItem, 1);
        //playerInventory.inventoryItems.Remove(originalSlot.inventoryItem);
        //if(swappedArmor != null)
        //{
        //    originalSlot.AddItem(swappedArmor, 1);
        //}
        //else
        //{
        //    originalSlot.ClearSlot();
        //}

        //_selectedHero.Execute_EquipArmor(armor);

        //ActorGearData gearData = _selectedHero.Equipment;

        //else if(dataSlot.itemData is Weapon weapon)
        //{
        //        ActorGearData gearData = _selectedHero.Equipment;
        //        if(gearData.equippedWeapon.weaponObject != null && gearData.equippedWeapon.Weapon != weapon)
        //        {
        //            //SetEquipped(playerInventory.inventoryItems.Where(s => s.itemData == gearData.equippedWeapon.Weapon).FirstOrDefault().slotIndex, false);
        //        }
        //        //equippedWeaponSlot.itemData = weapon;
        //        //equippedWeaponSlot.slotIndex = dataSlot.slotIndex;
        //        _selectedHero.Execute_EquipWeapon(weapon, true, true);

        //    //else
        //    //{
        //    //    _selectedHero.Execute_UnequipWeapon(weapon, true, true);
        //    //}
        //}
    }
    public static void UnequipArmor(UIInventorySlot originalSlot, Armor armor)
    {
        if(Instance.debug)
        {
            Debug.Log("<color=cyan>Adding equipment to slot</color>");
        }

        //InventorySlot heroTargetSlot = partyMemberEquipment[_selectedHero].GetEquipmentSlots()[(int)armor.bodySlot];



        //ActorGearData gearData = _selectedHero.Equipment;

        //else if(dataSlot.itemData is Weapon weapon)
        //{
        //        ActorGearData gearData = _selectedHero.Equipment;
        //        if(gearData.equippedWeapon.weaponObject != null && gearData.equippedWeapon.Weapon != weapon)
        //        {
        //            //SetEquipped(playerInventory.inventoryItems.Where(s => s.itemData == gearData.equippedWeapon.Weapon).FirstOrDefault().slotIndex, false);
        //        }
        //        //equippedWeaponSlot.itemData = weapon;
        //        //equippedWeaponSlot.slotIndex = dataSlot.slotIndex;
        //        _selectedHero.Execute_EquipWeapon(weapon, true, true);

        //    //else
        //    //{
        //    //    _selectedHero.Execute_UnequipWeapon(weapon, true, true);
        //    //}
        //}
    }

    private void DropItem(InventoryItem item, int quantity)
    {
        //SelectionManager.selected[0].Execute_DropItem(item, true, true);

        if(item.itemData is Armor)
        {
            //SelectionManager.selected[0].Execute_UnequipArmor(armor, true);
        }
        else if(item.itemData is Weapon)
        {
            //SelectionManager.selected[0].Execute_UnequipWeapon(true, true);
        }
        _ = StartCoroutine(CR_DropItemDelayed(item.itemData.identifier, quantity, 0.2f));
    }

    private IEnumerator CR_DropItemDelayed(string identifier, int quantity, float delay)
    {
        yield return new WaitForSeconds(delay);
        Transform playerTransform = SelectionManager.selected[0].transform;
        _ = ItemDatabase.DropPhysicalObjectAsPickupItem(identifier, quantity, playerTransform.position + playerTransform.forward + new Vector3(0, 1, 0.13f), playerTransform.rotation);
    }

    public static void ToggleActive(bool active)
    {
        if(_uiScrolling)
        {
            return;
        }

        if(slots != null && slots[0] == null)
        {
            Instance.InitUISlots();
        }

        if(active == false)
        {
            Disable3DPreview();


            if(startSlot != null)
            {
                startSlot.ClearSlot();
                Dragging = false;
            }
            DisableGhostItem();
        }

        _ = CoroutineRunner.Instance.StartCoroutine(Instance.CR_ScrollPanel(active));
    }

    //! Scroll
    private IEnumerator CR_ScrollPanel(bool scrollIn)
    {
        if(_uiScrolling)
        {
            yield break;
        }

        _uiScrolling = true;

        if(scrollIn)
        {
            PopulateUISlotsFromInventory(PlayerInventory);
            inventoryPanel.gameObject.SetActive(true);
        }
        else
        {
            //GameStateManager.Instance.GetUIScript().gamePaused = false;
        }

        GameEventSystem.OnInventoryToggled?.Invoke(scrollIn);

        //float startScrollValue = mainPanel.position.y;
        float targetScrollValue = scrollIn ? 0 : Screen.height;
        float elapsedTime = 0;

        while(elapsedTime < mainPanelScrollTime /*mainPanel.anchoredPosition.y < targetScrollValue*/)
        {
            elapsedTime += Time.unscaledDeltaTime;
            Vector2 pos = inventoryPanel.anchoredPosition;
            pos.y = Mathf.Lerp(pos.y, targetScrollValue, elapsedTime / mainPanelScrollTime);
            inventoryPanel.anchoredPosition = pos;
            yield return null;
        }
        //while(panelRect.anchoredPosition.y != targetScrollValue)
        //{

        //    //elapsedTime += Time.deltaTime;
        //    Vector2 pos = panelRect.anchoredPosition;
        //    pos.y = Mathf.MoveTowards(pos.y, targetScrollValue, Time.unscaledDeltaTime * 100);
        //    panelRect.anchoredPosition = pos;
        //    yield return null;
        //}
        if(scrollIn == false)
        {
            inventoryPanel.gameObject.SetActive(false);
        }
        else
        {
            Enable3DPreview();
            //GameStateManager.Instance.uiScript.gamePaused = true;
            //TODO implement pause
        }

        _uiScrolling = false;
    }

    public static bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        //foreach(var result in results)
        //{
        //    Debug.Log("RaycastResult: " + result.gameObject.name);
        //}
        return results.Count > 0;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }

    private static void SetDragPreviewAlpha(Color color)
    {
        ItemGhostPreview.image.color = color;
        if(color.a == 0)
        {
            ItemGhostPreview.SetStackSize(0);
        }
    }

    public static void EnableGhostItem(UIInventorySlot slot, Image image, int stackSize, bool isSplitStack)
    {
        if(Instance.debug)
        {
            Debug.Log("<color=grey>Enabling ghost item</color>");
        }

        Dragging = true;

        ItemGhostPreview.startSlotIndex = slot.CellIndex;
        //itemGhostPreview.equipped = slot.equipped;
        ItemGhostPreview.SetData(slot.InventoryItem, stackSize, isSplitStack);
        ItemGhostPreview.image.sprite = image.sprite;

        SetDragPreviewAlpha(new Color(0.7f, 0.7f, 0.7f, 1));
        ItemGhostPreview.image.transform.position = Input.mousePosition + new Vector3(15, -15, 0);
        _ = Instance.StartCoroutine(CR_UpdateDragging());
    }

    private static IEnumerator CR_UpdateDragging()
    {
        while(Dragging)
        {
            ItemGhostPreview.image.transform.position = Input.mousePosition + new Vector3(15, -15, 0);

            yield return null;
        }
    }

    public static void DisableGhostItem()
    {

        Dragging = false;
        ItemGhostPreview.equipped = false;
        ItemGhostPreview.image.sprite = null;
        ItemGhostPreview.inventoryItem = null;
        ItemGhostPreview.SetStackSize(0);
        ItemGhostPreview.isSplitStack = false;

        SetDragPreviewAlpha(new Color(0.7f, 0.7f, 0.7f, 0));
        startSlot = null;
    }

    private void ShowSplitSlider(string name, int stackSize)
    {
        splitMax.text = stackSize.ToString();
        _splitSlider.maxValue = stackSize;
        _splitSlider.value = stackSize / 2;
        splitNum.text = _splitSlider.value.ToString();
        splitItemName.text = name;
        stackSplitPanel.gameObject.SetActive(true);
    }

    public void AcceptSplit(/*string name, int stackSize*/)
    {
        //_splitNum.text = _splitMax.text = stackSize.ToString();
        //_splitSlider.value = _splitSlider.maxValue = stackSize;
        //_splitItemName.text = name;
        //_stackSplitPanel.gameObject.SetActive(true);
        //int surplus = (int)_splitSlider.maxValue - (int)_splitSlider.value;

        Dragging = true;
        EnableGhostItem(startSlot, startSlot.GetIcon(), (int)_splitSlider.value, true);
        startSlot.GetIcon().color = new Color(0.5f, 0.5f, 0.5f, 1);
        //if(surplus == 0)
        //{
        //    startSlot.ClearItemData();
        //}
        //else
        //    startSlot.SetStackSize(surplus);

        //Debug.Log("<color=grey>Slider val: " + (int)_splitSlider.value + "</color>");

        HideSplitSlider();
        splitting = false;
        //startSlot = null;
    }

    private void HideSplitSlider()
    {
        stackSplitPanel.gameObject.SetActive(false);
    }

    //? Inspector events
    public void OnSplitSliderChanged(float newValue)
    {
        if(splitNum != null)
        {
            splitNum.text = newValue.ToString();
        }
    }

    private static void SetInventoryOwner(Actor actor)
    {
        if(Instance.inventoryPanel.gameObject.activeInHierarchy == false)
        {
            return;
        }
        if(actor != null)
        {
            actor.GetComponentInChildren<SkinnedMeshRenderer>().gameObject.layer = LayerMask.NameToLayer("Default");
        }

        PlayerInventory = actor.Inventory;

        _previewCamera.transform.position = actor.transform.position + (actor.transform.forward * 10f) + (actor.transform.right * 10f) + (Vector3.up * 1.7f);
        _previewCamera.transform.LookAt(actor.transform.position + (Vector3.up * 1f));

        SetUILayers();

        //if(partyMemberEquipment.Count > 0)
        PopulateEquipmentAndQuickSlots();
    }

    private static void Enable3DPreview()
    {
        _previewCamera.transform.position = SelectionManager.selected[0].transform.position + (SelectionManager.selected[0].transform.forward * 10f) + (SelectionManager.selected[0].transform.right * 10f) + (Vector3.up * 1.7f);
        _previewCamera.transform.LookAt(SelectionManager.selected[0].transform.position + (Vector3.up * 1f));
        _previewCamera.enabled = true;

        SetUILayers();
    }

    private static void Disable3DPreview()
    {
        _previewCamera.enabled = false;

        RestoreLayers();
    }

    private static void SetUILayers()
    {
        SelectionManager.selected[0].GetComponentInChildren<SkinnedMeshRenderer>().gameObject.layer = LayerMask.NameToLayer("PortraitCreation");
        Transform dressContainer = SelectionManager.selected[0].transform.Find("Dresses");
        if(dressContainer != null)
        {
            foreach(Transform t in dressContainer)
            {
                t.GetComponentInChildren<SkinnedMeshRenderer>().gameObject.layer = LayerMask.NameToLayer("PortraitCreation");
            }
        }
    }

    private static void RestoreLayers()
    {
        SelectionManager.selected[0].GetComponentInChildren<SkinnedMeshRenderer>().gameObject.layer = LayerMask.NameToLayer("Default");
        Transform dressContainer = SelectionManager.selected[0].transform.Find("Dresses");
        if(dressContainer != null)
        {
            foreach(Transform t in dressContainer)
            {
                t.GetComponentInChildren<SkinnedMeshRenderer>().gameObject.layer = LayerMask.NameToLayer("Default");
            }
        }
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(1, 61, 300, 30), "Dragging: " + Dragging);
        GUI.Label(new Rect(1, 81, 300, 30), "SlotType Below: " + SlotTypeBelow.ToString());
        GUI.Label(new Rect(1, 101, 300, 30), "SlotBelow: " + (slotBelow != null));
        GUI.Label(new Rect(1, 121, 300, 30), "StartSlot: " + (startSlot != null));
        GUI.Label(new Rect(1, 141, 300, 30), "Item Type: " + ((slotBelow != null && slotBelow.InventoryItem != null && slotBelow.InventoryItem.itemData != null) ? slotBelow.InventoryItem.itemData.identifier : "ItemData = null"));
    }
}

public struct GhostItem
{
    public int startSlotIndex;
    public bool equipped;
    public InventoryItem inventoryItem;
    public int stackSize { get; set; }
    public Image image;
    private TMPro.TextMeshProUGUI stackSizeText;

    public bool isSplitStack;

    public void Init(TMPro.TextMeshProUGUI stackSizeText)
    {
        this.stackSizeText = stackSizeText;
        this.stackSizeText.text = "";
    }

    public void SetData(InventoryItem item, int stackSize, bool isSplitStack)
    {
        this.isSplitStack = isSplitStack;
        this.inventoryItem = item;
        SetStackSize(stackSize);
    }

    public void SetStackSize(int newStackSize)
    {
        stackSize = newStackSize;

        if(newStackSize > 1)
        {
            stackSizeText.text = newStackSize.ToString();
        }
        else
        {
            stackSizeText.text = "";
        }
    }
}