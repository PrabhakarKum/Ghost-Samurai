using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIEnemySamuraiCombatManager : AICharacterCombatManager
{
    [Header("Damage Colliders")]
    [SerializeField] EnemySamuraiDamageCollider swordDamageCollider;

    [Header("Damage")]
    [SerializeField] private int baseDamage = 25;
    [SerializeField] private float basePoiseDamage = 15f;
    [SerializeField] private float attack01DamageModifier = 1.2f;
    [SerializeField] private float attack02DamageModifier = 1.3f;
    [SerializeField] private float attack04DamageModifier = 1.4f;
    [SerializeField] private float attack03DamageModifier = 2f;   //Long Attack Sequence
    [SerializeField] private float attack05DamageModifier = 2f;   //Long Attack Sequence
    [SerializeField] private float attack06DamageModifier = 1.7f;

    public void SetAttack01Damage()
    {
        swordDamageCollider.physicalDamage = baseDamage * attack01DamageModifier;
        swordDamageCollider.poiseDamage = basePoiseDamage * attack01DamageModifier;
    }
    
    public void SetAttack02Damage()
    {
        swordDamageCollider.physicalDamage = baseDamage * attack02DamageModifier;
        swordDamageCollider.poiseDamage = basePoiseDamage * attack02DamageModifier;
    }
    
    public void SetAttack03Damage()
    {
        swordDamageCollider.physicalDamage = baseDamage * attack03DamageModifier;
        swordDamageCollider.poiseDamage = basePoiseDamage * attack03DamageModifier;
    }
    public void SetAttack04Damage()
    {
        swordDamageCollider.physicalDamage = baseDamage * attack04DamageModifier;
        swordDamageCollider.poiseDamage = basePoiseDamage * attack04DamageModifier;
    }
    public void SetAttack05Damage()
    {
        swordDamageCollider.physicalDamage = baseDamage * attack05DamageModifier;
        swordDamageCollider.poiseDamage = basePoiseDamage * attack05DamageModifier;
    }
    public void SetAttack06Damage()
    {
        swordDamageCollider.physicalDamage = baseDamage * attack06DamageModifier;
        swordDamageCollider.poiseDamage = basePoiseDamage * attack06DamageModifier;
    }

    public void OpenSwordDamageCollider()
    {
        swordDamageCollider.EnableDamageCollider();
        aiCharacter.characterSoundFXManager.PlayAttackWhooshSoundFX();
    }
    
    public void CloseSwordDamageCollider()
    {
        swordDamageCollider.DisableDamageCollider();
    }

}
