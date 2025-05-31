using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetActionFlag : StateMachineBehaviour
{
    CharacterManager characterManager;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (characterManager == null)
        {
            characterManager = animator.GetComponentInParent<CharacterManager>();
        }
        //THIS IS CALLED WHEN AN ACTION ENDS, AND THE STATE RETURN TO EMPTY
        characterManager.isPerformingAction = false;
        characterManager.characterAnimatorManager._applyRootMotion = false;
        characterManager.characterLocomotionManager.canMove = true;
        characterManager.characterLocomotionManager.canRotate = true;
        characterManager.isJumping = false;
        characterManager.characterLocomotionManager.isRolling = false;
        characterManager.isVulnerable = false;
        characterManager.isAttacking = false;
        characterManager.isRipostable = false;
        characterManager.isBeingCriticallyDamaged = false;
        characterManager.characterAnimatorManager.DisableCanDoCombo();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
