using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class AICharacterCombatManager : CharacterCombatManager
{
    protected AICharacterManager aiCharacter;
    
    [Header("Target Information")] 
    public float distanceFromTarget;
    public float viewableAngle;
    public Vector3 targetDirection;
    
    [Header("Detection")]
    [SerializeField] private float detectionRadius = 15f;
    public float minimumFOV = -35f;
    public float maximumFOV = 35f;
    public float aiRotateSpeed = 10f;
    
    [Header("Recovery Timer")]
    public float actionRecoveryTimer = 5f;
    
    [Header("Pivot")]
    public bool enablePivot = true;

    [Header("Attack Rotation Speed")]
    public float attackRotationSpeed = 0;

    [Header("Stance/Posture Setting")] 
    public float maximumStance = 150;
    public float currentStance;
    [SerializeField] private float stanceRegeneratedPerSecond = 15f;
    [SerializeField] private bool ignoreStanceBreak = false;

    [Header("Stance Timer")] 
    [SerializeField] private float defaultTimeUntilStanceRegenerationBegins = 15f;
    [SerializeField] private float stanceTickTimer = 0;
    [SerializeField] private float stanceRegenerationTimer = 5f;
    

    protected override void Awake()
    {
        base.Awake();
        aiCharacter = GetComponent<AICharacterManager>();
        lockOnTransform = GetComponentInChildren<LockOnTransform>().transform;
    }

    private void FixedUpdate()
    {
        HandleStanceBreak();
    }

    private void HandleStanceBreak()
    {
        if(aiCharacter.isDead)
            return;

        if (stanceRegenerationTimer > 0)
        {
            stanceRegenerationTimer -= Time.deltaTime;
        }
        else
        {
            stanceRegenerationTimer = 0;
            if (currentStance < maximumStance)
            {
                // begin adding stance each tick
                stanceTickTimer += Time.deltaTime;
                if (stanceTickTimer >= 1)
                {
                    stanceTickTimer = 0;
                    currentStance += stanceRegeneratedPerSecond;
                }
            }
            else
            {
                currentStance = maximumStance;
            }
        }

        if (currentStance <= 0)
        {
            DamageIntensity previousDamageIntensity = WorldUtiityManagers.Instance.GetDamageIntensityBasedOnPoiseDamage(previousPoiseDamageTaken);

            if (previousDamageIntensity == DamageIntensity.Colossal)
            {
                currentStance = 1;
                return;
            }
            
            // TO DO: IF WE ARE BEING BACKSTABBED/ RIPOSTED (CRITICALLY DAMAGED) DO NOT  PLAY THE STANCE BREAK ANIMATION, AS THIS WOULD BREAK THE STATE
            
            currentStance = maximumStance;
            
            if(ignoreStanceBreak)
                return;
            
            aiCharacter.characterAnimatorManager.PlayTargetActionAnimationInstantly("Stance_Break_01", true);
        }
    }

    public void DamageStance(int stanceDamage)
    {
        // WHEN STANCE IS DAMAGED, TIMER IS RESET, MEANING CONSTANT ATTACKS GIVE NO CHANCE TO RECOVERING STANCE THT IS LOST
        stanceRegenerationTimer = defaultTimeUntilStanceRegenerationBegins;
        currentStance -= stanceDamage;
    }

    public void FindATargetViaLineOfSight(AICharacterManager aiCharacter)
    {
        if(currentTarget != null)
        {
            // Check if the current target is dead or out of range, if so, deactivate combat
            if(currentTarget.isDead || !IsTargetInSight(aiCharacter, currentTarget))
            {
                aiCharacter.enemyFightIsActive = false; // Deactivate combat
                currentTarget = null; // Clear the target
            }
            return;
        }
        
        Collider[] colliders = Physics.OverlapSphere(aiCharacter.transform.position, detectionRadius, WorldUtiityManagers.Instance.GetCharacterLayers());

        for (int i = 0; i < colliders.Length; i++)
        {
            CharacterManager targetCharacter = colliders[i].GetComponent<CharacterManager>();

            if (targetCharacter == null)
                continue;
            
            if(targetCharacter == aiCharacter)
                continue;

            if (targetCharacter.isDead)
                continue;

            if (WorldUtiityManagers.Instance.CanIDamageThisTarget(aiCharacter.characterGroup, targetCharacter.characterGroup))
            {
                // IF A POTENTIAL TARGET IS FOUND, IT HAS TO BE IN FRONT OF US
                
                Vector3 targetsDirection = targetCharacter.transform.position - aiCharacter.transform.position;
                float angleOfPotentialTarget = Vector3.Angle(targetsDirection, aiCharacter.transform.forward);

                if (angleOfPotentialTarget > minimumFOV && angleOfPotentialTarget < maximumFOV)
                {
                    // LASTLY, WE CHECK FOR ENVIRONMENT BLOCKS
                    if (Physics.Linecast(aiCharacter.characterCombatManager.lockOnTransform.position, targetCharacter.characterCombatManager.lockOnTransform.position, WorldUtiityManagers.Instance.GetEnvironmentLayers()))
                    {
                        Debug.DrawLine(aiCharacter.characterCombatManager.lockOnTransform.position, targetCharacter.characterCombatManager.lockOnTransform.position, Color.red);
                    }
                    else
                    {
                        targetDirection = targetCharacter.transform.position - transform.position;
                        viewableAngle = WorldUtiityManagers.Instance.GetAngleOfTarget(transform, targetDirection);
                        aiCharacter.characterCombatManager.SetTarget(targetCharacter);
                        if(enablePivot)
                            PivotTowardsTarget(aiCharacter);
                        
                        aiCharacter.enemyFightIsActive = true;
                    }
                }
            }
        }
    }

    public void PivotTowardsTarget(AICharacterManager aiCharacter)
    {
        if(aiCharacter.isPerformingAction)
            return;

        // I DON'T HAVE ANIMATION FOR THIS RIGHT NOW MAYBE IN FUTURE IF I CAN CHANGE THAT TO SPECIFIC ANIMATION 
        // if (viewableAngle >= 20 && viewableAngle <= 60)
        // {
        //     aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Right_45", true);
        // }
        // else if (viewableAngle <= -20 && viewableAngle >= -60)
        // {
        //     aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Left_45", true);
        // }
        // else if (viewableAngle >= 61 && viewableAngle <= 110)
        // {
        //     aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Right_90", true);
        // }
        // else if (viewableAngle <= -61 && viewableAngle >= -110)
        // {
        //     aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Left_90", true);
        // }
        // if (viewableAngle >= 110 && viewableAngle <= 145)
        // {
        //     aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Right_135", true);
        // }
        // else if (viewableAngle <= -110 && viewableAngle >= -145)
        // {
        //     aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Left_135", true);
        // }
        // if (viewableAngle >= 146 && viewableAngle <= 180)
        // {
        //     aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Right_180", true);
        // }
        // else if (viewableAngle <= -146 && viewableAngle >= -180)
        // {
        //     aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Left_180", true);
        // }
        
        //aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Idle", true);

        // Rotate the character towards the target based on the viewable angle
        Quaternion targetRotation = Quaternion.Euler(0, aiCharacter.transform.eulerAngles.y + viewableAngle, 0);
    
        // Smoothly rotate the character
        aiCharacter.transform.rotation = Quaternion.Slerp(aiCharacter.transform.rotation, targetRotation,
            Time.deltaTime * aiRotateSpeed); // Adjust aiRotateSpeed as needed

    }

    public void HandleActionRecovery(AICharacterManager aiCharacter)
    {
        if (actionRecoveryTimer > 0)
        {
            if (!aiCharacter.isPerformingAction)
            {
                actionRecoveryTimer -= Time.deltaTime;
            }
                
        }
    }

    public void RotateTowardsAgent(AICharacterManager aiCharacter)
    {
        if (aiCharacter.isAIMoving)
        {
            aiCharacter.transform.rotation = aiCharacter.navMeshAgent.transform.rotation;
        }
    }
    
    public void RotateTowardsTargetWhileAttacking(AICharacterManager aiCharacter)
    {
        if(currentTarget == null)
            return;
        
        if(!aiCharacter.aiCharacterLocomotionManager.canRotate)
            return;
        
        if(!aiCharacter.isPerformingAction)
            return;
        
        Vector3 targetDirection = currentTarget.transform.position - aiCharacter.transform.position;
        targetDirection.y = 0;
        targetDirection.Normalize();

        if (targetDirection == Vector3.zero)
            targetDirection = aiCharacter.transform.forward;
        
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        
        aiCharacter.transform.rotation = Quaternion.Slerp(aiCharacter.transform.rotation, targetRotation, attackRotationSpeed * Time.deltaTime);
        
    }
    
    private bool IsTargetInSight(AICharacterManager aiCharacter, CharacterManager targetCharacter)
    {
        Vector3 targetDirection = targetCharacter.transform.position - aiCharacter.transform.position;
        float angleOfPotentialTarget = Vector3.Angle(targetDirection, aiCharacter.transform.forward);
        return angleOfPotentialTarget > minimumFOV && angleOfPotentialTarget < maximumFOV &&
               !Physics.Linecast(aiCharacter.characterCombatManager.lockOnTransform.position, targetCharacter.characterCombatManager.lockOnTransform.position, WorldUtiityManagers.Instance.GetEnvironmentLayers());
    }
    
    private void OnDrawGizmosSelected()
    {
        if (Application.isPlaying == false) return;
    
        AICharacterManager aiCharacter = GetComponent<AICharacterManager>();

        if (aiCharacter == null) return;

        Vector3 forward = - aiCharacter.characterCombatManager.lockOnTransform.up;
        Vector3 position = aiCharacter.characterCombatManager.lockOnTransform.position;

        // Set the radius color
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(position, detectionRadius);

        // Set angle visualization color
        Gizmos.color = Color.green;

        // Calculate min & max detection direction
        Quaternion leftRayRotation = Quaternion.Euler(0, minimumFOV, 0);
        Quaternion rightRayRotation = Quaternion.Euler(0, maximumFOV, 0);

        Vector3 leftDirection = leftRayRotation * forward;
        Vector3 rightDirection = rightRayRotation * forward;

        // Draw the lines for the field of view
        Gizmos.DrawLine(position, position + leftDirection * detectionRadius);
        Gizmos.DrawLine(position, position + rightDirection * detectionRadius);
        
        //Debug.DrawLine(aiCharacter.characterCombatManager.lockOnTransform.position, targetCharacter.characterCombatManager.lockOnTransform.position, Color.red);
    }
}
