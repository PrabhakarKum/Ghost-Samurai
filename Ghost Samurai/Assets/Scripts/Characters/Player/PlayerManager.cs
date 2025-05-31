using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerManager : CharacterManager
{
    [HideInInspector] public PlayerLocomotionManager _playerLocomotionManager;
    [HideInInspector] public PlayerAnimatorManager _playerAnimatorManager;
    [HideInInspector] public PlayerStatManager _playerStatManager;
    [HideInInspector] public PlayerInventoryManager _playerInventoryManager;
    [HideInInspector] public PlayerEquipmentManager _playerEquipmentManager;
    [HideInInspector] public PlayerCombatManager _playerCombatManager;
    [HideInInspector] public SwordComboEffects _swordComboEffects;
    
    
    [Header("Flags")]
    [HideInInspector]public int currentAttackID = 0;


    [Header("Colliders")] 
    [HideInInspector] public MeleeWeaponDamageCollider leftHandDamageCollider;
    [HideInInspector] public MeleeWeaponDamageCollider rightHandDamageCollider;

    [Header("Debug Menu")] 
    [SerializeField] private bool respawnCharacter = false;
    public bool playerLevelUp = false; //debug proposes
    public bool switchRightWeapon = false;
    protected override void Awake()
    {
        base.Awake();
        _playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
        _playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
        _playerStatManager = GetComponent<PlayerStatManager>();
        _playerInventoryManager = GetComponent<PlayerInventoryManager>();
        _playerEquipmentManager = GetComponent<PlayerEquipmentManager>();
        _playerCombatManager = GetComponent<PlayerCombatManager>();
        _swordComboEffects = GetComponent<SwordComboEffects>();
    }

    protected override void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    protected override void Update()
    {
        base.Update();
        //HANDLE ALL OUR MOVEMENT
        //_playerLocomotionManager.HandleAllMovement();
        
        DebugMenu();
    }
    protected override void LateUpdate()
    {
        base.LateUpdate();
        PlayerCamera.instance.HandleCameraActions();
    }
    
    protected override void OnEnable()
    {
        base.OnEnable();
        GameEvents.OnRightHandWeaponChanged += OnCurrentRightHandWeaponIDChange;
        GameEvents.OnLeftHandWeaponChanged += OnCurrentLeftHandWeaponIDChange;
        GameEvents.OnWeaponBeingUsed += OnCurrentWeaponBeingUsedIDChange;
        GameEvents.OnIsBlockingChanged += OnIsBlockingChanged;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        GameEvents.OnRightHandWeaponChanged -= OnCurrentRightHandWeaponIDChange;
        GameEvents.OnLeftHandWeaponChanged -= OnCurrentLeftHandWeaponIDChange;
        GameEvents.OnWeaponBeingUsed -= OnCurrentWeaponBeingUsedIDChange;
        GameEvents.OnIsBlockingChanged -= OnIsBlockingChanged;
    }

    protected override void ReviveCharacter()
    {
        base.ReviveCharacter();
        _currentHealth = maxHealth;
        _currentStamina = maxStamina;
        PlayerUIManager.instance.hudManager.UpdateMaxHealthUI(0,maxHealth);
        //PLAYER REBIRTH EFFECT
        _playerAnimatorManager.PlayTargetActionAnimation("Empty", false);
    }

    public override IEnumerator ProcessDeathEvent(bool manuallySelectDeathAnimation = false)
    {
        PlayerUIManager.instance.popUpManager.SendYouDiedPopup();
        return base.ProcessDeathEvent(manuallySelectDeathAnimation);
    }


    public void OnCurrentRightHandWeaponIDChange(int oldID, int newID)
    {
        WeaponItems newWeapon = Instantiate(WorldItemDatabase.instance.GetWeaponByID(newID));
        _playerInventoryManager.currentRightHandWeapon = newWeapon;
        _playerEquipmentManager.LoadWeaponsOnRightHand();
        
        PlayerUIManager.instance.hudManager.SetRightWeaponQuickSlotIcon(newID);
    }
    
    public void OnCurrentLeftHandWeaponIDChange(int oldID, int newID)
    {
        WeaponItems newWeapon = Instantiate(WorldItemDatabase.instance.GetWeaponByID(newID));
        _playerInventoryManager.currentLeftHandWeapon = newWeapon;
        _playerEquipmentManager.LoadWeaponsOnLeftHand();
        
        PlayerUIManager.instance.hudManager.SetLeftWeaponQuickSlotIcon(newID);
    }
    
    public void OnCurrentWeaponBeingUsedIDChange(int oldID, int newID)
    {
        WeaponItems newWeapon = Instantiate(WorldItemDatabase.instance.GetWeaponByID(newID));
        _playerCombatManager.currentWeaponBeingUsed = newWeapon;
    }

    public void PerformWeaponBasedAction(int actionID, int weaponID)
    {
        WeaponItemAction weaponAction = WorldActionManager.Instance.GetWeaponItemAction(actionID);
        WeaponItems weaponPerformingAction = WorldItemDatabase.instance.GetWeaponByID(weaponID);

        if (weaponAction != null && weaponPerformingAction != null)
        {
            // Call PlayerCombatManager's PerformWeaponBasedAction
            _playerCombatManager.PerformWeaponBasedAction(weaponAction, WorldItemDatabase.instance.GetWeaponByID(weaponID));
        }
    }


    public void OnIsBlockingChanged(bool oldValue, bool newValue)
    { 
        animator.SetBool("isBlocking", isBlocking);

        _playerStatManager.blockingPhysicalAbsorption = _playerCombatManager.currentWeaponBeingUsed.physicalBaseDamageAbsorption;
        _playerStatManager.blockingMagicAbsorption = _playerCombatManager.currentWeaponBeingUsed.magicBaseDamageAbsorption;
        _playerStatManager.blockingFireAbsorption = _playerCombatManager.currentWeaponBeingUsed.fireBaseDamageAbsorption;
        _playerStatManager.blockingHolyAbsorption = _playerCombatManager.currentWeaponBeingUsed.holyBaseDamageAbsorption;
        _playerStatManager.blockingLightningAbsorption = _playerCombatManager.currentWeaponBeingUsed.lightningBaseDamageAbsorption;
        _playerStatManager.blockingStability = _playerCombatManager.currentWeaponBeingUsed.stability;
    }
    
    private void DebugMenu()
    {
        if (respawnCharacter)
        {
            respawnCharacter = false;
            isDead = false;
            ReviveCharacter();
        }
        
        if (playerLevelUp)
        {
            playerLevelUp = false;
            currentVitality += 1;
            currentEndurance += 1;
        }

        if (switchRightWeapon)
        {
            switchRightWeapon = false;
            _playerEquipmentManager.SwitchRightWeapon();
        }
    }
    
    public void RegisterAttack()
    {
        currentAttackID++; // Increment every time a new attack is triggered
    }

    public void UnregisterAttack()
    {
        currentAttackID = -1;
    }
}
