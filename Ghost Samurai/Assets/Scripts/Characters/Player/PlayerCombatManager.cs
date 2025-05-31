using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatManager : CharacterCombatManager
{
    private PlayerManager _playerManager;
    public WeaponItems currentWeaponBeingUsed;

    [Header("Flags")] 
    public bool canComboWithRightHandWeapon = false;
    //public bool canComboWithLeftHandWeapon = false;
    protected override void Awake()
    {
        base.Awake();
        _playerManager = GetComponent<PlayerManager>();
    }

    protected override void Update()
    {
        base.Update();
    }

    public void PerformWeaponBasedAction(WeaponItemAction weaponAction, WeaponItems weaponPerformingAction)
    {
        weaponAction.AttemptToPerformAction(_playerManager, weaponPerformingAction);
    }

    public virtual void DrainStaminaBasedOnAttack()
    {
        if(currentWeaponBeingUsed == null)
            return;
        
        float staminaDrained = 0;
        switch (currentAttackType)
        {
            case AttackType.LightAttack01:
                staminaDrained = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.lightAttackStaminaCostMultiplier;
                break;
            case AttackType.LightAttack02:
                staminaDrained = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.lightAttackStaminaCostMultiplier;
                break;
            case AttackType.HeavyAttack01:
                staminaDrained = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.heavyAttackStaminaCostMultiplier;
                break;
            default:
                break;
        }
        _playerManager.currentStamina -= Mathf.RoundToInt(staminaDrained);
    }

    public override void SetTarget(CharacterManager newTarget)
    {
        base.SetTarget(newTarget);
        
        PlayerCamera.instance.SetLockCameraHeight();
        
    }


    public override void AttemptRiposte(CharacterManager characterManager, RaycastHit hit)
    {
        
        CharacterManager damagedCharacter = hit.transform.gameObject.GetComponent<CharacterManager>();
        
        if(damagedCharacter == null)
            return;
        
        if(!damagedCharacter.isRipostable)
            return;
        
        if(damagedCharacter.isBeingCriticallyDamaged)
            return;

        MeleeWeaponItem riposteWeapon;
        MeleeWeaponDamageCollider riposteCollider;
        
        riposteWeapon = _playerManager._playerInventoryManager.currentRightHandWeapon as MeleeWeaponItem;
        riposteCollider = _playerManager._playerEquipmentManager.rightWeaponManager.meleeWeaponDamageCollider;
        
        
        _playerManager._playerAnimatorManager.PlayTargetActionAnimationInstantly("Riposte_Attack", true);
        
        // WHILE PERFORMING A CRITICAL STRIKE, YOU CANNOT  BE DAMAGED
        characterManager.isVulnerable = true;

        // CREATING A NEW  DAMAGE EFFECT FOR THIS TYPE OF DAMAGE
        TakeCriticalDamageEffect damageEffect =
            Instantiate(WorldCharacterEffectsManager.instance.takeCriticalDamageEffect);
        
        
        // APPLYING ALL THE DAMAGE STATS FROM THE COLLIDER TO THE DAMAGE EFFECT
        damageEffect.physicalDamage = riposteCollider.physicalDamage;
        damageEffect.holyDamage = riposteCollider.holyDamage;
        damageEffect.fireDamage = riposteCollider.fireDamage;
        damageEffect.magicDamage = riposteCollider.magicDamage;
        damageEffect.poiseDamage = riposteCollider.poiseDamage;
        damageEffect.lightingDamage = riposteCollider.lightningDamage;
        
        // MULTIPLYING DAMAGE BY WEAPON RIPOSTE MODIFIER
        damageEffect.physicalDamage *= riposteWeapon.riposte_Attack_01_Modifier;
        damageEffect.holyDamage *= riposteWeapon.riposte_Attack_01_Modifier ;
        damageEffect.fireDamage *= riposteWeapon.riposte_Attack_01_Modifier ;
        damageEffect.magicDamage *= riposteWeapon.riposte_Attack_01_Modifier ;
        damageEffect.lightingDamage *= riposteWeapon.riposte_Attack_01_Modifier ;
        damageEffect.poiseDamage *= riposteWeapon.riposte_Attack_01_Modifier;

        
        
        ProcessRiposte(damagedCharacter, damageEffect);

    }

    private void ProcessRiposte(CharacterManager damagedCharacter, TakeCriticalDamageEffect damageEffect)
    {
        string criticalDamageAnimation = "Riposted_01";

        damagedCharacter.characterEffectsManager.ProcessInstantEffect(damageEffect);
        // damagedCharacter.characterAnimatorManager.PlayTargetActionAnimationInstantly(criticalDamageAnimation, true);
        
        damagedCharacter.isBeingCriticallyDamaged = true;
       
        // MOVE THE ENEMY TO THE PROPER RIPOSTE ANIMATION
        StartCoroutine(damagedCharacter.characterCombatManager.ForceMoveEnemyCharacterToRipostePosition(_playerManager,WorldUtiityManagers.Instance.GetRipostePosition()));
        
        if (damagedCharacter.currentHealth <= 0)
        {
            damagedCharacter.characterAnimatorManager.PlayTargetActionAnimationInstantly("Riposted_01", true); // death
        }
        else
        {
            damagedCharacter.characterAnimatorManager.PlayTargetActionAnimationInstantly("Riposte_01", true); // survives
        }
    }
    
    
}
