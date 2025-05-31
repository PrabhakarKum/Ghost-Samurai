using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "AI/States/PursueTargetState")]
public class AI_PursueTargetState : AIState
{
    
    public override AIState Tick(AICharacterManager aiCharacter)
    {
        // CHECK IF WE ARE PERFORMING AN ACTION(IF SO WAIT UNTIL ACTION IS COMPLETE)
        if (aiCharacter.isPerformingAction)
            return this;
        
        // IF WE DON'T HAVE A TARGET, RETURN TO IDLE STATE
        if(aiCharacter.aiCharacterCombatManager.currentTarget == null)
            return SwitchState(aiCharacter, aiCharacter.idleState);
        
        // MAKE SURE TO ENABLE NAVMESH AGENT
        if(!aiCharacter.navMeshAgent.enabled)
            aiCharacter.navMeshAgent.enabled = true;

        if (aiCharacter.aiCharacterCombatManager.enablePivot)
        {
            if (aiCharacter.aiCharacterCombatManager.viewableAngle < aiCharacter.aiCharacterCombatManager.minimumFOV
                || aiCharacter.aiCharacterCombatManager.viewableAngle > aiCharacter.aiCharacterCombatManager.maximumFOV)
            {
                aiCharacter.aiCharacterCombatManager.PivotTowardsTarget(aiCharacter);
            }
        }
        // IF OUR TARGET GOES OUTSIDE THE CHARACTERS FOV, PIVOT TO FACE THEM
        
        
        aiCharacter.aiCharacterLocomotionManager.RotateTowardsAgent(aiCharacter);

        //
        if (aiCharacter.aiCharacterCombatManager.distanceFromTarget <= aiCharacter.navMeshAgent.stoppingDistance)
            return SwitchState(aiCharacter, aiCharacter.combatStance);
        
        // PURSUE THE TARGET
        NavMeshPath path = new NavMeshPath();
        aiCharacter.navMeshAgent.CalculatePath(aiCharacter.aiCharacterCombatManager.currentTarget.transform.position, path);
        aiCharacter.navMeshAgent.SetPath(path);
        
        return this;


    }
}
