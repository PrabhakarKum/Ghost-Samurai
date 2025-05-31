using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class WeaponItems : Item
{
    // Animator Controller Override(change attack animation based on weapon you are using)

    [Header("Weapon Model")]
    public GameObject weaponModel;

    [Header("Weapon Requirements")] 
    public int strengthREQ;
    public int dexterityREQ;

    [Header("Weapon Base Damage")]
    public int physicalDamage;
    public int magicDamage;
    public int fireDamage;
    public int lightningDamage;
    public int holyDamage;

    [Header("Weapon Base Poise Damage")]
    public float poiseDamage = 10;

    [Header("Attack Modifiers")]
    public float light_Attack_01_Modifier = 1.1f;
    public float light_Attack_02_Modifier = 1.5f;
    public float heavy_Attack_01_Modifier = 2f;
    public float riposte_Attack_01_Modifier = 3.3f;

    [Header("Stamina Costs Modifiers")]
    public float baseStaminaCost = 7;
    public float lightAttackStaminaCostMultiplier = 0.8f;
    public float heavyAttackStaminaCostMultiplier = 0.8f;

    [Header("Weapon Blocking Absorptions")]
    public float physicalBaseDamageAbsorption = 50;
    public float magicBaseDamageAbsorption = 50;
    public float fireBaseDamageAbsorption = 50;
    public float holyBaseDamageAbsorption = 50;
    public float lightningBaseDamageAbsorption = 50;
    public float stability = 50; //REDUCES STAMINA LOST FROM BLOCK
    
    

    [Header("Actions")] 
    public WeaponItemAction mouse_1_Action; //ONE HAND RIGHT BUMPER ACTION 
    public WeaponItemAction hold_Mouse_1_Action; //ONE HAND RIGHT TRIGGER ACTION 
    public WeaponItemAction mouse_2_Action;  //ONE HAND RIGHT BUMPER ACTION
    
    //BLOCKING SOUNDS 
    [Header("SFX")]
    public AudioClip[] whooshesSFX;
    public AudioClip[] blockingSFX;
}
