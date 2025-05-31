using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CharacterStatManager : MonoBehaviour
{
    private CharacterManager _characterManager;
    
    [Header("Stamina Regeneration")]
    private float _staminaRegenTimer = 0f;
    private float _staminaTickTimer = 0f;
    [SerializeField] private float staminaRegenerationAmount = 2f;
    [SerializeField] private float staminaRegenerationDelay = 2f;

    [Header("Blocking Absorption")] 
    public float blockingPhysicalAbsorption;
    public float blockingFireAbsorption;
    public float blockingHolyAbsorption;
    public float blockingMagicAbsorption;
    public float blockingLightningAbsorption;
    public float blockingStability;

    [Header("Poise")] 
    [SerializeField] public float totalPoiseDamage; // how much poise damage we have taken
    public float basePoiseDefence; // The poise bonus gained from using weapons (heavy weapons have a much larger bonus)
    public float offensivePoiseBonus; // The poise bonus gained from armour/ talisman etc
    public float defaultPoiseResetTime = 8; // The time  it takes for poise damage to reset (must not be hit in the time, or it will reset )
    [SerializeField] public float poiseResetTimer = 0;

    protected virtual void Awake()
    {
        _characterManager = GetComponent<CharacterManager>();
    }

    protected virtual void Start()
    {
       
    }

    protected virtual void Update()
    {
        HandlePoiseResetTimer();
    }
    
    
    public int CalculateHealthBasedOnVitalityLevel(int vitality)
    {
        float health = 0f;
        //CREATING AN EQUATION FOR HOW YOU WANT YOUR health TO BE CALCULATED
        health = vitality * 15f;
        
        return Mathf.RoundToInt(health);
    }
    
    public int CalculateStaminaBasedOnEnduranceLevel(int endurance)
    {
        float stamina = 0f;
        //CREATING AN EQUATION FOR HOW YOU WANT YOUR STAMINA TO BE CALCULATED
        stamina = endurance * 10f;
        
        return Mathf.RoundToInt(stamina);
    }
    
    protected virtual void RegenerateStamina()
    {
        if (_characterManager.isSprinting)
            return;
      
        if(_characterManager.isPerformingAction)
            return;
      
        _staminaRegenTimer += Time.deltaTime;
        if (_staminaRegenTimer >= staminaRegenerationDelay)
        {
            if (_characterManager.currentStamina < _characterManager.maxStamina)
            {
                _staminaTickTimer += Time.deltaTime;
                if (_staminaTickTimer >= 0.1)
                {
                    _staminaTickTimer = 0f;
                    _characterManager.currentStamina += staminaRegenerationAmount;
                }
            }
        }
      
    }

    public virtual void ResetStaminaRegenerateTimer(float previousStaminaValue, float newStaminaValue)
    {
        //WE ONLY WANT TO RESET THE REGENERATION IF THE ACTION USED STAMINA
        // WE DON'T WANT TO RESET THE REGENERATION IF WE ARE ALREADY REGENERATING THE STAMINA
        if (newStaminaValue < previousStaminaValue)
        {
            _staminaRegenTimer = 0;
        }
    }

    public void HandlePoiseResetTimer()
    {
        if (poiseResetTimer > 0)
        {
            poiseResetTimer -= Time.deltaTime;
            //Debug.Log("Poise Timer: " + poiseResetTimer);
        }
        else
        {
            totalPoiseDamage = 0;
            //Debug.Log("Poise reset! Total Poise Damage is now: " + totalPoiseDamage);
        }
    }
    
}
