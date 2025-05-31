using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character Actions/Weapon Actions/Heavy Attack Actions")] 
public class HeavyAttackWeaponItemAction : WeaponItemAction
{
    [SerializeField] private string heavy_Attack_01 = "Right_Heavy_Attack_01";
    public override void AttemptToPerformAction(PlayerManager playerPerformingAction, WeaponItems weaponPerformingAction)
    {
        base.AttemptToPerformAction(playerPerformingAction, weaponPerformingAction);

        if (playerPerformingAction.currentStamina <= 0)
            return;
        
        if(!playerPerformingAction._playerLocomotionManager.isGrounded)
            return;
        
        playerPerformingAction.isAttacking = true;
        
        PerformHeavyAttack(playerPerformingAction, weaponPerformingAction);
    }

    private void PerformHeavyAttack(PlayerManager playerPerformingAction, WeaponItems weaponPerformingAction)
    {
        // IF WE ARE ATTACKING CURRENTLY, AND WE CAN COMBO,PERFORM COMBO ATTACK
        if (playerPerformingAction._playerCombatManager.canComboWithRightHandWeapon && playerPerformingAction.isPerformingAction)
        {
            playerPerformingAction._playerCombatManager.canComboWithRightHandWeapon = false;

            if (playerPerformingAction._playerCombatManager.lastAttackAnimationPerformed == heavy_Attack_01)
            {
                playerPerformingAction._playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.HeavyAttack01 ,heavy_Attack_01, true);
            }
            else
            {
                playerPerformingAction._playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.HeavyAttack01 ,heavy_Attack_01, true);
            }
            
        }
        //OTHERWISE, JUST PERFORM REGULAR ATTACK
        else if(!playerPerformingAction.isPerformingAction)
        {
            playerPerformingAction._playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.HeavyAttack01 , heavy_Attack_01, true);
        }
    }
}
