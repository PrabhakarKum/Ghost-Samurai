using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCollider : MonoBehaviour
{
    [Header("Collider")]
    [SerializeField] protected Collider damageCollider;
    
    [Header("Damage")]
    public float physicalDamage = 0;
    public float magicDamage = 0;
    public float fireDamage = 0;
    public float lightningDamage = 0;
    public float holyDamage = 0;
    
    [Header("Poise")]
    public float poiseDamage = 0;

    [Header("Contact Point")]
    public Vector3 contactPoint;
    
    [Header("Character Damaged")]
    protected List<CharacterManager> charactersDamaged = new List<CharacterManager>();

    [Header("Block")] 
    protected Vector3 directionFromAttackToDamageTarget;
    protected float dotValueFromAttackToDamageTarget;

    protected virtual void Awake()
    {
        
    }
    protected virtual void OnTriggerEnter(Collider collision)
    {
        CharacterManager damageTarget = collision.GetComponentInParent<CharacterManager>();
        if (damageTarget != null)
        {
            contactPoint = collision.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
            
            bool blocked = CheckForBlock(damageTarget);

            if (blocked)
                return;
            //check if we can damage this target
            DamageTarget(damageTarget);
        }
    }


    protected virtual bool CheckForBlock(CharacterManager damageTarget)
    {
        // IF THIS CHARACTER HAS ALREADY BEN DAMAGED, DO NOT PROCEED
        if(charactersDamaged.Contains(damageTarget))
            return true;

        GetBlockingDotValues(damageTarget);
        
        if (damageTarget.isBlocking && dotValueFromAttackToDamageTarget > 0.3f)
        { 
            charactersDamaged.Add(damageTarget);
           TakeBlockedDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeBlockedDamageEffect);
          
           damageEffect.physicalDamage = physicalDamage;
           damageEffect.magicDamage = magicDamage;
           damageEffect.fireDamage = fireDamage;
           damageEffect.lightningDamage = lightningDamage;
           damageEffect.holyDamage = holyDamage;
           damageEffect.poiseDamage = poiseDamage;
           damageEffect.staminaDamage = poiseDamage;
           damageEffect.contactPoint = contactPoint;
           
           // APPLY BLOCKED CHARACTER DAMAGE TO TARGET 
           damageTarget.characterEffectsManager.ProcessInstantEffect(damageEffect);
           
           return true;
        }
        
        return false;
    }

    protected virtual void GetBlockingDotValues(CharacterManager damageTarget)
    {
         directionFromAttackToDamageTarget = transform.position - damageTarget.transform.position;
         dotValueFromAttackToDamageTarget = Vector3.Dot(directionFromAttackToDamageTarget, damageTarget.transform.forward);
    }

    protected virtual void DamageTarget(CharacterManager damageTarget)
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
        damageTarget.characterEffectsManager.ProcessInstantEffect(damageEffect);
    }

    public virtual void EnableDamageCollider()
    {
        damageCollider.enabled = true;
    }
    
    public virtual void DisableDamageCollider()
    {
        damageCollider.enabled = false;
        charactersDamaged.Clear(); //WE RESET THE CHARACTER THAT HAVE BEEN HIT WHEN WE RESET THE COLLIDER, SO THEY CAN BE HIT AGAIN
    }
}
