using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "AI/States/Block State")]
public class AI_BlockState : AIState
{
    private int lastProcessedAttackID = -1;
    public override AIState Tick(AICharacterManager aiCharacter)
    {
        aiCharacter.isBlocking = true;
        GameManager gameManager = GameManager.instance;
        PlayerManager playerManager = gameManager.playerManager; // Access the player directly
        
        if (!playerManager.isAttacking && aiCharacter.isBlocking)
        {
            Debug.Log("leaving blocked states");
            aiCharacter.isBlocking = false;
            return SwitchState(aiCharacter, aiCharacter.pursueTargetState);  // Transition to patrol 
        }
        
        aiCharacter.isBlocking = true;
        
        Debug.Log("player manager current attack id: "+ playerManager.currentAttackID);
        if (playerManager.currentAttackID != lastProcessedAttackID)
        {
            lastProcessedAttackID = playerManager.currentAttackID;
            
            MeleeWeaponDamageCollider weaponCollider = playerManager.rightHandDamageCollider;

            if (weaponCollider != null)
            {
                TakeBlockedDamageEffect blockedEffect = Instantiate(WorldCharacterEffectsManager.instance.takeBlockedDamageEffect);

                blockedEffect._characterCausingDamage = playerManager;
                blockedEffect.physicalDamage = weaponCollider.physicalDamage;
                blockedEffect.magicDamage = weaponCollider.magicDamage;
                blockedEffect.fireDamage = weaponCollider.fireDamage;
                blockedEffect.lightningDamage = weaponCollider.lightningDamage;
                blockedEffect.holyDamage = weaponCollider.holyDamage;
                blockedEffect.poiseDamage = weaponCollider.poiseDamage;
                blockedEffect.staminaDamage = 0;

                blockedEffect.ProcessEffect(aiCharacter);

                Debug.Log("AI took full blocked damage.");
            }
        }

        return this;
    }
    
    protected override void ResetStateFlags(AICharacterManager aiCharacter)
    {
        base.ResetStateFlags(aiCharacter);
        aiCharacter.isBlocking = false;
    }
}
