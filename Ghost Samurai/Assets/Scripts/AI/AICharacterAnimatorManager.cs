using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICharacterAnimatorManager : CharacterAnimatorManager
{ 
    private AICharacterManager aiCharacter;
    
    protected override void Awake()
    {
        base.Awake();
        
        aiCharacter = GetComponent<AICharacterManager>();
    }
    
    private void OnAnimatorMove()
    {
        if(!aiCharacter.aiCharacterLocomotionManager.isGrounded)
            return;
        
        Vector3 velocity = aiCharacter.animator.deltaPosition;
        
        velocity.y = 0;
        
        aiCharacter.characterController.Move(velocity);
        aiCharacter.transform.rotation *= aiCharacter.animator.deltaRotation;
    }
}
