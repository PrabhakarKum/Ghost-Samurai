using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeaponDamageCollider : DamageCollider
{
    [Header("Attacking Character")] 
    public CharacterManager characterCausingDamage;

    [Header("Weapon Attack Modifiers")] 
    public float light_Attack_01_Modifier;
    public float light_Attack_02_Modifier;
    public float heavy_Attack_01_Modifier;

    protected override void Awake()
    {
        base.Awake();
        if (damageCollider == null)
        {
            damageCollider = GetComponent<Collider>();
        }
        
        damageCollider.enabled = false; // MELEE WEAPON COLLIDERS SHOULD BE DISABLED AT START, ONLY ALLOW WHEN ANIMATION ALLOW
    }

    protected override void OnTriggerEnter(Collider collision)
    {
        CharacterManager damageTarget = collision.GetComponentInParent<CharacterManager>();
        // WE DO NOT WANT TO DAMAGE OURSELVES
        if (damageTarget == characterCausingDamage)
            return;
        
        if (damageTarget != null)
        {
            contactPoint = collision.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
            //check if we can damage this target
            DamageTarget(damageTarget);
        }
    }

    protected override void DamageTarget(CharacterManager damageTarget)
    {
        if (charactersDamaged.Contains(damageTarget))
            return;
        
        charactersDamaged.Add(damageTarget);
        
        TakeDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeDamageEffect);
        damageEffect.physicalDamage = physicalDamage;
        damageEffect.magicDamage = magicDamage;
        damageEffect.fireDamage = fireDamage;
        damageEffect.lightingDamage = lightningDamage;
        damageEffect.holyDamage = holyDamage;
        damageEffect.poiseDamage = poiseDamage;
        damageEffect.contactPoint = contactPoint;
        damageEffect.angleHitFrom = Vector3.SignedAngle(characterCausingDamage.transform.forward, damageTarget.transform.forward, Vector3.up);
        
        switch (characterCausingDamage.characterCombatManager.currentAttackType)
        {
            case AttackType.LightAttack01:
                ApplyAttackDamageModifiers(light_Attack_01_Modifier, damageEffect);
                break;
            case AttackType.LightAttack02:
                ApplyAttackDamageModifiers(light_Attack_02_Modifier, damageEffect);
                break;
            case AttackType.HeavyAttack01:
                ApplyAttackDamageModifiers(heavy_Attack_01_Modifier, damageEffect);
                break;
            default:
                break;
        }
        
        damageTarget.characterEffectsManager.ProcessInstantEffect(damageEffect);
    }

    private void ApplyAttackDamageModifiers(float modifier, TakeDamageEffect damageEffect)
    {
        damageEffect.physicalDamage *= modifier;
        damageEffect.magicDamage *= modifier;
        damageEffect.fireDamage *= modifier;
        damageEffect.lightingDamage *= modifier;
        damageEffect.holyDamage *= modifier;
        damageEffect.poiseDamage *= modifier;
        
        //IF ATTACK IS FULLY CHARGED HEAVY, MULTIPLY BY FULL CHARGE AFTER NORMAL MODIFIER HAVE BEEN CALCULATED
    }

    protected override void GetBlockingDotValues(CharacterManager damageTarget)
    {
        directionFromAttackToDamageTarget = characterCausingDamage.transform.position - damageTarget.transform.position;
        dotValueFromAttackToDamageTarget = Vector3.Dot(directionFromAttackToDamageTarget, damageTarget.transform.forward);
    }
}
