using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIEnemyHPBar : UI_StatBar
{
    [SerializeField] AICharacterManager aiCharacterManager;

    public void EnableEnemyHPBar(AICharacterManager aiCharacter)
    {
        aiCharacterManager = aiCharacter;
        SetMaxStat(aiCharacter.maxHealth);
        SetStat(aiCharacter.currentHealth);
        GetComponentInChildren<TextMeshProUGUI>().text = aiCharacterManager.aiCharacterName;
    }

    private void OnEnable()
    {
        GameEvents.OnEnemyHealthChanged += OnEnemyHPChanged;
    }
    
    private void OnDestroy()
    {
        GameEvents.OnEnemyHealthChanged -= OnEnemyHPChanged;
    }

    private void OnEnemyHPChanged(AICharacterManager aiCharacter,int oldValue, int newValue)
    {
        if (aiCharacter == aiCharacterManager)
        {
            SetStat(newValue);
            if (newValue <= 0)
            {
                RemoveHPBar(2.5f);
            }
        }
    }

    private void RemoveHPBar(float time)
    {
        Destroy(gameObject, time);
    }
}
