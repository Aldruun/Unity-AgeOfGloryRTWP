
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;

[Flags]
public enum InventoryItemFlags
{
    IDENTIFIED = 1,
    UNSTEALABLE = 2,
    STOLEN = 4, // denotes steel items in pst
    UNDROPPABLE = 8,
    ACQUIRED = 0x10,	//this is a gemrb extension
    DESTRUCTIBLE = 0x20,//this is a gemrb extension
    EQUIPPED = 0x40,	//this is a gemrb extension
    SELECTED = 0x40,    //this is a gemrb extension
    STACKED = 0x80,	//this is a gemrb extension
    CRITICAL = 0x100, //coming from original item
    TWOHANDED = 0x200,
    MOVABLE = 0x400, //same as undroppable
    RESELLABLE = 0x800, //item will appear in shop when sold
    CURSED = 0x1000, //item is cursed
    UNKNOWN2000 = 0x2000, //totally unknown
    MAGICAL = 0x4000, //magical
    BOW = 0x8000, //
    SILVER = 0x10000,
    COLDIRON = 0x20000,
    STOLEN2 = 0x40000, //same as 4
    CONVERSABLE = 0x80000,
    PULSATING = 0x100000
}


//public enum InventoryType
//{
//    CREATURE,
//    HEAP
//}

[System.Serializable]
public class Inventory
{
    //private ActorMonoController _agent;
    public bool debug;

    private InventoryType inventoryType;

    private static int SLOT_HEAD = -1;
    private static int SLOT_MAGIC = -1;
    private static int SLOT_FIST = -1;
    private static int SLOT_MELEE = -1;
    private static int LAST_MELEE = -1;
    private static int SLOT_RANGED = -1;
    private static int LAST_RANGED = -1;
    private static int SLOT_QUICK = -1;

    private static int LAST_QUICK = -1;
    private static int SLOT_INV = -1;
    private static int LAST_INV = -1;
    private static int SLOT_LEFT = -1;
    private static int SLOT_ARMOR = -1;

    //! 32 Item Slots
    //! 10 Equipment Slots
    //! 6 Quick Slots
    //! = 48 slots
    public List<Ammo> ammo;

    public List<InventoryItem> inventoryItems;
    public int maxSlots = 15;
    public Action OnAmmoAdded;

    public Action OnWeaponAdded;
    private List<Weapon> weapons;

    public Inventory()
    {
        weapons = new List<Weapon>();
    }

    public void Init(List<InventoryItem> items, InventoryType inventoryType)
    {
        this.inventoryType = inventoryType;

        Profiler.BeginSample("INVENTORY items.Clear");
        inventoryItems = new List<InventoryItem>();

        if(items == null)
        {
            return;
        }

        InventoryItem[] cached = items.ToArray();
        Profiler.EndSample();

        Profiler.BeginSample("INVENTORY AddItem loop");
        foreach(var item in cached)
        {
            //if()
            //Debug.Log(item.stackSize + " x " + item.ID + " added");

            AddItem(item.ID, item.stackSize);
        }
        Profiler.EndSample();
    }

    public void AddItem(string identifier, int amount)
    {
        Profiler.BeginSample("-INVENTORY.GetItemFromJSON");
        Item item = ItemDatabase.GetItemTemplate(identifier);
        Profiler.EndSample();
        if(item == null)
        {
            Debug.LogError("Function::AddItem - Could not retrieve item [" + identifier +
                           "] from database");
            return;
        }

        int maxQuantity = item.maxStackSize;
        int slotIndex = 0;
        // Try to find an existing item stack that isn't full
        //KeyValuePair slotWithItem = inventoryItems.Where(k => k.Value.itemData != null && k.Value.itemData.identifier == item.identifier && maxQuantity > 1 &&
        //                                 k.Value.stackSize < maxQuantity).FirstOrDefault().Value;
        InventoryItem existingStackFound = null;

        Profiler.BeginSample("-INVENTORY.foreach");
        foreach(InventoryItem invItem in inventoryItems) //Hack Check every inventoryItem
        {
            if(invItem.itemData.identifier == identifier && maxQuantity > 1 && invItem.stackSize < maxQuantity)
            {
                existingStackFound = invItem;
                slotIndex = invItem.slotIndex;
                Profiler.EndSample();
                break;
            }
        }
        Profiler.EndSample();

        if(existingStackFound != null)
        {
            Profiler.BeginSample("-INVENTORY.if(existingStackFound != null)");
            // Add new amount to existing stack
            existingStackFound.stackSize += amount;

            // Make sure the stack doesn't grow beyond limits
            if(existingStackFound.stackSize > maxQuantity)
            {
                int surplus = existingStackFound.stackSize - maxQuantity;
                existingStackFound.stackSize = maxQuantity;

                AddItem(identifier, surplus);
            }
            Profiler.EndSample();
        }
        // This item type does not exist or only in full stacks, so attempt to create a new stack
        else
        {
            Profiler.BeginSample("-INVENTORY.if(existingStackFound == null)");
            var freeSlot = new InventoryItem();
            freeSlot.ID = identifier;
            freeSlot.itemData = item;
            freeSlot.slotIndex = -1;

            if(item is Weapon w)
                AddWeapon(w);
            else if(item is Ammo a)
                ammo.Add(a);

            if(amount > maxQuantity)
            {
                //items.Add(new InventoryItem(identifier, item.itemCategoryType, maxQuantity, 0));
                freeSlot.stackSize = maxQuantity;
                AddItem(identifier, amount - maxQuantity);
            }
            else
            {
                freeSlot.stackSize = amount;
                //items.Add(new InventoryItem(identifier, item.itemCategoryType, amount, 0));
            }

            inventoryItems.Add(freeSlot);
            Profiler.EndSample();
        }
        //GameEventSystem.OnPlayerItemAdded?.Invoke(identifier, amount);
    }

    internal void AddItems(List<InventoryItem> items)
    {
        InventoryItem[] cached = items.ToArray();
        Profiler.EndSample();

        Profiler.BeginSample("INVENTORY AddItem loop");
        foreach(InventoryItem item in cached)
        {
            AddItem(item.ID, item.stackSize);
        }
        Profiler.EndSample();
    }

    internal int RemoveItem(string identifier, InventoryItemFlags flags, int count)
    {
        Item item = ItemDatabase.GetItemTemplate(identifier);
        Debug.Assert(item != null, "Inventory.RemoveItem::ItemToRemove = null");

        int slot = inventoryItems.Count;
        InventoryItemFlags mask = (flags ^ InventoryItemFlags.UNDROPPABLE);
        //if(core->HasFeature(Constants.GF_NO_DROP_CAN_MOVE))
        //{
        //    mask &= ~IE_INV_ITEM_UNDROPPABLE;
        //}
        while(slot-- > 0)
        {
            InventoryItem invitem = inventoryItems[slot];
            if(invitem == null)
            {
                inventoryItems.RemoveAt(slot);
                continue;
            }

            if(flags > 0 && (mask & invitem.flags) == flags)
            {
                continue;
            }
            if(flags == 0 && (mask & invitem.flags) != 0)
            {
                continue;
            }
            //if(resref[0] && strnicmp(item->ItemResRef, resref, 8))
            //{
            //    continue;
            //}
            RemoveItem(slot, count);
            return (int)slot;
        }
        return -1;
    }

    private InventoryItem RemoveItem(int slot, int count)
    {
        InventoryItem item;

        if(slot >= inventoryItems.Count)
        {
            Debug.LogError("Inventory.RemoveItem::Slot out of bounds");
        }
        item = inventoryItems[slot];

        if(item == null)
        {
            return null;
        }

        if(count == 0 || (item.flags.HasFlag(InventoryItemFlags.STACKED)) || (count >= item.stackSize))
        {
            KillSlot(slot);
            return item;
        }

        InventoryItem returned = new InventoryItem(item);
        item.stackSize -= count;
        returned.stackSize = count;
        //CalculateWeight();
        return returned;
    }

    private void KillSlot(int index)
    {
        if(inventoryType == InventoryType.Actor)
        {
            inventoryItems.RemoveAt(index);
            return;
        }
    }

    public InventoryItem GetItem(string ID)
    {
        //Item item = ItemDatabase.GetItemByID(ID);
        InventoryItem item = null;
        return item;
    }

    public void TransferItem(Inventory targetInventory, string identifier, int amount)
    {
        targetInventory.AddItem(identifier, TakeQuantity(identifier, amount));
    }

    public void RemoveStack(int slotIndex)
    {
        inventoryItems[slotIndex].ClearData();
    }

    internal bool HasItemOfType<T>() where T : Item
    {
        foreach(InventoryItem item in inventoryItems)
        {
            if(item.itemData is T)
            {
                return true;
            }
        }

        return false;
    }

    //public void RemoveItem(string identifier) { // This removes a full stack
    //    InventoryItem item = items.Where(i => i.itemData.ID == identifier).FirstOrDefault();

    //    if(item != null) {
    //        items.Remove(item);
    //    }
    //}

    public InventoryItem GetItem(string identifier, int amount, bool onlyFullAmount = false)
    {
        InventoryItem item = inventoryItems.Where(k => k.itemData.identifier == identifier).FirstOrDefault();

        if(item != null)
        {
            int available = item.stackSize;

            if(amount > available)
            {
                if(onlyFullAmount == false)
                    return item;
            }
            else
            {
                item.stackSize -= amount;
                return item;
            }
        }

        return null;
    }

    /// <summary>
    ///     Returns the amount of the specified item and removes it from the inventory.
    /// </summary>
    /// <param name="identifier"></param>
    /// <param name="amount"></param>
    /// <param name="onlyFullAmount"></param>
    /// <returns></returns>
    public int TakeQuantity(string identifier, int amount, bool onlyFullAmount = false)
    {
        List<InventoryItem> foundItemStacks = inventoryItems.Where(k => k.itemData != null && k.itemData.identifier == identifier).Select(i => i).ToList();

        int sum = 0;

        for(int i = 0; i < foundItemStacks.Count; i++)
        {
            InventoryItem itemStack = foundItemStacks[i];

            int currQuant = itemStack.stackSize;
            int restNeeded = Mathf.Clamp(amount - sum, 0, amount);

            if(restNeeded >= currQuant)
            {
                inventoryItems.Remove(itemStack);
                //GameEventSystem.OnPlayerItemRemoved?.Invoke(itemStack.itemData.identifier, itemStack.slotIndex);
                sum += currQuant;
            }
            else
            {
                int surplus = currQuant - restNeeded;
                sum += restNeeded;

                foundItemStacks[i].stackSize = surplus;
            }
        }

        return Mathf.Clamp(sum, 0, sum);
    }

    public int GetAmountOf(string identifier)
    {
        int sum = 0;
        InventoryItem[] all = inventoryItems.Where(k => k.itemData.identifier == identifier).Select(i => i).ToArray();
        foreach(InventoryItem found in all)
            sum += found.stackSize;

        return sum;
    }

    public void ClearSlotData(int index)
    {
        inventoryItems[index].ClearData();
    }

    //public InventoryItem GetItemWithHighestStat(string statName) {
    //    InventoryItem[] itemsWithStat = items.Where(i => i.m_itemData.stats != null && i.m_itemData.ContainsStat(statName)).ToArray();
    //    return itemsWithStat.Aggregate((i1, i2) => i1.m_itemData.GetStat(statName).GetCalculatedStatValue() > i2.m_itemData.GetStat(statName).GetCalculatedStatValue() ? i1 : i2);
    //}

    //public InventoryItem[] GetItemsByItemType(ItemType itemType) {
    //    InventoryItem[] item = items.Where(i => i.m_itemData.itemType == itemType).ToArray();

    //    return item;
    //}

    /// <summary>
    ///     Returns an item and removes it.
    /// </summary>
    /// <param name="itemType"></param>
    /// <returns></returns>
    //public InventoryItem TakeItemByItemType(ItemType itemType, int amount) {
    //    InventoryItem item = items.Where(i => i.m_itemData.itemType == itemType).FirstOrDefault();
    //    if(item != null)
    //        TakeQuantity(item.m_itemData.ID, amount);

    //    return item;
    //}

    //public bool HasItemOfType(ItemType itemType) {
    //    return items.Where(i => i.m_item.itemType == itemType).FirstOrDefault() != null;
    //}

    //internal InventoryItem[] GetRangedWeapons(bool onlyWhenHasAmmo) {
    //    // This list will be empty if there is no ammo in the inventory
    //    string[] availableAmmoTypes = onlyWhenHasAmmo ? GetItemsByItemType(ItemType.Ammo).Select(a => a.m_itemData.ID).ToArray() : null;

    //    //Debug.Log('▤'.ToString() + " Available ammo types: " + availableAmmoTypes.Length);

    //    List<InventoryItem> availableWeapons = new List<InventoryItem>();

    //    for(int i = 0; i < items.Count; i++) {
    //        if(items[i].m_item.itemType != ItemType.WeaponRanged1H && items[i].m_item.itemType != ItemType.WeaponRanged2H)
    //            continue;

    //        List<BaseStat> stats = new List<BaseStat>(items[i].m_item.stats);
    //        //if(stats == null)
    //        //    Debug.Log("Stats = null");
    //        //if(stats.Count == 0)
    //        //    Debug.Log("Stats = empty");

    //        if(stats != null && stats.Count > 0) {
    //            BaseStat ammoStat = stats.Where(s => s.statType == BaseStatType.Ammo).FirstOrDefault();

    //            //Debug.Log("ammotStat identifier = '" + ammoStat.statID + "'");

    //            //if(ammoStat == null)
    //            //    Debug.Log("Ammo stat = null");
    //            if(ammoStat != null) {
    //                if(onlyWhenHasAmmo) {
    //                    if(availableAmmoTypes.Contains(ammoStat.statID)) {
    //                        //Debug.Log("Adding item '" + items[i].itemData.ID + "'");
    //                        availableWeapons.Add(items[i]);
    //                    }
    //                }
    //                else {
    //                    availableWeapons.Add(items[i]);
    //                }
    //            }
    //        }
    //    }

    //    //Debug.Log('▤'.ToString() + " Available weapons: " + availableWeapons.Count);

    //    return availableWeapons.ToArray();
    //}
    //internal InventoryItem[] GetMeleeWeapons()
    //{
    //    List<InventoryItem> availableWeapons = new List<InventoryItem>();

    //    for(int i = 0; i < items.Count; i++)
    //    {
    //        List<BaseStat> stats = new List<BaseStat>(items[i].m_item.stats);
    //        if(stats.Where(s => s.statType == BaseStatType.Damage).FirstOrDefault() == null || stats.Where(s => s.statType == BaseStatType.Ammo).FirstOrDefault() != null)
    //            continue;

    //        if(stats.Count > 0)
    //        {
    //            //Debug.Log("Adding item '" + items[i].itemData.ID + "'");
    //            availableWeapons.Add(items[i]);
    //        }
    //    }

    //    //Debug.Log('▤'.ToString() + " Available weapons: " + availableWeapons.Count);

    //    return availableWeapons.ToArray();
    //}
    public List<Weapon> GetWeapons()
    {
        List<Weapon> weapons = new List<Weapon>();

        foreach(InventoryItem invItem in inventoryItems)
        //Debug.Log(invItem.item.ID + " (" + invItem.itemType.ToString() + ")");
        {
            if(invItem.itemData == null)
            {
                break;
            }

            //Debug.Log(_agent.ActorRecord.Name + ": GetWeapons(): Iterating item '" + invItem.itemData.identifier + "'");
            if(invItem.itemData.itemCategoryType == ItemCategoryType.Weapon)
            {
                //if(invItem == null)
                //{
                //    Debug.LogError("invItem null");
                //}
                //if(invItem.item == null)
                //{
                //    invItem.item = ResourceManager.GetItem(invItem.ID);

                //}

                Item item = ResourceManager.GetItem(invItem.ID);
                //Debug.Log(_agent.ActorRecord.Name + ": GetWeapons(): Adding possible weapon '" + invItem.itemData.identifier + "'");
                weapons.Add((Weapon)item);
            }
        }

        //Debug.Log(weapons.Count);
        return weapons;
    }

    public bool HasWeapon(bool onlyRanged)
    {
        if(onlyRanged)
            for(int i = 0; i < inventoryItems.Count; i++)
            {
                Weapon weapon = (Weapon)ResourceManager.GetItem(inventoryItems[i].itemData.identifier);

                if(inventoryItems[i].itemType == ItemCategoryType.Weapon || inventoryItems[i].itemType == ItemCategoryType.Weapon)
                    return true;
            }
        else
            for(int i = 0; i < inventoryItems.Count; i++)
                if(inventoryItems[i].itemType == ItemCategoryType.Weapon)
                    return true;

        return false;
    }

    public bool HasItem(string identifier, int num)
    {
        foreach(var slot in inventoryItems)
        {
            if(slot.itemData != null)
            {
                if(slot.itemData.identifier == identifier)
                {
                    return true;
                }
            }
        }

        //Debug.Log("Couldn't find item of type '" + identifier + "'");

        return false;
    }

    //public bool HasItem<T>() where T : Item {
    //    for(int i = 0; i < items.Count; i++) {
    //        if(items[i].item.GetType() == typeof(T)) {
    //            return true;
    //        }
    //    }

    //    return false;
    //}

    //void GenerateRandomContent() {
    //    if(lootTable == null) {
    //        Debug.LogError(gameObject.name + ": Inventory.cs: No loot table assigned");
    //        return;
    //    }
    //    if(overrideExistingItems) {
    //        items.Clear();
    //    }

    //    FillInventoryRandomly( numRandomItems);
    //        //Debug.Log("Adding '" + rndItemData.Name + "'");
    //    //}
    //}

    //public void FillInventoryRandomly(int numSlotsToFill) {
    //    items.AddRange(lootTable.GetResultingItems(numSlotsToFill, 2));
    //}

    public Weapon AddWeapon(Weapon newWeapon)
    {
        weapons.Add(newWeapon);
        OnWeaponAdded?.Invoke();
        return newWeapon;
    }

    public Weapon GetBestMeleeWeapon()
    {
        Weapon bestWeapon = null;

        for(int i = 0; i < inventoryItems.Count; i++)
        {
            if(inventoryItems[i].itemData is Weapon weapon)
            {
                if(weapon.IsRanged == false && (bestWeapon == null || weapon.BaseDamageRoll > bestWeapon.BaseDamageRoll))
                    bestWeapon = weapon;
            }
        }

        return bestWeapon;
    }

    public Weapon GetBestRangedWeapon()
    {
        Weapon bestWeapon = null;

        for(int i = 0; i < inventoryItems.Count; i++)
        {
            if(inventoryItems[i].itemData is Weapon weapon)
            {
                if(weapon.IsRanged && (bestWeapon == null || weapon.BaseDamageRoll > bestWeapon.BaseDamageRoll))
                {
                    if(HasAmmo(weapon.ammoType))
                    {
                        bestWeapon = weapon;
                    }
                }
            }
        }

        return bestWeapon;
    }

    public Ammo AddAmmo(Ammo newAmmo)
    {
        ammo.Add(newAmmo);
        OnWeaponAdded?.Invoke();
        return newAmmo;
    }

    private bool HasAmmo(AmmoType ammoType)
    {
        if(ammoType == AmmoType.None)
        {
            Debug.LogError("Searching for ammo with ammo type None");
            return false;
        }

        for(int i = 0; i < inventoryItems.Count; i++)
        {
            //if(inventoryItems[i].itemData is Ammo ammo)
            //{
            //    if(ammo.ammoType == AmmoType.None)
            //    {
            //        Debug.LogError("Ammo's ammo type is None");
            //        return false;
            //    }

            //    if(ammo.ammoType == ammoType)
            //        return true;
            //}
        }

        return false;
    }

    internal Weapon GetUsedWeapon(ref int slot)
    {
        slot = 0;

        return null;
    }
}