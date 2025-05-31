using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIGolemCombatManager : AICharacterCombatManager
{
    [Header("Damage Colliders")]
    [SerializeField] EnemyGolemDamageCollider golemRightHandDamageCollider;
    [SerializeField] EnemyGolemDamageCollider golemLeftHandDamageCollider;
    [SerializeField] EnemyGolemDamageCollider golemRightLegDamageCollider;
    [SerializeField] EnemyGolemDamageCollider golemLeftLegDamageCollider;

    [Header("Damage")]
    [SerializeField] private int baseDamage = 25;
    [SerializeField] private float attack01DamageModifier = 1.2f;
    [SerializeField] private float attack02DamageModifier = 1.3f;
    [SerializeField] private float attack03DamageModifier = 2f;
    [SerializeField] private float attack04DamageModifier = 2f;
    [SerializeField] private float attack05DamageModifier = 2f;

    public void SetAttack01Damage()
    {
        golemLeftHandDamageCollider.physicalDamage = baseDamage * attack01DamageModifier;
    }
    
    public void SetAttack02Damage()
    {
        golemLeftHandDamageCollider.physicalDamage = baseDamage * attack02DamageModifier;
        golemRightHandDamageCollider.physicalDamage = baseDamage * attack02DamageModifier;
        
    }
    
    public void SetAttack03Damage()
    {
        golemLeftHandDamageCollider.physicalDamage = baseDamage * attack03DamageModifier;
        golemRightHandDamageCollider.physicalDamage = baseDamage * attack03DamageModifier;
    }
    
    public void SetAttack04Damage()
    {
        golemLeftLegDamageCollider.physicalDamage = baseDamage * attack04DamageModifier;
    }
    
    public void SetAttack05Damage()
    {
        golemRightLegDamageCollider.physicalDamage = baseDamage * attack05DamageModifier;
    }

    public void OpenRightHandDamageCollider()
    {
        golemRightHandDamageCollider.EnableDamageCollider();
        
    }
    
    public void OpenLeftHandDamageCollider()
    {
        golemLeftHandDamageCollider.EnableDamageCollider();
    }
    
    public void OpenLeftLegDamageCollider()
    {
        golemLeftLegDamageCollider.EnableDamageCollider();
    }
    
    public void OpenRightLegDamageCollider()
    {
        golemRightLegDamageCollider.EnableDamageCollider();
    }
    
    public void CloseRightHandDamageCollider()
    {
        golemRightHandDamageCollider.DisableDamageCollider();
    }
    
    public void CloseLeftHandDamageCollider()
    {
        golemLeftHandDamageCollider.DisableDamageCollider();
    }
    
    public void CloseLeftLegDamageCollider()
    {
        golemLeftLegDamageCollider.DisableDamageCollider();
    }
    
    public void CloseRightLegDamageCollider()
    {
        golemRightLegDamageCollider.DisableDamageCollider();
    }
}
