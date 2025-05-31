using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/States/Attack State")]
public class AI_AttackState : AIState
{
    [Header("Current Attack")]
    [HideInInspector] public AICharacterAttackAction currentAttack;
    [HideInInspector] public bool willPerformCombo = false;
    
    
    [Header("State Flags")]
    protected bool hasPerformedAttack = false;
    protected bool hasPerformedCombo = false;
    
    [Header("Pivot After Attack")]
    [SerializeField] protected bool pivotAfterAttack = false;

    [Header("Retreat Cooldown")]
    [SerializeField] private float retreatCooldown = 5f; // Time in seconds before it can retreat again
    private float lastRetreatTime = -10f;
    
    public override AIState Tick(AICharacterManager aiCharacter)
    {
        if(aiCharacter.aiCharacterCombatManager.currentTarget == null)
            return SwitchState(aiCharacter, aiCharacter.pursueTargetState);

        if (aiCharacter.aiCharacterCombatManager.currentTarget.isDead)
        {
            return SwitchState(aiCharacter, aiCharacter.idleState);
        }
            
        
        aiCharacter.aiCharacterCombatManager.RotateTowardsTargetWhileAttacking(aiCharacter);
        
        aiCharacter.characterAnimatorManager.UpdateAnimatorMovementParameters(0,0,false);
        
         if(aiCharacter.navMeshAgent.stoppingDistance < 0.5f || aiCharacter.aiCharacterCombatManager.distanceFromTarget < 0.7f ) // Example threshold for "too close"
        {
            // Transition to retreat state
            if (Time.time - lastRetreatTime >= retreatCooldown)
            {
                // Transition to retreat state
                lastRetreatTime = Time.time; // Update the last retreat time
                return SwitchState(aiCharacter, aiCharacter.retreatState);
            }
        }
        
        // KNOWING IF THE PLAYER IS ATTACKING OR NOT
        GameManager gameManager = GameManager.instance;
        PlayerManager playerManager = gameManager.playerManager; // Access the player directly
        
        if (playerManager.isAttacking && !aiCharacter.isBlocking && (aiCharacter.aiCharacterCombatManager.viewableAngle < -30 || aiCharacter.aiCharacterCombatManager.viewableAngle > 30))
        {
            return SwitchState(aiCharacter, aiCharacter.blockState);  // Transition to Block State
        }

        if (willPerformCombo && !hasPerformedCombo)
        {
            if (currentAttack.comboAction != null)
            {
                // hasPerformedCombo = true;
                // currentAttack.comboAction.AttemptToPerformAction(aiCharacter);
            }
        }
        
        if(aiCharacter.isPerformingAction)
            return this;

        if (!hasPerformedAttack)
        {
            // IF WE ARE STILL RECOVERING FROM AN ACTION, WAIT BEFORE PERFORMING ANOTHER
            if (aiCharacter.aiCharacterCombatManager.actionRecoveryTimer > 0)
                return this;
            
            PerformAttack(aiCharacter);
            
            // RETURN TO THE TOP, SO IF WE HAVE A COMBO WE PROCESS THAT WHEN WE ARE ABLE
            return this;
        }
        
        if(pivotAfterAttack)
            aiCharacter.aiCharacterCombatManager.PivotTowardsTarget(aiCharacter);
        
        return SwitchState(aiCharacter, aiCharacter.combatStance);
    }

    protected void PerformAttack(AICharacterManager aiCharacter)
    {
        hasPerformedAttack = true;
        currentAttack.AttemptToPerformAction(aiCharacter);
        aiCharacter.aiCharacterCombatManager.actionRecoveryTimer = currentAttack.actionRecoveryTime;
    }

    protected override void ResetStateFlags(AICharacterManager aiCharacter)
    {
        base.ResetStateFlags(aiCharacter);
        
        hasPerformedCombo = false;
        hasPerformedAttack = false;
    }
}
