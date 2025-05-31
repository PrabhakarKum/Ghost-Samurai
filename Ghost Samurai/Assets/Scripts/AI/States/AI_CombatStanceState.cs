using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.Random;

[CreateAssetMenu(menuName = "AI/States/Combat Stance State")]
public class AI_CombatStanceState : AIState
{
    [Header("Attacks")] 
    public List<AICharacterAttackAction> aiCharacterAttacks; // A list of all attacks this character can do
    protected List<AICharacterAttackAction> potentialAttacks;  // all attacks possible in this situation(based on angle, distance etc)
    protected bool hasAttacked = false;
    private AICharacterAttackAction previousAttackAction;
    private AICharacterAttackAction choosenAttackAction;
    
    [Header("Combo")]
    [SerializeField] protected bool canPerformCombo = false;
    [SerializeField] protected int chanceToPerformCombo = 25; // the chance (in percent) of the character to perform a combo on the next aatck
    [SerializeField] protected bool hasRolledForCombo = false;
    
    [Header("Engagement Distance")]
    [SerializeField] public float maximumEngagementDistance = 15f; //The distance we have to be away from the target before we enter the pursue target state


    public override AIState Tick(AICharacterManager aiCharacter)
    {
        if (aiCharacter.isPerformingAction)
            return this;
        
        if(!aiCharacter.navMeshAgent.enabled)
            aiCharacter.navMeshAgent.enabled = true;

        if (aiCharacter.aiCharacterCombatManager.enablePivot)
        {
            if (!aiCharacter.isAIMoving)
            {
                if (aiCharacter.aiCharacterCombatManager.viewableAngle < -30 || aiCharacter.aiCharacterCombatManager.viewableAngle > 30)
                {
                    aiCharacter.aiCharacterCombatManager.PivotTowardsTarget(aiCharacter);
                }
            }
        }
        
        // ROTATE TO FACE OUR TARGET
        aiCharacter.aiCharacterCombatManager.RotateTowardsAgent(aiCharacter);
        
        // IF OUR TARGET IS NO LONGER PRESENT ,  SWITCH BACK TO IDLE
        if(aiCharacter.aiCharacterCombatManager.currentTarget == null)
            return SwitchState(aiCharacter, aiCharacter.idleState);
        
        // IF WE DO NOT HAVE AN ATTACK, GET NEW ATTACK
        if (!hasAttacked)
        {
            GetNewAttack(aiCharacter);
        }
        else
        {
            aiCharacter.attackState.currentAttack = choosenAttackAction;
            
            return SwitchState(aiCharacter, aiCharacter.attackState);
        }
        
        //IF WE OUR OUTSIDE THE COMBAT ENGAGEMENT DISTANCE, SWITCH TO PURSUE TARGET STATE
        if(aiCharacter.aiCharacterCombatManager.distanceFromTarget > maximumEngagementDistance)
            return SwitchState(aiCharacter, aiCharacter.pursueTargetState);
        
        NavMeshPath path = new NavMeshPath();
        aiCharacter.navMeshAgent.CalculatePath(aiCharacter.aiCharacterCombatManager.currentTarget.transform.position, path);
        aiCharacter.navMeshAgent.SetPath(path);

        return this;
    }

    protected virtual void GetNewAttack(AICharacterManager aiCharacter)
    {
        potentialAttacks = new List<AICharacterAttackAction>();

        foreach (var potentialAttack in aiCharacterAttacks)
        {
            // IF WE ARE TOO CLOSE FOR THIS ATTACK, CHECK THE NEXT
            if (potentialAttack.minimumAttackDistance > aiCharacter.aiCharacterCombatManager.distanceFromTarget)
                continue;
            
            // IF WE ARE TOO FAR FOR THIS ATTACK, CHECK THE NEXT
            if(potentialAttack.maximumAttackDistance < aiCharacter.aiCharacterCombatManager.distanceFromTarget)
                continue;
            
            // IF THE TARGET IS OUTSIDE THE MINIMUM FOV, CHECK THE NEXT
            if(potentialAttack.minimumAttackAngle > aiCharacter.aiCharacterCombatManager.viewableAngle)
                continue;
            
            // IF THE TARGET IS OUTSIDE THE MAXIMUM FOV, CHECK THE NEXT
            if(potentialAttack.maximumAttackAngle < aiCharacter.aiCharacterCombatManager.viewableAngle)
                continue;

            potentialAttacks.Add(potentialAttack);
        }

        if (potentialAttacks.Count <= 0)
            return;

        var totalWeight = 0;

        foreach (var attack in potentialAttacks)
        {
            totalWeight += attack.attackWeight;
        }
        
        var randomWeightValue = Random.Range(1, totalWeight + 1);
        var processedWeight = 0;

        foreach (var attack in potentialAttacks)
        {
            processedWeight += attack.attackWeight;
            if (randomWeightValue <= processedWeight)
            {
                choosenAttackAction = attack;
                previousAttackAction = choosenAttackAction;
                hasAttacked = true;
                return;
            }
        }
    }

    protected virtual bool RollForOutcomeChance(int outcomeChance)
    {
        bool outcomeWillBePerformed = false;
        int randomPercentChance = Range(0, 100);

        if (randomPercentChance < outcomeChance)
            outcomeWillBePerformed = true;
        
        return outcomeWillBePerformed;
    }

    protected override void ResetStateFlags(AICharacterManager aiCharacter)
    {
        base.ResetStateFlags(aiCharacter);
        
        hasAttacked = false;
        hasRolledForCombo = false;
    }

}
