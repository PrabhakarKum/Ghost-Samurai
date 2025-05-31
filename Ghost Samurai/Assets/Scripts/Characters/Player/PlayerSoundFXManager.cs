using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundFXManager : CharacterSoundFXManager
{
    
    PlayerManager playerManager;

    protected override void Awake()
    {
        base.Awake();
        playerManager = GetComponent<PlayerManager>();
    }

    public override void PlayBlockSFX()
    {
        PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSfxFromArray(playerManager._playerCombatManager.currentWeaponBeingUsed.blockingSFX));
    }
    
}
