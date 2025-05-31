using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character Actions/Weapon Actions/Light Attack Actions")] 
public class LightAttackWeaponItemAction : WeaponItemAction
{
    [SerializeField] private string light_Attack_01 = "Right_Light_Attack_01";
    [SerializeField] private string light_Attack_02 = "Right_Light_Attack_02";
    public override void AttemptToPerformAction(PlayerManager playerPerformingAction, WeaponItems weaponPerformingAction)
    {
        base.AttemptToPerformAction(playerPerformingAction, weaponPerformingAction);

        if (playerPerformingAction.currentStamina <= 0)
            return;
        
        if(!playerPerformingAction._playerLocomotionManager.isGrounded)
            return;

        playerPerformingAction.isAttacking = true;
        
        playerPerformingAction.characterCombatManager.AttemptCriticalAttack();
        
        PerformLightAttack(playerPerformingAction, weaponPerformingAction);
    }

    private void PerformLightAttack(PlayerManager playerPerformingAction, WeaponItems weaponPerformingAction)
    {
        // IF WE ARE ATTACKING CURRENTLY, AND WE CAN COMBO,PERFORM COMBO ATTACK
        if (playerPerformingAction._playerCombatManager.canComboWithRightHandWeapon && playerPerformingAction.isPerformingAction)
        {
            playerPerformingAction._playerCombatManager.canComboWithRightHandWeapon = false;

            if (playerPerformingAction._playerCombatManager.lastAttackAnimationPerformed == light_Attack_01)
            {
                playerPerformingAction._playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.LightAttack02 ,light_Attack_02, true);
            }
            else
            {
                playerPerformingAction._playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.LightAttack01 ,light_Attack_01, true);
            }
            
        }
        //OTHERWISE, JUST PERFORM REGULAR ATTACK
        else if(!playerPerformingAction.isPerformingAction)
        {
            playerPerformingAction._playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.LightAttack01 ,light_Attack_01, true);
        }
    }
}
