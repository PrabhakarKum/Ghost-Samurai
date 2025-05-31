using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterLocomotionManager : MonoBehaviour
{
    private CharacterManager _characterManager;
    
    [Header("Ground Check & Jumping")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] protected float groundCheckSphereRadius = 0.5f;
    [SerializeField] protected float gravityForce = -5.55f;
    [SerializeField] protected Vector3 yVelocity; // THE FORCE AT WHICH OUR CHARACTER IS PULLED UP OR DOWN(JUMPING OR FALLING)
    [SerializeField] protected float groundedYVelocity = -20f; //THE FORCE AT WHICH OUR CHARACTER IS STICKING TO THE GROUND
    [SerializeField] protected float fallYVelocity = -5f; //THHE FORCE  AT WHICH OUR CHARACTER BEGINS TO FALL WHEN THEY BECOME GROUNDED(RISES AS THEY FALL LONGER)
    protected bool fallingVelocityHasBeenSet = false;
    protected float inAirTime = 0f;
    
    [Header("Flags")]
    public bool isRolling = false;
    public bool canRotate = true;
    public bool canMove = true;
    public bool isGrounded = true;

    
    protected virtual void Awake()
    {
        _characterManager = GetComponent<CharacterManager>();
    }

    protected virtual void Update()
    {
        HandleGroundCheck();
        if (isGrounded)
        {
            inAirTime = 0;
            fallingVelocityHasBeenSet = false;
            
            //IF WE ARE NOT ATTEMPTING TO JUMP OR MOVE FORWARD
            if (yVelocity.y < 0)
            {
                
                yVelocity.y = groundedYVelocity;
            }
        }
        else
        {
            //IF WE ARE NOT JUMPING, AND OUR FALLING VELOCITY HAS NOT BEEN SET
            if (!_characterManager.isJumping && !fallingVelocityHasBeenSet)
            {
                fallingVelocityHasBeenSet = true;
                yVelocity.y = fallYVelocity;
                
            }
            inAirTime += Time.deltaTime;
            _characterManager.animator.SetFloat("inAirTimer", inAirTime);
            yVelocity.y += gravityForce * Time.deltaTime;
        }
        
        //THERE SHOULD ALWAYS BE SOME FORCE APPLIED TO THE Y VELOCITY
        _characterManager.characterController.Move(yVelocity * Time.deltaTime);
    }

    protected void HandleGroundCheck()
    {
        isGrounded = Physics.CheckSphere(_characterManager.transform.position, groundCheckSphereRadius,groundLayer);
    }

    public void EnableCanRotate()
    {
        canRotate = true;
    }
    
    public void DisableCanRotate()
    {
        canRotate = false;
    }
}
