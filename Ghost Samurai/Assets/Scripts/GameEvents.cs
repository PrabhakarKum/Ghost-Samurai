using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameEvents 
{
    public static Action<int, int> OnVitalityChanged; // (oldVitality, newVitality)
    public static Action<int, int> OnEnduranceChanged; // (oldEndurance, newEndurance)
    public static Action<CharacterManager,float, float> OnHealthChanged; // (oldHealth, newHealth)
    public static Action<float, float> OnStaminaChanged; // (oldStamina, newStamina)
    public static Action<int, int> OnRightHandWeaponChanged; //(oldID, NewID)
    public static Action<int, int> OnLeftHandWeaponChanged; //(oldID, NewID)
    public static Action<int, int> OnWeaponBeingUsed; //(oldID, NewID)
    public static Action<bool, bool> OnHeavyAttackChanged;
    public static Action<bool, bool> OnIsMovingChanged;
    public static Action<AICharacterManager,int, int> OnEnemyHealthChanged;
    public static Action<bool, bool> EnemyFightIsActive;
    public static Action<bool,bool> OnIsBlockingChanged;
    public static Action<bool, bool> OnIsDeadChanged;
    
    
    // Enemy Events
    public static Action<AICharacterManager, bool, bool> OnAIBlockingChanged;
}
