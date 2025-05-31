using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character Actions/Weapon Actions/Test Actions")] 
public class WeaponItemAction : ScriptableObject
{
    public int actionID;
    public virtual void AttemptToPerformAction(PlayerManager playerPerformingAction, WeaponItems weaponPerformingAction)
    {
        playerPerformingAction._playerCombatManager.currentWeaponBeingUsed = weaponPerformingAction;
    }
}
