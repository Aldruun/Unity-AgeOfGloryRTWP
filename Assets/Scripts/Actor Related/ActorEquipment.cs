using AoG.Serialization;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.VFX;

// NOTES:
/*
 * "Armor" refers to the whole body mesh
 */

public class ActorEquipment
{
    public Equipment EquipmentSlots;
    public WeaponData equippedWeapon;
    public KeyValuePair<Armor, GameObject> equippedHeadwear;
    public KeyValuePair<Armor, GameObject> equippedCape;
    public Armor equippedNecklace;
    public KeyValuePair<Armor, GameObject> equippedDress;
    public Armor equippedRingLeft;

    public Armor equippedRingRight;
    public KeyValuePair<Armor, GameObject> equippedGloves;
    public Armor equippedBelt;
    public KeyValuePair<Armor, GameObject> equippedBoots;
    public KeyValuePair<Armor, GameObject> equippedUnderwear;
    public KeyValuePair<Armor, GameObject> equippedShield;
    public Ammo equippedAmmo;

    private readonly ActorInput self;
    private readonly Inventory Inventory;
    public int healthPotions;
    private readonly Transform m_offhand;
    public readonly Transform m_weaponHand;
    private readonly Transform m_weaponHolsterHipLeft;
    private readonly Transform m_weaponHolster2HSword;
    private readonly Transform m_weaponHolster2HHammer;
    private readonly Transform m_weaponHolsterRifle;
    private readonly Transform m_weaponHolsterBow;
    private readonly Transform m_weaponHolsterQuiver;

    public readonly Transform spellAnchor;
    private GameObject _quiverObject;
    private ActorStats stats;

    // #############

    public int manaPotions;

    public ActorEquipment(ActorInput agent, ActorStats stats, Inventory inventory)
    {
        Debug.Assert(agent != null);
        self = agent;
        if(agent.debugGear)
            Debug.Log(agent.GetName() + " /Creating GearData");

        this.stats = stats;
        Inventory = inventory;
        Animator animator = agent.GetComponent<Animator>();
        m_weaponHolsterHipLeft = animator.GetBoneTransform(HumanBodyBones.Hips).Find("anchor_hip");
        Transform chest = animator.GetBoneTransform(HumanBodyBones.Chest);
        m_weaponHolster2HSword = chest.Find("anchor_2hsword");
        m_weaponHolster2HHammer = chest.Find("anchor_2hhammer");
        m_weaponHolsterRifle = chest.Find("anchor_rifle");
        m_weaponHolsterBow = chest.Find("anchor_bow");
        m_weaponHolsterQuiver = chest.Find("anchor_quiver");

        m_offhand = animator.GetBoneTransform(HumanBodyBones.LeftHand).Find("anchor_hand.L");
        m_weaponHand = animator.GetBoneTransform(HumanBodyBones.RightHand).Find("anchor_hand.R");

        equippedWeapon = new WeaponData();
        SetUpFist();
        //inventory = this._player.GetComponent<Inventory>();

        if(animator.isHuman)
        {
            switch(stats.Gender)
            {
                case Gender.Female:
                    if(agent.debugGear)
                        Debug.Log(agent.GetName() + ": Equipping underwear");
                    break;

                case Gender.Male:
                    break;
            }

            Transform rightHand = animator.GetBoneTransform(HumanBodyBones.RightHand);

            spellAnchor = new GameObject("vfxAnchor").transform;
            spellAnchor.SetParent(m_weaponHand, false);

        }
        else
        {
            spellAnchor.SetParent(agent.Combat.GetAttackPoint());
        }
    }

    internal bool WeaponInHand()
    {
        return equippedWeapon.Weapon.animationPack == AnimationSet.UNARMED || equippedWeapon.WeaponObject.transform.parent == m_weaponHand;
    }

    private void SpawnAndHolsterWeapon(Weapon weapon)
    {
        if(equippedWeapon.WeaponObject != null)
        {
            if(equippedWeapon.Weapon.identifier == weapon.identifier)
            {
                return;
            }
            MonoBehaviour.Destroy(equippedWeapon.WeaponObject);
        }

        if(weapon.identifier == "")
            Debug.LogError("weapon.identifier empty");
        GameObject weaponObject = ItemDatabase.InstantiatePhysicalItem(weapon.identifier);
        equippedWeapon.Set(weapon, weaponObject, weaponObject.GetComponentInChildren<Animator>());

        if(equippedWeapon.WeaponTrailVFX != null)
            equippedWeapon.WeaponTrailVFX.transform.SetParent(self.transform, false);

        equippedWeapon.Weapon.Init();
        if(weaponObject == null)
            Debug.LogError("weaponObject = null");
        bool isBow = equippedWeapon.Weapon.weaponCategory is WeaponCategory.Longbow or WeaponCategory.Shortbow;
        if(isBow)
            CreateQuiver();

        ParentWeaponToHolster();
    }

    public void ParentWeaponToHand()
    {
        if(equippedWeapon.WeaponObject == null)
        {
            if(equippedWeapon.Weapon == null)
                SetUpFist();
            return;
        }

        if(self.debugGear)
            Debug.Log(self.GetName() + ":<color=orange>4</color> ActorGearData.DrawWeapon");

        Transform hand = equippedWeapon.Weapon.animationPack == AnimationSet.BOW ? m_offhand : m_weaponHand;
        if(hand == null)
        {
            Debug.LogError(self.GetName() + ": weaponHand null");
        }

        equippedWeapon.WeaponObject.transform.SetParent(hand, false);
    }

    public void ParentWeaponToHolster()
    {
        if(equippedWeapon.WeaponObject == null)
        {
            if(self.debugGear)
                Debug.LogError(self.GetName() + ":<color=red>*</color> Cannot holster: weaponObject = null");
            return;
        }
        if(self.debugGear)
            Debug.Log(self.GetName() + ":<color=green>*</color> ActorGearData.HolsterEquippedWeapon");
        Transform holster = null;
        switch(equippedWeapon.Weapon.animationPack)
        {
            case AnimationSet.DAGGER:
            case AnimationSet.ONEHANDED:
                holster = m_weaponHolsterHipLeft;
                break;
            case AnimationSet.TWOHANDED:
                holster = m_weaponHolster2HSword;
                break;
            case AnimationSet.XBOW:
            case AnimationSet.BOW:
                holster = m_weaponHolsterBow;
                break;
        }

        equippedWeapon.WeaponObject.transform.SetParent(holster, false);
    }

    private void CreateQuiver()
    {
        if(_quiverObject == null)
        {
            _quiverObject = Object.Instantiate(Resources.Load<GameObject>("Prefabs/Items/Weapons/Ranged/Bows/Quivers/standard quiver"));
            _quiverObject.transform.SetParent(m_weaponHolsterQuiver, false);
        }
    }

    /// <summary>
    /// Destroys the equipped weapon immediately. It doesn't matter if it is drawn or not.
    /// </summary>
    public void UnequipWeapon()
    {
        self.StartCoroutine(CR_UnequipWeapon());
    }

    private IEnumerator CR_UnequipWeapon()
    {
        yield return new WaitForSeconds(0.3f);

        if(equippedWeapon.Weapon.weaponCategory == WeaponCategory.Longbow || equippedWeapon.Weapon.weaponCategory == WeaponCategory.Shortbow)
        {
            Object.Destroy(m_weaponHolsterBow.GetChild(0).gameObject);
        }

        Object.Destroy(equippedWeapon.WeaponObject);
    }

    public Weapon EquipBestWeapon(int EQUIP_TYPE)
    {
        if(self.debugGear)
            Debug.Log(self.GetName() + ":<color=orange>3</color> ActorGearData.EquipBestWeapon");

        if(self.ActorStats.isBeast)
        {
            return SetUpClaw();
        }

        //_self.OnHandleBow = HandleBow;

        if(self.Inventory.inventoryItems == null)
        {
            //Debug.LogError(_self.ActorRecord.Name + ": inventory.items == null");
            return null;
        }
        else
        {
            Weapon _weapon = GetBestWeaponOfTypeFromInventory(EQUIP_TYPE);

            if(_weapon == null) // No weapon found. Use fist or current weapon.
            {
                //if(equippedWeapon.weaponObject != null)
                //    return;

                if(self.debugGear)
                    Debug.Log(self.GetName() + ": <color=yellow>weapon null, setting up fist</color>");

                return SetUpFist();
            }
            else
            {
                DestroyUsedWeapon();
                SpawnAndHolsterWeapon(_weapon); //! Sets up weaponinfo
                //ParentWeaponToHand();
                return _weapon;
            }
        }
    }

    private Weapon SetUpClaw()
    {
        Weapon creatureClaw = new Weapon();
        creatureClaw.animationPack = AnimationSet.UNARMED;
        creatureClaw.weaponCategory = WeaponCategory.Unarmed;
        creatureClaw.impactSFXType = WeaponImpactType.Creature;
        creatureClaw.damage = 10;
        creatureClaw.Init();
        equippedWeapon.Weapon = creatureClaw;
        return equippedWeapon.Weapon;
    }

    internal Weapon SetUpFist()
    {
        Weapon fist = new Weapon();
        fist.animationPack = AnimationSet.UNARMED;
        fist.weaponCategory = WeaponCategory.Unarmed;
        fist.damageType = DamageType.CRUSHING;
        fist.identifier = "fist";
        fist.damage = 4;
        fist.range = 1.5f;
        fist.Init();
        equippedWeapon.Weapon = fist;
        return equippedWeapon.Weapon;
    }

    private Weapon GetBestWeaponOfTypeFromInventory(int EQUIP_TYPE)
    {
        switch(EQUIP_TYPE)
        {
            case Constants.EQUIP_MELEE:
                return self.Inventory.GetBestMeleeWeapon();
            case Constants.EQUIP_RANGED:
                return self.Inventory.GetBestRangedWeapon();
            case Constants.EQUIP_ANY:
                Weapon weapon = self.Inventory.GetBestRangedWeapon();
                if(weapon == null)
                    return self.Inventory.GetBestMeleeWeapon();
                else
                {
                    return weapon;
                }
        }

        return null;
    }

    private void DestroyUsedWeapon()
    {
        if(equippedWeapon.WeaponObject != null)
        {
            if(_quiverObject != null)
            {
                Object.DestroyImmediate(_quiverObject);
            }
            Object.DestroyImmediate(equippedWeapon.WeaponObject);
            if(equippedWeapon.WeaponTrailVFX != null)
                Object.DestroyImmediate(equippedWeapon.WeaponTrailVFX.gameObject);
            equippedWeapon.Weapon = null;
        }
    }

    public Equipment InitEquipment()
    {
        //if(owner.debugGear)
        //    Debug.Log("<color=grey>UIInventory OnPartyMemberAdded</color>");

        EquipmentSlots = new Equipment();

        if(equippedHeadwear.Value != null)
        {
            EquipmentSlots.EquipArmor(equippedHeadwear.Key, out ArmorData none);
        }
        if(equippedCape.Value != null)
        {
            EquipmentSlots.EquipArmor(equippedCape.Key, out ArmorData none);
        }
        if(equippedNecklace != null)
        {
            EquipmentSlots.EquipArmor(equippedNecklace, out ArmorData none);
        }
        if(equippedDress.Value != null)
        {
            EquipmentSlots.EquipArmor(equippedDress.Key, out ArmorData none);
        }
        if(equippedRingLeft != null)
        {
            EquipmentSlots.EquipArmor(equippedRingLeft, out ArmorData none);
        }
        if(equippedRingRight != null)
        {
            EquipmentSlots.EquipArmor(equippedRingRight, out ArmorData none);
        }
        if(equippedBelt != null)
        {
            EquipmentSlots.EquipArmor(equippedBelt, out ArmorData none);
        }
        if(equippedBoots.Value != null)
        {
            EquipmentSlots.EquipArmor(equippedBoots.Key, out ArmorData none);
        }

        return EquipmentSlots;
        //partyMemberEquipment.Add(newMember, newEquipment);
    }

    public void EquipBestArmor()
    {
        if(self == null)
        {
            return;
        }

        if(Inventory.HasItemOfType<Armor>() == false)
            return;

        InventoryItem[] armorPieces = Inventory.inventoryItems.Where(i => i.itemData is Armor).ToArray();

        for(int i = 0; i < armorPieces.Length; i++)
        {
            Profiler.BeginSample("Equipment.EquipArmor");
            EquipArmor((Armor)armorPieces[i].itemData);
            Profiler.EndSample();
        }
    }

    public void EquipBestArmor(EquipmentSlot equipmentSlot)
    {
        if(self == null)
        {
            return;
        }

        switch(equipmentSlot)
        {
            case EquipmentSlot.Head:
                break;
            case EquipmentSlot.Hands:
                break;
            case EquipmentSlot.Body:
                break;
            case EquipmentSlot.Bottom:
                break;
            case EquipmentSlot.Feet:
                break;
        }
       
        if(Inventory.HasItemOfType<Armor>() == false)
            return;

        InventoryItem armorPiece = Inventory.inventoryItems.Where(i => i.itemData is Armor).FirstOrDefault();
        
        //if(armorPieces == null)
        //{
        //    return;
        //}

        Profiler.BeginSample("Equipment.EquipArmorPiece");
        EquipArmor((Armor)armorPiece.itemData);
        Profiler.EndSample();

    }

    internal void EquipArmorFromEquipmentData(Equipment equipmentData)
    {
        for(int i = 0; i < equipmentData.GetArmorSlots().Length; i++)
        {
            Armor armor = equipmentData.GetArmorSlots()[i].Armor;

            Profiler.BeginSample(self.GetName() + ": ACTOR InstantiatePhysicalItem");
            GameObject dressRootObject = ItemDatabase.InstantiatePhysicalItem((armor.identifier + " " + self.ActorStats.Race.ToString() + self.ActorStats.Gender.ToString()).ToLower());
            Profiler.EndSample();

            if(dressRootObject == null)
            {
                Debug.LogError(self.GetName() + ": Dress root object = null");
            }
            else
            {
                Debug.Log(self.GetName() + ": Instantiated armor object '" + dressRootObject.name + "'");
            }
            Profiler.BeginSample("ACTOR EquipArmor");
            ActorUtility.ModifyStatModded(self.ActorStats, ActorStat.AC, armor.AC, ModType.ADDITIVE);
            Profiler.EndSample();

            //actorInput.character.OnArmorEquipped?.Invoke(armor, true, true);

            switch(armor.bodySlot)
            {
                case BodySlot.Helmet:

                    if(equippedHeadwear.Value != null)
                    {
                        UnityEngine.Object.DestroyImmediate(equippedHeadwear.Value);
                    }
                    equippedHeadwear = new KeyValuePair<Armor, GameObject>(armor, dressRootObject);
                    break;
                case BodySlot.Cape:

                    if(equippedCape.Value != null)
                    {
                        UnityEngine.Object.DestroyImmediate(equippedCape.Value);
                    }
                    equippedCape = new KeyValuePair<Armor, GameObject>(armor, dressRootObject);
                    break;

                case BodySlot.Necklace:
                    equippedNecklace = armor;
                    break;
                case BodySlot.Dress:

                    if(equippedDress.Value != null)
                    {
                        UnityEngine.Object.DestroyImmediate(equippedDress.Value);
                    }
                    equippedDress = new KeyValuePair<Armor, GameObject>(armor, dressRootObject);
                    break;

                case BodySlot.Gloves:
                    if(equippedGloves.Value != null)
                    {
                        UnityEngine.Object.DestroyImmediate(equippedGloves.Value);
                    }
                    equippedGloves = new KeyValuePair<Armor, GameObject>(armor, dressRootObject);
                    break;
                case BodySlot.RingLeft:
                    equippedRingLeft = armor;
                    break;
                case BodySlot.RingRight:
                    equippedRingRight = armor;
                    break;
                case BodySlot.Belt:
                    equippedBelt = armor;
                    break;
                case BodySlot.Boots:

                    if(equippedBoots.Value != null)
                    {
                        UnityEngine.Object.Destroy(equippedBoots.Value);
                    }
                    equippedBoots = new KeyValuePair<Armor, GameObject>(armor, dressRootObject);
                    break;
            }

            //equippedUnderwear.Value.SetActive(equippedDress.Value == null);

            //Debug.Log("<color=grey>Dress root object '" + dressRootObject.name + "' being processed</color>");

            DressUpManager.AttachBipedObject(this.self.transform, dressRootObject);
        }
    }

    internal void EquipArmor(Armor armor)
    {
        Profiler.BeginSample(self.GetName() + ": ACTOR InstantiatePhysicalItem");
        GameObject dressRootObject = ItemDatabase.InstantiatePhysicalItem((armor.identifier + " " + self.ActorStats.Race.ToString() + self.ActorStats.Gender.ToString()).ToLower());
        Profiler.EndSample();

        if(dressRootObject == null)
        {
            Debug.LogError(self.GetName() + ": Dress root object = null");
        }
        else
        {
            Debug.Log(self.GetName() + ": Instantiated armor object '" + dressRootObject.name + "'");
        }
        Profiler.BeginSample("ACTOR EquipArmor");
        ActorUtility.ModifyStatModded(self.ActorStats, ActorStat.AC, armor.AC, ModType.ADDITIVE);
        Profiler.EndSample();

        //actorInput.character.OnArmorEquipped?.Invoke(armor, true, true);

        switch(armor.bodySlot)
        {
            case BodySlot.Helmet:

                if(equippedHeadwear.Value != null)
                {
                    UnityEngine.Object.DestroyImmediate(equippedHeadwear.Value);
                }
                equippedHeadwear = new KeyValuePair<Armor, GameObject>(armor, dressRootObject);
                break;
            case BodySlot.Cape:

                if(equippedCape.Value != null)
                {
                    UnityEngine.Object.DestroyImmediate(equippedCape.Value);
                }
                equippedCape = new KeyValuePair<Armor, GameObject>(armor, dressRootObject);
                break;

            case BodySlot.Necklace:
                equippedNecklace = armor;
                break;
            case BodySlot.Dress:

                if(equippedDress.Value != null)
                {
                    UnityEngine.Object.DestroyImmediate(equippedDress.Value);
                }
                equippedDress = new KeyValuePair<Armor, GameObject>(armor, dressRootObject);
                break;

            case BodySlot.Gloves:
                if(equippedGloves.Value != null)
                {
                    UnityEngine.Object.DestroyImmediate(equippedGloves.Value);
                }
                equippedGloves = new KeyValuePair<Armor, GameObject>(armor, dressRootObject);
                break;
            case BodySlot.RingLeft:
                equippedRingLeft = armor;
                break;
            case BodySlot.RingRight:
                equippedRingRight = armor;
                break;
            case BodySlot.Belt:
                equippedBelt = armor;
                break;
            case BodySlot.Boots:

                if(equippedBoots.Value != null)
                {
                    UnityEngine.Object.Destroy(equippedBoots.Value);
                }
                equippedBoots = new KeyValuePair<Armor, GameObject>(armor, dressRootObject);
                break;
        }

        //equippedUnderwear.Value.SetActive(equippedDress.Value == null);

        //Debug.Log("<color=grey>Dress root object '" + dressRootObject.name + "' being processed</color>");

        DressUpManager.AttachBipedObject(this.self.transform, dressRootObject);
    }



    internal void UnequipArmor(Armor armor)
    {
        //Debug.Log("UnequipArmor: Destroying armor of type " + armor.bodySlot.ToString());

        switch(armor.bodySlot)
        {
            case BodySlot.Helmet:
                UnityEngine.Object.DestroyImmediate(equippedHeadwear.Value);
                break;
            case BodySlot.Cape:
                UnityEngine.Object.DestroyImmediate(equippedCape.Value);
                break;
            case BodySlot.Necklace:
                equippedNecklace = null;
                break;
            case BodySlot.Dress:
                //Debug.Log("Destroying dress");
                GameObject dress = equippedDress.Value;
                UnityEngine.Object.DestroyImmediate(dress);
                break;
            case BodySlot.Gloves:
                UnityEngine.Object.DestroyImmediate(equippedGloves.Value);
                break;
            case BodySlot.RingLeft:
                equippedRingLeft = null;
                break;
            case BodySlot.RingRight:
                equippedRingRight = null;
                break;
            case BodySlot.Belt:
                equippedBelt = null;
                break;
            case BodySlot.Boots:
                UnityEngine.Object.DestroyImmediate(equippedBoots.Value);
                break;
        }
        //if(equippedDress.Value != null)
        //    Debug.Log("Dress not null");
        //else
        //Debug.Log("Dress null");
        //equippedUnderwear.Value.SetActive(equippedDress.Value == null);
    }
}

public class WeaponData
{
    public Animator Animator;
    public GameObject WeaponObject;
    public Weapon Weapon;
    public VisualEffect WeaponTrailVFX;

    public bool IsRanged => Weapon?.IsRanged ?? false;

    public void Set(Weapon weaponData, GameObject weaponObject, Animator animator)
    {
        this.Animator = animator;
        this.WeaponObject = weaponObject;
        Transform vfxTransform = weaponObject.transform.Find("WeaponTrailVFX");
        if(vfxTransform != null)
            WeaponTrailVFX = vfxTransform.GetComponent<VisualEffect>();
        this.Weapon = weaponData;
    }

    public WeaponData(Weapon weaponData, GameObject weaponObject, Animator animator)
    {
        this.Animator = animator;
        this.WeaponObject = weaponObject;
        this.Weapon = weaponData;
    }

    public WeaponData()
    {

    }
}

public class ArmorData
{
    public GameObject ArmorObject;
    public Armor Armor;

    public Color[] Colors;

    private SkinnedMeshRenderer armorSkinnedMesh;

    public void Set(Armor armorData, SkinnedMeshRenderer armorSkinnedMesh)
    {
        this.armorSkinnedMesh = armorSkinnedMesh;

        Colors = new Color[armorSkinnedMesh.materials.Length];
        for(int i = 0; i < armorSkinnedMesh.materials.Length; i++)
        {
            Material material = armorSkinnedMesh.materials[i];
            Colors[i] = material.color;
        }
        
        this.Armor = armorData;
    }

    public void SetColors(int index, Color newColor)
    {
        if(index + 1 > Colors.Length)
        {
            return;
        }

        Colors[index] = newColor;

        armorSkinnedMesh.materials[index].color = newColor;
    }

    public ArmorData()
    {

    }
}

[System.Serializable]
public class Equipment
{
    public SerializableColor ArmorColor1 { get; private set; }
    public SerializableColor ArmorColor2 { get; private set; }
    //private InventoryItem[] armorPieces;
    private ArmorData[] armorSlots;
    private WeaponData[] weaponSlots;
    private InventoryItem[] quickSlots;

    public Equipment()
    {
        armorSlots = new ArmorData[System.Enum.GetValues(typeof(BodySlot)).Length];
        //_owner = owner;
        

        //this.quickSlots = quickSlots;

        //for(int i = 0; i < this.quickSlots.Length; i++)
        //{
        //    this.quickSlots[i] = new InventoryItem();
        //}
    }

    public void EquipArmor(BodySlot bodySlot)
    {
        switch(bodySlot)
        {
            case BodySlot.Helmet:
                break;
            case BodySlot.Necklace:
                break;
            case BodySlot.Cape:
                break;
            case BodySlot.Dress:
                break;
            case BodySlot.Gloves:
                break;
            case BodySlot.RingLeft:
                break;
            case BodySlot.RingRight:
                break;
            case BodySlot.Belt:
                break;
            case BodySlot.Boots:
                break;
        }
    }

    public void EquipArmor(Armor armor, out ArmorData swappedItemData)
    {
        swappedItemData = null;

        //InventoryItem invItem = ;
        if(armorSlots[(int)armor.bodySlot].Armor != null)
        {
            swappedItemData = armorSlots[(int)armor.bodySlot];
        }
        armorSlots[(int)armor.bodySlot].Armor = armor;
    }

    public void ClearArmorSlot(BodySlot bodySlot)
    {
        armorSlots[(int)bodySlot].Armor = null;
    }

    public ArmorData[] GetArmorSlots()
    {
        return armorSlots;
    }

    public void ClearAllEquipmentSlots()
    {
        armorSlots = null;
        weaponSlots = null;
    }

    public void SetQuickSlotItem(Item consumable, int index, out InventoryItem swappedItemData, out int swappedStackSize)
    {
        swappedItemData = null;
        swappedStackSize = 0;

        //InventoryItem slot = quickSlots[index];
        //if(quickSlots[index].itemData != null)
        //{
        //    swappedItemData = slot;
        //    swappedStackSize = slot.stackSize;
        //}

        //quickSlots[index].itemData = consumable;
    }

    public void ClearQuickSlot(int index)
    {
        //quickSlots[index].stackSize = 0;
        //quickSlots[index].itemData = null;
    }

    public InventoryItem[] GetQuickSlots()
    {
        return quickSlots;
    }

    public void SetArmorColor(int arrayIndex, Color color)
    {
        if(arrayIndex == 0)
        {
            ArmorColor1 = new SerializableColor(color);
        }
        else
        {
            ArmorColor2 = new SerializableColor(color);
        }

        for(int i = 0; i < armorSlots.Length; i++)
        {
            ArmorData  armorData = armorSlots[i];

            if(armorData.Colors.Length > 0)
            {
                armorData.SetColors(arrayIndex, color);
            }
        }
    }
}