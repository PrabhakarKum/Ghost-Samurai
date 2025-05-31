using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/States/IdleState")]
public class AI_IdleState : AIState
{
    [Header("Idle Options")]
    [SerializeField] IdleStateMode idleStateMode;
    
    [Header("Patrol Options")]
    [SerializeField] float patrolPathID;
    [SerializeField] bool hasFoundClosestPointNearCharacterSpawn = false; // IF THE CHARACTER SPAWNS CLOSER TO THE SECOND POINT, START AT THE SECOND POINT
    [SerializeField] bool patrolComplete = false; //
    [SerializeField] bool repeatPatrol = false;
    public override AIState Tick(AICharacterManager aiCharacter)
    {
        if (aiCharacter.characterCombatManager.currentTarget != null)
        {
            return SwitchState(aiCharacter, aiCharacter.pursueTargetState);
        }

        // Return this state, to continue search for target (KEEP THE STATE HERE, UNTIL A TARGET IS FOUND)
        aiCharacter.aiCharacterCombatManager.FindATargetViaLineOfSight(aiCharacter);
        return this;

    }
}
