using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class AICharacterManager : CharacterManager
{
    [HideInInspector] public AICharacterCombatManager aiCharacterCombatManager;
    [HideInInspector] public AICharacterLocomotionManager aiCharacterLocomotionManager;
    [HideInInspector] public AICharacterAnimatorManager aiCharacterAnimatorManager;
    [HideInInspector] public AICharacterStatManager aiCharacterStatManager;
    
    [Header("Character Name")]
    public string aiCharacterName = "";
    
    [Header("Resources")]

    [Header("Flags")] 
    public bool _isAIMoving = false;
    public bool isAIMoving
    {
        get => _isAIMoving;
        set
        {
            if (_isAIMoving != value)
            {
                bool oldIsMoving = _isAIMoving;
                _isAIMoving = value;
                GameEvents.OnIsMovingChanged?.Invoke(oldIsMoving, _isAIMoving);
            }
        }
    }
    
    [Header("Status")] 
    public bool _enemyFightIsActive;
    public bool enemyFightIsActive
    {
        get => _enemyFightIsActive;
        set
        {
            if (_enemyFightIsActive != value)
            {
                bool oldIsActive = _enemyFightIsActive;
                _enemyFightIsActive = value;
                GameEvents.EnemyFightIsActive?.Invoke(oldIsActive, _enemyFightIsActive);
            }
        }
    }
    
    
    [Header("Navmesh Agent")]
    public NavMeshAgent navMeshAgent;
    
    
    [Header("Current State")]
    [SerializeField] protected AIState currentState;
    
    
    [Header("AI State Templates")]
    public AI_IdleState idleStateTemplate;
    public AI_PursueTargetState pursueTargetStateTemplate;
    public AI_CombatStanceState combatStanceStateTemplate;
    public AI_AttackState attackStateTemplate;
    public AI_RetreatState retreatStateTemplate;
    //public AI_BlockState blockStateTemplate;
    
    # region AI-States
    public AI_IdleState idleState { get; private set; }
    public AI_PursueTargetState pursueTargetState { get; private set; }
    public AI_CombatStanceState combatStance { get; protected set; }
    public AI_AttackState attackState { get; private set; }
    public AI_RetreatState retreatState { get; private set; }
    public AI_BlockState blockState { get; private set; }
   
    
    # endregion

    protected override void Awake()
    {
        base.Awake();
        
        GameEvents.OnIsMovingChanged += OnAIMovingChanged;
        
        aiCharacterCombatManager = GetComponent<AICharacterCombatManager>();
        aiCharacterLocomotionManager = GetComponent<AICharacterLocomotionManager>();
        aiCharacterAnimatorManager = GetComponent<AICharacterAnimatorManager>();
        aiCharacterStatManager = GetComponent<AICharacterStatManager>();
        
        navMeshAgent = GetComponentInChildren<NavMeshAgent>();
        
        // USE A COPY OF THE SCRIPTABLE OBJECT, SOO THE ORIGINALS ARE NOT MODIFIED
        idleState = Instantiate(idleStateTemplate);
        pursueTargetState = Instantiate(pursueTargetStateTemplate);
        combatStance = Instantiate(combatStanceStateTemplate);
        attackState = Instantiate(attackStateTemplate);
        retreatState = Instantiate(retreatStateTemplate);
        //blockState = Instantiate(blockStateTemplate);
        
        
        currentState = idleState;
        
    }

    protected override void OnDisable()
    {
        //enemyFightIsActive = false;
        GameEvents.OnIsMovingChanged -= OnAIMovingChanged;
    }

    protected override void Update()
    {
        base.Update();
        aiCharacterCombatManager.HandleActionRecovery(this);
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        
        ProcessStateMachine();
    }
    
    

    private void ProcessStateMachine()
    {
        AIState nextState = null;

        if (currentState != null)
        {
            nextState = currentState.Tick(this);
        }

        if (nextState != null)
        {
            currentState = nextState;
        }
        
        // THE POSITION/ROTATION SHOULD BE RESET ONLY AFTER THE STATE MACHINE HAS PROCESSED ITS TICK
        navMeshAgent.transform.localPosition = Vector3.zero;
        navMeshAgent.transform.localRotation = Quaternion.identity;

        if (aiCharacterCombatManager.currentTarget != null)
        {
            aiCharacterCombatManager.targetDirection = aiCharacterCombatManager.currentTarget.transform.position - transform.position;
            aiCharacterCombatManager.viewableAngle = WorldUtiityManagers.Instance.GetAngleOfTarget(transform, aiCharacterCombatManager.targetDirection);
            aiCharacterCombatManager.distanceFromTarget = Vector3.Distance(transform.position, aiCharacterCombatManager.currentTarget.transform.position);
        }

        if (navMeshAgent.enabled)
        {
            Vector3 agentDestination = navMeshAgent.destination;
            float remainingDestination = Vector3.Distance(agentDestination, transform.position);
            
            if (remainingDestination > navMeshAgent.stoppingDistance)
            {
                isAIMoving = true;
            }
            else
            {
                isAIMoving = false;
            }
        }
        else
        {
            isAIMoving = false;
        }
    }

    private void OnAIMovingChanged(bool oldStatus, bool newStatus)
    {
        animator.SetBool("isMoving", newStatus); 
    }

    

}
