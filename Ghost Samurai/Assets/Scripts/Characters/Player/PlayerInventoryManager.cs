using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventoryManager : CharacterInventoryManager
{
    public WeaponItems currentLeftHandWeapon;
    public WeaponItems currentRightHandWeapon;
    
    
    [Header("Quick Slots")] 
    public WeaponItems[] weaponsInRightHandSlots = new WeaponItems[3];
    public int rightHandWeaponIndex = 0;
    public WeaponItems[] weaponsInLeftHandSlots = new WeaponItems[3];
    public int leftHandWeaponIndex = 0;
    
    
}
