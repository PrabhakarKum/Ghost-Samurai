using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public Transform slashPoint;
    public MeleeWeaponDamageCollider meleeWeaponDamageCollider;

    private void Awake()
    {
        meleeWeaponDamageCollider = GetComponentInChildren<MeleeWeaponDamageCollider>();
    }

    public void SetWeaponDamage(CharacterManager characterWieldingWeapon,WeaponItems weapon)
    {
        meleeWeaponDamageCollider.characterCausingDamage = characterWieldingWeapon;
        meleeWeaponDamageCollider.physicalDamage = weapon.physicalDamage;
        meleeWeaponDamageCollider.magicDamage = weapon.magicDamage;
        meleeWeaponDamageCollider.fireDamage = weapon.fireDamage;
        meleeWeaponDamageCollider.lightningDamage = weapon.lightningDamage;
        meleeWeaponDamageCollider.holyDamage = weapon.holyDamage;
        meleeWeaponDamageCollider.poiseDamage = weapon.poiseDamage;
        
        
        meleeWeaponDamageCollider.light_Attack_01_Modifier = weapon.light_Attack_01_Modifier;
        meleeWeaponDamageCollider.light_Attack_02_Modifier = weapon.light_Attack_02_Modifier;
        meleeWeaponDamageCollider.heavy_Attack_01_Modifier = weapon.heavy_Attack_01_Modifier;

    }
}
