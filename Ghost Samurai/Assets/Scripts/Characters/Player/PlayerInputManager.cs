using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager _instance;
    private PlayerControl _playerControl;
    public PlayerManager _playerManager;
    
    [Header("CAMERA MOVEMENT INPUT")]
    [SerializeField] private Vector2 cameraInput;
    public float cameraVerticalInput;
    public float cameraHorizontalInput;
    
    [Header("LOCK ON INPUT")]
    [SerializeField] private bool lockOnInput;
    [SerializeField] private bool lockOnLeftInput;
    [SerializeField] private bool lockOnRightInput;
    private Coroutine lockOnCoroutine;
    
    [Header("PLAYER MOVEMENT INPUT")]
    public Vector2 movementInput;
    public float horizontalInput;
    public float verticalInput;
    public float moveAmount;
    
    [Header("PLAYER ACTIONS INPUT")]
    [SerializeField] private bool dodgeInput = false;
    [SerializeField] private bool sprintInput = false;
    [SerializeField] private bool jumpInput = false;
    [SerializeField] private float switchRightWeaponInput;
    
    [Header("TRIGGER INPUT")]
    [SerializeField] private float heavyAttackHoldThreshold = 0.1f;
    private float mouse1HoldTimer = 0f;
    private bool isMouse1Held = false;
    private bool hasTriggeredHeavyAttack = false;
    
    [Header("QUEUED INPUTS")]
    [SerializeField] private bool input_Que_Is_Active = false;
    [SerializeField] private float default_Que_Input_Timer = 0.35f;
    [SerializeField] private float que_Input_Timer = 0f;
    [SerializeField] private bool que_Mouse1_Input = false;
    
    
    [Header("BUMPER INPUT")]
    [SerializeField] private bool mouse1Input = false;
    [SerializeField] private bool mouse2Input = false;
    
    

    
    public void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void OnEnable()
    {
        if (_playerControl == null)
        {
            _playerControl = new PlayerControl();
            _playerControl.PlayerMovement.MovementControls.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
            _playerControl.PlayerCamera.CameraControls.performed += ctx => cameraInput = ctx.ReadValue<Vector2>();
            
            //ACTIONS
            _playerControl.PlayerActions.Dodge.performed += ctx => dodgeInput = true;
            _playerControl.PlayerActions.Jump.performed += ctx => jumpInput = true;
            _playerControl.PlayerActions.SwitchRightWeapon.performed += ctx => switchRightWeaponInput = ctx.ReadValue<float>();
            
            //BUMPERS
            _playerControl.PlayerActions.Mouse1.performed += ctx => mouse1Input = true;
            _playerControl.PlayerActions.Mouse2.performed += ctx => mouse2Input = true;
            _playerControl.PlayerActions.Mouse2.canceled += ctx => _playerManager.isBlocking = false;
            
            
            // TRIGGERS
            _playerControl.PlayerActions.HoldMouse1.performed += ctx=> OnMouse1Down();
            _playerControl.PlayerActions.HoldMouse1.canceled += ctx => OnMouse1Up();
            
            //LOCK ON INPUT
            _playerControl.PlayerActions.LockOn.performed += ctx => lockOnInput = true;
            _playerControl.PlayerActions.SeekRightLockOnTarget.performed += ctx => lockOnRightInput = true;
            _playerControl.PlayerActions.SeekLeftLockOnTarget.performed += ctx => lockOnLeftInput = true;
            
            
            
            //HOLDING THE SHIFT INPUT, SET THE BOOL TO TRUE
            _playerControl.PlayerActions.Sprint.performed += ctx => sprintInput = true;
            //RELEASING THE INPUT, SET THE BOOL TO FALSE
            _playerControl.PlayerActions.Sprint.canceled += ctx => sprintInput = false;
            
            // QUEUED INPUTS
            _playerControl.PlayerActions.QueMouse1.performed += ctx => QueueInput(ref que_Mouse1_Input);
        }
        _playerControl.Enable();
    }
    
    //IF WE MINIMIZE OR LOWER THE WINDOW, STOP ADJUSTING THE INPUTS
    private void OnApplicationFocus(bool focus)
    {
        if(enabled)
        {
            if(focus)
            {
                _playerControl.Enable();
            }
            else
            {
                _playerControl.Disable();
            }
        }

    }

    private void Update()
    {
       HandleAllInput();
    }

    private void HandleAllInput()
    {
        HandleLockOnInput();
        HandleLockOnSwitchTargetInput();
        HandlePlayerMovementInput();
        HandleCameraMovementInput();
        HandleDodgeMovementInput();
        HandleSprintMovementInput();
        HandleJumpMovementInput();
        HandleMouse1Input();
        HandleMouse2Input();
        HandleChargeMouse1Input();
        HandleSwitchRightAndLeftWeaponInput();
        HandleQuedInputs();
        
    }

    //  MOVEMENT
    private void HandlePlayerMovementInput()
    {
        horizontalInput = movementInput.x;
        verticalInput = movementInput.y;
        
        moveAmount = Mathf.Clamp01(Mathf.Abs(verticalInput) + Mathf.Abs(horizontalInput));
        if (moveAmount <= 0.5 && moveAmount > 0)
        {
            moveAmount = 0.5f;
        }
        else if (moveAmount > 0.5 && moveAmount <= 1)
        {
            moveAmount = 1;
        }

        if (_playerManager == null)
            return;
        //why do we pass 0 on the horizontal? because we  only non-strafing movement
        //we use horizontal when we are strafing

        if (moveAmount != 0)
        {
            _playerManager.animator.SetBool("isMoving", true);
            
        }
        else
        {
            _playerManager.animator.SetBool("isMoving", false);
        }
        
        // IF WE ARE NOT LOCKED ON PASS MOVE AMOUNT
        if (!_playerManager.isLockedOn || _playerManager.isSprinting)
        {
            _playerManager._playerAnimatorManager.UpdateAnimatorMovementParameters(0, moveAmount, _playerManager.isSprinting);
        }
        // IF WE ARE LOCKED ON PASS HORIZONTAL AND VERTICAL AMOUNT
        else
        {
            _playerManager._playerAnimatorManager.UpdateAnimatorMovementParameters(horizontalInput, verticalInput, false);
        }
    }
    
    
    private void OnMouse1Down()
    {
        isMouse1Held = true;
        mouse1HoldTimer = 0f;
        hasTriggeredHeavyAttack = false;
    }

    private void OnMouse1Up()
    {
        isMouse1Held = false;
        mouse1HoldTimer = 0f;
        hasTriggeredHeavyAttack = false;
    }
    
    private void HandleCameraMovementInput()
    {
        cameraVerticalInput = cameraInput.y;
        cameraHorizontalInput = cameraInput.x;
    }

    // ACTIONS
    private void HandleDodgeMovementInput()
    {
        if (dodgeInput)
        {
            dodgeInput = false;
            _playerManager._playerLocomotionManager.AttemptToPerformDodge();
        }
    }

    private void HandleSprintMovementInput()
    {
        if (sprintInput)
        {
            _playerManager._playerLocomotionManager.HandleSprinting();
        }
        else
        {
            _playerManager.isSprinting = false;
        }
    }

    private void HandleJumpMovementInput()
    {
        if (jumpInput)
        {
            jumpInput = false;
            _playerManager._playerLocomotionManager.AttemptToPerformJump();
        }
    }

    private void HandleMouse1Input()
    {
        if (mouse1Input)
        {
            mouse1Input = false;
            _playerManager.RegisterAttack();
            _playerManager.SetCharacterActionHand(true);
            _playerManager.PerformWeaponBasedAction(_playerManager._playerInventoryManager.currentRightHandWeapon.mouse_1_Action.actionID, _playerManager._playerInventoryManager.currentRightHandWeapon.itemID);
        }
    }
    
    private void HandleMouse2Input()
    {
        if (mouse2Input)
        {
            mouse2Input = false;
            _playerManager.SetCharacterActionHand(true);
            _playerManager.PerformWeaponBasedAction(_playerManager._playerInventoryManager.currentRightHandWeapon.mouse_2_Action.actionID, _playerManager._playerInventoryManager.currentRightHandWeapon.itemID);
        }
    }

    //LOCK ON
    private void HandleLockOnInput()
    {
        //CHECK FOR DEAD TARGET
        if (_playerManager.isLockedOn)
        {
            if (_playerManager._playerCombatManager.currentTarget == null)
             return;

            if (_playerManager._playerCombatManager.currentTarget.isDead)
            {
                _playerManager.isLockedOn = false;
            }
            
            // ATTEMPT TO FIND NEW TARGET
            
            //THIS ASSURES US THAT THE COROUTINE NEVER RUNS MULTIPLE TIMES OVERLAPPING ITSELF
            if (lockOnCoroutine != null)
                StopCoroutine(lockOnCoroutine);

            lockOnCoroutine = StartCoroutine(PlayerCamera.instance.WaitThenFindNewTarget());
        }
        
        if (lockOnInput && _playerManager.isLockedOn)
        {
            lockOnInput = false;
            PlayerCamera.instance.ClearLockedOnTargets();
            _playerManager._playerCombatManager.currentTarget = null;
            _playerManager.isLockedOn = false;
            //DISABLE LOCK ON
            return;
        }
        
        if (lockOnInput && !_playerManager.isLockedOn)
        {
            lockOnInput = false;
            
            //ENABLE LOCK ON
            PlayerCamera.instance.HandleLocatingLockOnTargets();
            if (PlayerCamera.instance.nearestLockOnTarget != null)
            {
                _playerManager._playerCombatManager.SetTarget(PlayerCamera.instance.nearestLockOnTarget);
                //SET THE TARGET AS OUR CURRENT TARGET
                _playerManager.isLockedOn = true;
            }
        }
    }

    private void HandleLockOnSwitchTargetInput()
    {
        if (lockOnLeftInput)
        {
            lockOnLeftInput = false;
            if (_playerManager.isLockedOn)
            {
                PlayerCamera.instance.HandleLocatingLockOnTargets();
                if (PlayerCamera.instance.leftLockOnTarget != null)
                {
                    _playerManager._playerCombatManager.SetTarget(PlayerCamera.instance.leftLockOnTarget);
                }
            }
        }
        
        if (lockOnRightInput)
        {
            lockOnRightInput = false;
            if (_playerManager.isLockedOn)
            {
                PlayerCamera.instance.HandleLocatingLockOnTargets();
                if (PlayerCamera.instance.rightLockOnTarget != null)
                {
                    _playerManager._playerCombatManager.SetTarget(PlayerCamera.instance.rightLockOnTarget);
                }
            }
        }
    }

    private void HandleChargeMouse1Input()
    {
        if (!isMouse1Held || hasTriggeredHeavyAttack) 
            return;
        
        mouse1HoldTimer += Time.deltaTime;
        if (mouse1HoldTimer >= heavyAttackHoldThreshold)
        {
            hasTriggeredHeavyAttack = true;
            if (_playerManager.isUsingRightHand)
            {
                _playerManager._playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.HeavyAttack01, "Heavy_Attack_01", true);
            }
        }
    }

    private void HandleSwitchRightAndLeftWeaponInput()
    {
        if (switchRightWeaponInput > 0)
        {
            switchRightWeaponInput = 0;
            _playerManager._playerEquipmentManager.SwitchRightWeapon();
        }
        else if (switchRightWeaponInput < 0)
        {
            switchRightWeaponInput = 0; 
            _playerManager._playerEquipmentManager.SwitchLeftWeapon();
        }
    }

    private void QueueInput(ref bool queuedInput) // PASSING A REFERENCE MENS WE PASS A SPECIFIC BOOL, AND NOT THE VALUE OF THAT BOOL(TRUE/ FALSE)
    {
        // RESET ALL QUED INPUT SO ONLY ONE CAN QUE AT A TIME
        que_Mouse1_Input = false;
        
        //que_Mouse2_Input = false;

        if (_playerManager.isPerformingAction || _playerManager.isJumping)
        {
            queuedInput = true;
            que_Input_Timer = default_Que_Input_Timer;
            input_Que_Is_Active = true;
        }
    }

    private void HandleQuedInputs()
    {
        if (input_Que_Is_Active)
        {
            //WHILE THE TIMER IS ABOVE 0, KEEP ATTEMPTING TO PRESS THE INPUT
            if (que_Input_Timer > 0)
            {
                que_Input_Timer -= Time.deltaTime;
                ProcessQuedInputs();
            }
            else
            {
                // RESET ALL QUED INPUTS
                que_Mouse1_Input = false;
                input_Que_Is_Active = false;
                que_Input_Timer = 0;
            }
        }
    }

    private void ProcessQuedInputs()
    {
        if(_playerManager.isDead)
            return;
        
        if(que_Mouse1_Input)
            mouse1Input = true;
        
    }
    
}
