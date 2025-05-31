using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Character Actions/Weapon Actions/Block Melee Actions")] 
public class BlockMeleeWeaponAction : WeaponItemAction
{
    public override void AttemptToPerformAction(PlayerManager playerPerformingAction, WeaponItems weaponPerformingAction)
    {
        base.AttemptToPerformAction(playerPerformingAction, weaponPerformingAction);

        if (!playerPerformingAction._playerCombatManager.canBlock)
            return;

        // CHECK FOR  ATTACK STATUS
        if (playerPerformingAction.isAttacking)
        {
            playerPerformingAction.isBlocking = false;
            return;
        }
        
        if(playerPerformingAction.isBlocking)
            return;
        
        playerPerformingAction.isBlocking = true;
        
    }
}
