using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGolemDamageCollider : DamageCollider
{
    [SerializeField] private AIBossCharacterManager enemyGolem;

    protected override void Awake()
    {
        base.Awake();
        
        damageCollider = GetComponent<Collider>();
        enemyGolem = GetComponentInParent<AIBossCharacterManager>();
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
        damageEffect.angleHitFrom = Vector3.SignedAngle(enemyGolem.transform.forward, damageTarget.transform.forward, Vector3.up);
        
    }
}
