using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAnimatorManager : CharacterAnimatorManager
{
    private PlayerManager _playerManager;

    protected override void Awake()
    {
        base.Awake();
        _playerManager = GetComponent<PlayerManager>();
    }
    private void OnAnimatorMove()
    {
        if (_playerManager._playerAnimatorManager._applyRootMotion)
        {
            Vector3 velocity = _playerManager.animator.deltaPosition;
            _playerManager.characterController.Move(velocity);
            _playerManager.transform.rotation *= _playerManager.animator.deltaRotation;
        }
    }
    
    // ANIMATION EVENT CALLS
    public override void EnableCanDoCombo()
    {
        base.EnableCanDoCombo();
        
        if (_playerManager.isUsingRightHand)
        {
           _playerManager._playerCombatManager.canComboWithRightHandWeapon = true;
        }
        else
        {
            // ENABLE LEFT HAND COMBO WEAPON
        }
    }
    
    public override void DisableCanDoCombo()
    {
        base.DisableCanDoCombo();
        
        _playerManager._playerCombatManager.canComboWithRightHandWeapon = false;
        //canComboWithLeftHandWeapon = false;
    }
}
