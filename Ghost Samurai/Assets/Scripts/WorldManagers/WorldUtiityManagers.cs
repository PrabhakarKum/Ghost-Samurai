using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class WorldUtiityManagers : MonoBehaviour
{
    public static WorldUtiityManagers Instance;
    [SerializeField] private LayerMask characterLayers;
    [SerializeField] private LayerMask environmentLayers;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public LayerMask GetCharacterLayers()
    {
        return characterLayers;
    }

    public LayerMask GetEnvironmentLayers()
    {
        return environmentLayers;
    }

    public bool CanIDamageThisTarget(CharacterGroup attackingCharacter, CharacterGroup targetCharacter)
    {
        if (attackingCharacter == CharacterGroup.Team1)
        {
            switch(targetCharacter)
            {
                case CharacterGroup.Team1: return false;
                case CharacterGroup.Team2: return true;
                default:
                    break;
            }
        }
        else if(attackingCharacter == CharacterGroup.Team2)
        {
            switch(targetCharacter)
            {
                case CharacterGroup.Team1: return true;
                case CharacterGroup.Team2: return false;
                default:
                    break;
            }
        }
        return false;
    }

    public float GetAngleOfTarget(Transform characterTransform, Vector3 targetDirection)
    {
        targetDirection.y = 0;
        float viewableAngle = Vector3.Angle(characterTransform.forward, targetDirection);
        Vector3 cross = Vector3.Cross(characterTransform.forward, targetDirection);

        if (cross.y < 0)
        {
            viewableAngle = -viewableAngle;
        }
        return viewableAngle;
    }

    public DamageIntensity GetDamageIntensityBasedOnPoiseDamage(float poiseDamage)
    {
        //throwing daggers, small items
        DamageIntensity damageIntensity = DamageIntensity.Ping;
        
        // Ultra Weapons / Colossal Attacks
        if (poiseDamage >= 120f)
            damageIntensity = DamageIntensity.Colossal;
            // Great Weapons / heavy Attack
        else if (poiseDamage >= 70f)
            damageIntensity = DamageIntensity.Heavy;
            // Standard Weapons / Medium Attack
        else if (poiseDamage >= 30f) 
            damageIntensity = DamageIntensity.Medium;
            // Daggers
        else if (poiseDamage >= 10f)
            damageIntensity = DamageIntensity.Light;
        
        return damageIntensity;
    }

    public Vector3 GetRipostePosition()
    {
        Vector3 position = new Vector3(0f, 0f, 3.2f);
        return position;
    }
}
