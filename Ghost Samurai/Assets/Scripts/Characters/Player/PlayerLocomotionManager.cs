using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLocomotionManager : CharacterLocomotionManager
{
    private PlayerManager _playerManager;
    
    //values from input manager
    public float verticalMovement { get; private set; }
    public float horizontalMovement { get; private set; }
    public float moveAmount { get; private set; }
    
    private Vector3 moveDirection;
    private Vector3 targetRotationDirection;
    
    [Header("Movement Settings")]
    [SerializeField] float walkingSpeed = 2f;
    [SerializeField] float runningSpeed = 5f;
    [SerializeField] float sprintingSpeed = 10f;
    [SerializeField] float rotationSpeed = 15f;
    [SerializeField] float sprintingStaminaCost = 2f;

    [Header("Dodge Settings")]
    private Vector3 rollDirection;
    [SerializeField] float dodgingStaminaCost = 5f;
    
    [Header("Jump Settings")]
    [SerializeField] float jumpForwardSpeed = 5f;
    [SerializeField] float freeFallSpeed = 2f;
    [SerializeField] float jumpHeight = 2f;
    [SerializeField] float jumpStaminaCost = 15f;
    private Vector3 jumpDirection;
    
    

    protected override void Awake()
    {
        base.Awake();
        _playerManager = GetComponent<PlayerManager>();
    }

    protected override void Update()
    {
        base.Update();
        HandleAllMovement();
        
    }
    public void HandleAllMovement()
    {
        HandleGroundedMovement();
        HandleRotation();
        HandleJumpingMovement();
        HandleFreeFallMovement();
    }
    public void GetHorizontalAndVerticalInputs()
    {
        verticalMovement = PlayerInputManager._instance.verticalInput;
        horizontalMovement = PlayerInputManager._instance.horizontalInput;
        moveAmount = PlayerInputManager._instance.moveAmount;
    }
    private void HandleGroundedMovement()
    {
        if(!_playerManager._playerLocomotionManager.canMove)
            return;
        
        GetHorizontalAndVerticalInputs();
        
        //OUR MOVE DIRECTION IS BASED ON CAMERA PERSPECTIVE &* OUR MOVEMENT INPUTS
        moveDirection = PlayerCamera.instance.transform.forward * verticalMovement;
        moveDirection += PlayerCamera.instance.transform.right * horizontalMovement;
        moveDirection.y = 0;
        


        if (_playerManager.isSprinting)
        {
            _playerManager.characterController.Move(moveDirection * sprintingSpeed * Time.deltaTime);
        }
        else
        {
            if(PlayerInputManager._instance.moveAmount > 0.5f)
            {
                //MOVE AT RUNNING SPEED
                _playerManager.characterController.Move(moveDirection * runningSpeed * Time.deltaTime);
            }

            else if (PlayerInputManager._instance.moveAmount <= 0.5f)
            {
                //Move at walking speed
                _playerManager.characterController.Move(moveDirection * walkingSpeed * Time.deltaTime);
            }
        }
        
    }
    private void HandleJumpingMovement()
    {
        if (_playerManager.isJumping)
        {
            _playerManager.characterController.Move(jumpDirection * jumpForwardSpeed * Time.deltaTime);
        }
    }

    private void HandleFreeFallMovement()
    {
        if (!_playerManager._playerLocomotionManager.isGrounded)
        {
            Vector3 freeFallDirection;
            freeFallDirection = PlayerCamera.instance.transform.forward * PlayerInputManager._instance.verticalInput;
            freeFallDirection += PlayerCamera.instance.transform.right * PlayerInputManager._instance.horizontalInput;
            freeFallDirection.y = 0;
            _playerManager.characterController.Move(freeFallDirection * freeFallSpeed * Time.deltaTime);
        }
    }

    private void HandleRotation()
    {
        if(_playerManager.isDead)
            return;
        
        if(!_playerManager._playerLocomotionManager.canRotate)
            return;
        
        if (_playerManager.isLockedOn)
        {
            if (_playerManager.isSprinting || _playerManager._playerLocomotionManager.isRolling)
            {
                
                Vector3 targetDirection = Vector3.zero;
                targetDirection = PlayerCamera.instance.cameraObject.transform.forward * verticalMovement;
                targetDirection += PlayerCamera.instance.cameraObject.transform.right * horizontalMovement;
                targetDirection.Normalize();
                targetDirection.y = 0;
                
                if(targetDirection == Vector3.zero)
                    targetDirection = transform.forward;
                
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                Quaternion finalRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                transform.rotation = finalRotation;
            }
            else
            {
                if(_playerManager._playerCombatManager.currentTarget == null)
                    return;
                
                Vector3 targetDirection;
                targetDirection = _playerManager._playerCombatManager.currentTarget.transform.position - transform.position;
                targetDirection.y = 0;
                targetDirection.Normalize();
                
                if(targetDirection == Vector3.zero)
                    targetDirection = transform.forward;
                
                
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                Quaternion finalRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                transform.rotation = finalRotation;
            }
        }
        else
        {
            targetRotationDirection = Vector3.zero;
            targetRotationDirection = PlayerCamera.instance.cameraObject.transform.forward * verticalMovement;
            targetRotationDirection += PlayerCamera.instance.cameraObject.transform.right * horizontalMovement;
            targetRotationDirection.Normalize();
            targetRotationDirection.y = 0;

            if(targetRotationDirection == Vector3.zero)
            {
                targetRotationDirection = transform.forward;
            }

            Quaternion newRotation = Quaternion.LookRotation(targetRotationDirection);
            Quaternion targetRotation = Quaternion.Slerp(transform.rotation, newRotation, rotationSpeed * Time.deltaTime);
            transform.rotation = targetRotation;
        }
    }
    public void HandleSprinting()
    {
        if(_playerManager.isPerformingAction)
        {
            _playerManager.isSprinting = false;
        }

        if (_playerManager.currentStamina <= 0)
        {
            _playerManager.isSprinting = false;
            return;
        }

        if (moveAmount > 0.5f)
        {
            _playerManager.isSprinting = true;
        }
        else
        {
            _playerManager.isSprinting = false;
        }

        if (_playerManager.isSprinting)
        {
            _playerManager._playerStatManager.ChangeStamina(-sprintingStaminaCost * Time.deltaTime);
        }
        
    }
    public void AttemptToPerformDodge()
    {
        if(_playerManager.isPerformingAction)
            return;
        
        if( _playerManager.currentStamina <=0)
            return;
        
        if (moveAmount > 0)
        {
            rollDirection = PlayerCamera.instance.transform.forward * verticalMovement;
            rollDirection += PlayerCamera.instance.transform.right * horizontalMovement;
        
            rollDirection.y = 0;
            rollDirection.Normalize();
        
            Quaternion playerRotation = Quaternion.LookRotation(rollDirection);
            _playerManager.transform.rotation = playerRotation;
            
            //perform a roll animation
            _playerManager._playerAnimatorManager.PlayTargetActionAnimation("Roll_Forward_01", true, true);
            _playerManager._playerLocomotionManager.isRolling = true;
        }
        else
        {
            _playerManager._playerAnimatorManager.PlayTargetActionAnimation("Back_Step_01", true, true);
        }
        
        _playerManager._playerStatManager.ChangeStamina(-dodgingStaminaCost);
        
    }

    public void AttemptToPerformJump()
    {
        //IF WE ARE PERFORMING A GENERAL ACTION, WE DO NOT WANT TO ALLOW A JUMP(WILL CHANGE WHEN COMBAT IS ADDED)
        if(_playerManager.isPerformingAction)
            return;
        
        //OUT OF STAMINA, DO NOT ALLOW TO JUMP
        if( _playerManager.currentStamina <=0)
            return;

        //IF WE ARE ALREADY JUMPING NOT ALLOW TO JUMP AGAIN UNTIL JUMP HAS FINSISHED
        if (_playerManager.isJumping)
            return;
        
        //NOT GROUND DO NOT ALLOW TO JUMP
        if(!_playerManager._playerLocomotionManager.isGrounded)
            return;
        
        _playerManager._playerAnimatorManager.PlayTargetActionAnimation("Jump_Start", false);
        _playerManager.isJumping = true;
        
        _playerManager._playerStatManager.ChangeStamina(-jumpStaminaCost);
        
        jumpDirection = PlayerCamera.instance.cameraObject.transform.forward * PlayerInputManager._instance.verticalInput;
        jumpDirection += PlayerCamera.instance.transform.right * PlayerInputManager._instance.horizontalInput;
        jumpDirection.y = 0;
        
        if(jumpDirection != Vector3.zero)
        {
            //IF WE ARE SPRINTING, JUMP DIRECTION IS AT  FULL DISTANCE
            if (_playerManager.isSprinting)
            {
                jumpDirection *= 1;
            }
            //IF WE ARE RUNNING, JUMP DIRECTION IS AT  HALF DISTANCE
            else if (PlayerInputManager._instance.moveAmount > 0.5f)
            {
                jumpDirection *= 0.5f;
            }
            //IF WE ARE WALKING, JUMP DIRECTION IS AT  1/4TH DISTANCE
            else if (PlayerInputManager._instance.moveAmount <= 0.5f)
            {
                jumpDirection *= 0.25f;
            }
        }
    }

    public void ApplyJumpingVelocity()
    {
        yVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravityForce);
    }
    
    protected void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, groundCheckSphereRadius);
    }
    
}
