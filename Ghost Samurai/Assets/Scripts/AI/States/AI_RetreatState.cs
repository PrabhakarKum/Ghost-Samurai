using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/States/RetreatState")]
public class AI_RetreatState : AIState
{
    private string retreatAnimation = "Roll_Back"; // Make sure this animation uses root motion
    private bool hasRolledBack = false;

    public override AIState Tick(AICharacterManager aiCharacter)
    {
        // CHECK IF WE ARE PERFORMING AN ACTION(IF SO WAIT UNTIL ACTION IS COMPLETE)
        if (aiCharacter.isPerformingAction)
            return this;
        
        if (aiCharacter.characterCombatManager.currentTarget == null)
            return SwitchState(aiCharacter, aiCharacter.idleState);

        
        if (!hasRolledBack && !aiCharacter.isPerformingAction)
        {
            Retreat(aiCharacter);
            return this; // Stay in the retreat state until roll-back is done
        }

        // After retreat, perform the attack action
        if (hasRolledBack && !aiCharacter.isPerformingAction)
        {
            return SwitchState(aiCharacter, aiCharacter.pursueTargetState);
        }

        return this;
    }
    
    private void Retreat(AICharacterManager aiCharacter)
    {
        CharacterManager target = aiCharacter.characterCombatManager.currentTarget;

        Vector3 directionAway = (aiCharacter.transform.position - target.transform.position).normalized;
        directionAway.y = 0;

        Quaternion retreatRotation = Quaternion.LookRotation(directionAway);
        aiCharacter.transform.rotation = retreatRotation;

        aiCharacter.aiCharacterAnimatorManager.PlayTargetActionAnimation(retreatAnimation, true);
        hasRolledBack = true;
    }
    
    protected override void ResetStateFlags(AICharacterManager aiCharacter)
    {
        base.ResetStateFlags(aiCharacter);
        
        hasRolledBack = false;
    }
    
}
