using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEquipmentManager : CharacterEquipmentManager
{
    private PlayerManager playerManager;
    
    public WeaponModelInstantiationSlot leftHandSlot;
    public WeaponModelInstantiationSlot rightHandSlot;
    public GameObject LeftHandWeaponModel;
    public GameObject RightHandWeaponModel;

    public WeaponManager leftWeaponManager; 
    public WeaponManager rightWeaponManager;
    protected override void Awake()
    {
        base.Awake();
        playerManager = GetComponent<PlayerManager>();
        InitializedWeaponSlots();
    }

    protected override void Start()
    {
        base.Start();
        LoadWeaponsOnBothHand();
    }

    private void InitializedWeaponSlots()
    {
        WeaponModelInstantiationSlot[] weaponSlots = GetComponentsInChildren<WeaponModelInstantiationSlot>();
        foreach (var weaponSlot in weaponSlots)
        {
            if (weaponSlot.weaponSlot == weaponModelSlot.LeftHand)
            {
                leftHandSlot = weaponSlot;
            }
            else if (weaponSlot.weaponSlot == weaponModelSlot.RightHand)
            {
                rightHandSlot = weaponSlot;
            }
        }

    }

    public void LoadWeaponsOnBothHand()
    {
        LoadWeaponsOnLeftHand();
        LoadWeaponsOnRightHand();
    }

    //Left Weapon
    
    public void SwitchLeftWeapon()
    {
        bool allSlotsUnarmed = true;
    
        foreach (WeaponItems weapon in playerManager._playerInventoryManager.weaponsInLeftHandSlots)
        {
            if (weapon.itemID != WorldItemDatabase.instance.unarmedWeapons.itemID)
            {
                allSlotsUnarmed = false;
                break; // Found at least one weapon, no need to check further
            }
        }

        // If all slots are unarmed, do NOT play the swap animation and return
        if (allSlotsUnarmed)
        {
            Debug.Log("No weapons equipped. Cannot switch.");
            return;
        }
        
        playerManager._playerAnimatorManager.PlayTargetActionAnimation("Swap_Left_Weapon_01", false, false, true, true);
        WeaponItems selectedWeapon = null;
        
        int oldWeaponID = playerManager._playerInventoryManager.currentLeftHandWeapon?.itemID ?? 0;
        
        playerManager._playerInventoryManager.leftHandWeaponIndex ++;

        if (playerManager._playerInventoryManager.leftHandWeaponIndex >=  playerManager._playerInventoryManager.weaponsInLeftHandSlots.Length)
        { 
            playerManager._playerInventoryManager.leftHandWeaponIndex = 0;
        }

        selectedWeapon = playerManager._playerInventoryManager.weaponsInLeftHandSlots[playerManager._playerInventoryManager.leftHandWeaponIndex];
        
        if (selectedWeapon == null)
        {
            FindAndEquipFirstAvailableWeapon(); // Find and equip a valid weapon
            return;
        }
        
        if (selectedWeapon.itemID != WorldItemDatabase.instance.unarmedWeapons.itemID)
        {
            playerManager._playerInventoryManager.currentLeftHandWeapon = selectedWeapon;
            playerManager._playerEquipmentManager.LoadWeaponsOnLeftHand();

            // Fire the event when weapon changes
            GameEvents.OnLeftHandWeaponChanged?.Invoke(oldWeaponID, selectedWeapon.itemID);
        }
        else
        {
            // If selected weapon is "unarmed", find the first valid weapon
            FindAndEquipFirstAvailableWeapon();
        }
    }
    
    public void LoadWeaponsOnLeftHand()
    {
        if (playerManager._playerInventoryManager.currentLeftHandWeapon != null)
        {
            // REMOVE THE OLD WEAPON
            leftHandSlot.UnloadWeapon();
            
            //// BRINGING THE NEW WEAPON
            LeftHandWeaponModel = Instantiate(playerManager._playerInventoryManager.currentLeftHandWeapon.weaponModel);
            leftHandSlot.LoadWeapon(LeftHandWeaponModel);
            leftWeaponManager = LeftHandWeaponModel.GetComponent<WeaponManager>();
            leftWeaponManager.SetWeaponDamage(playerManager,playerManager._playerInventoryManager.currentLeftHandWeapon);
            
            playerManager.leftHandDamageCollider = leftWeaponManager.meleeWeaponDamageCollider;
        }
    }
    
    //Right Weapon
    public void SwitchRightWeapon()
    {
        
        bool allSlotsUnarmed = true;
    
        foreach (WeaponItems weapon in playerManager._playerInventoryManager.weaponsInRightHandSlots)
        {
            if (weapon.itemID != WorldItemDatabase.instance.unarmedWeapons.itemID)
            {
                allSlotsUnarmed = false;
                break; // Found at least one weapon, no need to check further
            }
        }

        // If all slots are unarmed, do NOT play the swap animation and return
        if (allSlotsUnarmed)
        {
            Debug.Log("No weapons equipped. Cannot switch.");
            return;
        }
        
        playerManager._playerAnimatorManager.PlayTargetActionAnimation("Swap_Right_Weapon_01", false, false, true, true);
        WeaponItems selectedWeapon = null;
        
        int oldWeaponID = playerManager._playerInventoryManager.currentRightHandWeapon?.itemID ?? 0;
        
        playerManager._playerInventoryManager.rightHandWeaponIndex ++;

        if (playerManager._playerInventoryManager.rightHandWeaponIndex >=  playerManager._playerInventoryManager.weaponsInRightHandSlots.Length)
        { 
            playerManager._playerInventoryManager.rightHandWeaponIndex = 0;
        }

        selectedWeapon = playerManager._playerInventoryManager.weaponsInRightHandSlots[playerManager._playerInventoryManager.rightHandWeaponIndex];
        
        if (selectedWeapon == null)
        {
            FindAndEquipFirstAvailableWeapon(); // Find and equip a valid weapon
            return;
        }
        
        if (selectedWeapon.itemID != WorldItemDatabase.instance.unarmedWeapons.itemID)
        {
            playerManager._playerInventoryManager.currentRightHandWeapon = selectedWeapon;
            playerManager._playerEquipmentManager.LoadWeaponsOnRightHand();

            // Fire the event when weapon changes
            GameEvents.OnRightHandWeaponChanged?.Invoke(oldWeaponID, selectedWeapon.itemID);
        }
        else
        {
            // If selected weapon is "unarmed", find the first valid weapon
            FindAndEquipFirstAvailableWeapon();
        }
    }
    
    
    // Finds and equips the first available weapon that is not "unarmed."
    private void FindAndEquipFirstAvailableWeapon()
    {
        for (int i = 0; i < playerManager._playerInventoryManager.weaponsInRightHandSlots.Length; i++)
        {
            WeaponItems weapon = playerManager._playerInventoryManager.weaponsInRightHandSlots[i];

            if (weapon != null && weapon.itemID != WorldItemDatabase.instance.unarmedWeapons.itemID)
            {
                playerManager._playerInventoryManager.rightHandWeaponIndex = i;
                playerManager._playerInventoryManager.currentRightHandWeapon = weapon;
                playerManager._playerEquipmentManager.LoadWeaponsOnRightHand();
                
                GameEvents.OnRightHandWeaponChanged?.Invoke(0, weapon.itemID);
                return; 
            }
        }
    }

    
    public void LoadWeaponsOnRightHand()
    {
        if (playerManager._playerInventoryManager.currentRightHandWeapon != null)
        {
            // REMOVE THE OLD WEAPON
            rightHandSlot.UnloadWeapon();
            
            //// BRINGING THE NEW WEAPON
            RightHandWeaponModel = Instantiate(playerManager._playerInventoryManager.currentRightHandWeapon.weaponModel);
            rightHandSlot.LoadWeapon(RightHandWeaponModel);
            
            //GET WEAPON MANAGER
            rightWeaponManager = RightHandWeaponModel.GetComponent<WeaponManager>();
            rightWeaponManager.SetWeaponDamage(playerManager,playerManager._playerInventoryManager.currentRightHandWeapon);
            
            Transform slashPoint = rightWeaponManager.slashPoint;
            playerManager.GetComponent<SwordComboEffects>().SetSlashPoint(slashPoint);
            
            playerManager.rightHandDamageCollider = rightWeaponManager.meleeWeaponDamageCollider;
        }
    }
    
    //DAMAGE COLLIDER
    public void OpenDamageCollider()
    {
        //OPEN RIGHT WEAPON DAMAGE COLLIDER
        if (playerManager.isUsingRightHand)
        {
            rightWeaponManager.meleeWeaponDamageCollider.EnableDamageCollider();
            playerManager.characterSoundFXManager.PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSfxFromArray(playerManager._playerInventoryManager.currentRightHandWeapon.whooshesSFX));
        }
        //OPEN LEFT WEAPON DAMAGE COLLIDER
        else if (playerManager.isUsingLeftHand)
        {
            leftWeaponManager.meleeWeaponDamageCollider.EnableDamageCollider();
        }
        
        //PLAY WOOSH SFX
    }
    
    public void CloseDamageCollider()
    {
        //OPEN RIGHT WEAPON DAMAGE COLLIDER
        if (playerManager.isUsingRightHand)
        {
            rightWeaponManager.meleeWeaponDamageCollider.DisableDamageCollider();
        }
        //OPEN LEFT WEAPON DAMAGE COLLIDER
        else if (playerManager.isUsingLeftHand)
        {
            leftWeaponManager.meleeWeaponDamageCollider.DisableDamageCollider();
        }
        
        
    }
}
