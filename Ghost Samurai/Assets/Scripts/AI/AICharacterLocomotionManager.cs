using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICharacterLocomotionManager : CharacterLocomotionManager
{
    public void RotateTowardsAgent(AICharacterManager aiCharacter)
    {
        if (aiCharacter.isAIMoving)
        {
            aiCharacter.transform.rotation = aiCharacter.navMeshAgent.transform.rotation;
        }
    }
}
