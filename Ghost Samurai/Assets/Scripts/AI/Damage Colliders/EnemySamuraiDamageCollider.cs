using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySamuraiDamageCollider : DamageCollider
{
    [SerializeField] private AICharacterManager enemySamurai;

    protected override void Awake()
    {
        base.Awake();
        
        damageCollider = GetComponent<Collider>();
        enemySamurai = GetComponentInParent<AICharacterManager>();
    }
    
    protected override void GetBlockingDotValues(CharacterManager damageTarget)
    {
        directionFromAttackToDamageTarget = enemySamurai.transform.position - damageTarget.transform.position;
        dotValueFromAttackToDamageTarget = Vector3.Dot(directionFromAttackToDamageTarget, damageTarget.transform.forward);
    }
    
    
    protected override void DamageTarget(CharacterManager damageTarget)
    {
        base.DamageTarget(damageTarget);
        
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
        damageEffect.angleHitFrom = Vector3.SignedAngle(enemySamurai.transform.forward, damageTarget.transform.forward, Vector3.up);
        
    }
}
