using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

public class CharacterManager : MonoBehaviour
{
   [HideInInspector] public CharacterController characterController;
   [HideInInspector] public CharacterEffectsManager characterEffectsManager;
   [HideInInspector] public CharacterStatManager characterStatManager;
   [HideInInspector] public CharacterAnimatorManager characterAnimatorManager;
   [HideInInspector] public CharacterCombatManager characterCombatManager;
   [HideInInspector] public CharacterSoundFXManager characterSoundFXManager;
   [HideInInspector] public CharacterLocomotionManager characterLocomotionManager;
   [HideInInspector] public Animator animator;

   [Header("Status")]
   public bool _isDead = false;
   public bool isDead
   {
      get => _isDead;
      set
      {
         if (_isDead != value)
         {
            bool oldValue = _isDead;
            _isDead = value;
            
            GameEvents.OnIsDeadChanged?.Invoke(oldValue, _isDead);
         }
      }
   }
   
   [Header("Character Groups" )]
   public CharacterGroup characterGroup;
   
   [Header("Flags")]
   public bool _isBlocking = true;
   public bool isPerformingAction = false;
   public bool isSprinting = false;
   public bool isJumping = false;
   public bool isLockedOn = false;
   public bool isChargingAttack = false;
   public bool isVulnerable = false;
   public bool isAttacking = false;
   public bool isRipostable = false;
   public bool isBeingCriticallyDamaged = false;
   
   [Header("Resources")]
   public int _currentHealth;
   public float _currentStamina;
   public int maxHealth = 0;
   public int maxStamina = 0;
   
   [Header("Actions")]
   public bool isUsingLeftHand = false;
   public bool isUsingRightHand = false;
   
   
   public float currentStamina
   {
      get => _currentStamina;
      set
      {
         float oldStamina = _currentStamina;
         _currentStamina = Mathf.Clamp(value, 0, maxStamina);

         // Fire event when stamina changes
         GameEvents.OnStaminaChanged?.Invoke(oldStamina, _currentStamina);
      }
   }
   public int currentHealth
   {
      get => _currentHealth;
      set
      {
         if (isDead)
            return;
         
         int oldHealth = _currentHealth;
         _currentHealth = Mathf.Clamp(value, 0, maxHealth);

         // Fire event when health changes
         if (this is PlayerManager playerManager)
         {
            GameEvents.OnHealthChanged?.Invoke(playerManager, oldHealth, _currentHealth);
            CheckHP(playerManager,_currentHealth);
         }
         
         if (this is AICharacterManager aiCharacter)
         {
            GameEvents.OnEnemyHealthChanged?.Invoke(aiCharacter, oldHealth, _currentHealth);
            CheckHP(aiCharacter,_currentHealth);
         }
      }
   }

   public bool isBlocking
   {
      get => _isBlocking;
      set
      {
         if (_isBlocking != value)
         {
            bool oldValue = _isBlocking;
            _isBlocking = value;
            
            if (this is PlayerManager)
            {
                GameEvents.OnIsBlockingChanged?.Invoke(oldValue,_isBlocking);
            }
            else if (this is AICharacterManager aiCharacter)
            {
                GameEvents.OnAIBlockingChanged?.Invoke(aiCharacter ,oldValue, _isBlocking);
            }
         }
      }
   }
   
   [Header(("Stats"))]
   public int _currentVitality;
   public int _currentEndurance;
   public int currentEndurance
   {
      get => _currentEndurance;
      set
      {
         int oldEndurance = _currentEndurance;
         _currentEndurance = Mathf.Max(1, value); // Ensures at least 1

         // Recalculate max stamina based on new endurance
         maxStamina = characterStatManager.CalculateStaminaBasedOnEnduranceLevel(_currentEndurance);
         currentStamina = maxStamina; // Fully restore stamina when max increases

         // Fire event to update UI
         GameEvents.OnEnduranceChanged?.Invoke(oldEndurance, maxStamina);
      }
   }
   public int currentVitality
   {
      get => _currentVitality;
      set
      {
         int oldVitality = _currentVitality;
         _currentVitality = Mathf.Max(1, value); // Ensures at least 1

         // Recalculate max health based on new vitality
         maxHealth = characterStatManager.CalculateHealthBasedOnVitalityLevel(_currentVitality);
         currentHealth = maxHealth; // Fully heal when max increases

         Debug.Log($"Vitality Changed: {oldVitality} → {_currentVitality}, New Max Health: {maxHealth}");

         // Fire event to update UI
         GameEvents.OnVitalityChanged?.Invoke(oldVitality, maxHealth);
      }
   }

   protected virtual void  OnEnable()
   {
      GameEvents.OnIsDeadChanged += OnIsDeadChanged;
   }

   protected virtual void OnDisable()
   {
      GameEvents.OnIsDeadChanged -= OnIsDeadChanged;
   }

   protected virtual void Awake()
   {
      DontDestroyOnLoad(this);
      characterController = GetComponent<CharacterController>(); 
      characterEffectsManager = GetComponent<CharacterEffectsManager>();
      characterStatManager = GetComponent<CharacterStatManager>();
      characterAnimatorManager = GetComponent<CharacterAnimatorManager>();
      characterCombatManager = GetComponent<CharacterCombatManager>();
      characterSoundFXManager = GetComponent<CharacterSoundFXManager>();
      characterLocomotionManager = GetComponent<CharacterLocomotionManager>();
      animator = GetComponent<Animator>();
   }

   protected virtual void Start()
   {
      IgnoreMyOwnColliders();
   }

   protected virtual void Update()
   {
      animator.SetBool("isGrounded", characterLocomotionManager.isGrounded);
   }
   
   protected virtual void LateUpdate()
   {
         
   }

   protected virtual void FixedUpdate()
   {
      
   }

   public void SetCharacterActionHand(bool rightHandedAction)
   {
      if (rightHandedAction)
      {
         isUsingLeftHand = false;
         isUsingRightHand = true;
      }
      else
      {
         isUsingLeftHand = true;
         isUsingRightHand = false;
      }
   }

   public virtual IEnumerator ProcessDeathEvent(bool manuallySelectDeathAnimation = false)
   {
      if (isDead) yield break;
      isDead = true;
      
      //Reset any flag here that need to be reset
      
      //if we are not grounded, play an aerial death animation
      
      if (!manuallySelectDeathAnimation)
      {
         Debug.Log("Playing Death Animation");
         characterAnimatorManager.PlayTargetActionAnimation("Die_01", true, false);
      }
      
      //Play Some Death SFX
      yield return new WaitForSeconds(5);
      
      // Disable characters
   }

   //Prevent us from over healing after the current health = max health; we have to manually call this where we take the damage
   protected virtual void CheckHP(CharacterManager characterManager, int currentHealth)
   {
      if (isDead) 
         return;
      
      if (currentHealth <= 0)
      {
         StartCoroutine(ProcessDeathEvent());
      }

      if (currentHealth > maxHealth)
      {
         currentHealth = maxHealth;
         this.currentHealth = currentHealth;
      }
   }

   protected virtual void ReviveCharacter()
   {
      
   }

   protected virtual void IgnoreMyOwnColliders()
   {
      Collider characterControllerCollider = GetComponent<Collider>();
      Collider[] damageableCharacterColliders = GetComponentsInChildren<Collider>();
      
      List<Collider> ignoreColliders = new List<Collider>();

      foreach (var collider in damageableCharacterColliders)
      {
         ignoreColliders.Add(collider);
      }
      ignoreColliders.Add(characterControllerCollider);

      foreach (var collider in ignoreColliders)
      {
         foreach (var otherCollider in ignoreColliders)
         {
            Physics.IgnoreCollision(collider, otherCollider, true);
         }
      }
   }

   private void OnIsDeadChanged(bool oldValue, bool newValue)
   {
      animator.SetBool("isDead", isDead);
   }
   
}
