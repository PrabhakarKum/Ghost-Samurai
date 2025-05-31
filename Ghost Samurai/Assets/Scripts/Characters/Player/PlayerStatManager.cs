using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatManager : CharacterStatManager
{
    private PlayerManager _playerManager;
    protected override void Awake()
    {
        base.Awake();
        _playerManager = GetComponent<PlayerManager>();
    }

    protected override void Start()
    {
        InitializeHealth();
        InitializeStamina();
    }

    protected override void Update()
    {
        base.Update();
        //Regenerate Stamina
        RegenerateStamina();
    }

    void InitializeHealth()
    {
        // Initialising Health based on currentEndurance
        int oldMaxHealth = _playerManager.maxHealth;
        _playerManager.maxHealth = CalculateHealthBasedOnVitalityLevel(_playerManager.currentVitality);
        GameEvents.OnVitalityChanged?.Invoke(oldMaxHealth,_playerManager.maxHealth);
        _playerManager.currentHealth = _playerManager.maxHealth;
    }
    void InitializeStamina()
    {
        // Initialising stamina based on currentEndurance
        int oldMaxStamina = _playerManager.maxStamina;
        _playerManager.maxStamina = CalculateStaminaBasedOnEnduranceLevel(_playerManager.currentEndurance);
        GameEvents.OnEnduranceChanged?.Invoke(oldMaxStamina,_playerManager.maxStamina);
        _playerManager.currentStamina = _playerManager.maxStamina;
    }
    
    public void ChangeStamina(float amount)
    {
        float oldStamina = _playerManager.currentStamina;
        //_playerManager.currentStamina = Mathf.Clamp(_playerManager.currentStamina + amount, 0, _playerManager.maxStamina);
        _playerManager.currentStamina += amount;
        
        // Manually call the stamina regeneration reset function when stamina is consumed
        if (amount < 0) 
        {
            ResetStaminaRegenerateTimer(oldStamina, _playerManager.currentStamina);
        }
    }
    
    public void TakeDamage(int damageAmount)
    {
        // Store old health for UI updates
        float oldHealth = _playerManager.currentHealth;

        // Reduce health but ensure it doesn't go below 0
        _playerManager.currentHealth = Mathf.Clamp(_playerManager.currentHealth - damageAmount, 0, _playerManager.maxHealth);
        
    }
    
}
