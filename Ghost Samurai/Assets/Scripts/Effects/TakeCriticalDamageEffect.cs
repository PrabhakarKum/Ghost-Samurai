using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character Effects/Instant Effects/Critical Damage Effect")]
public class TakeCriticalDamageEffect : TakeDamageEffect
{
    public override void ProcessEffect(CharacterManager characterManager)
    {
        if(characterManager.isVulnerable)
            return;
        //IF THE CHARACTER IS DEAD, NO ADDITIONAL DAMAGE EFFECTS SHOULD BE PROCESSED
        
        if (characterManager.isDead)
            return;
        
        CalculateDamage(characterManager);

        characterManager.characterCombatManager.pendingCriticalDamage = finalDamageDealt;

    }
    
    protected override void CalculateDamage(CharacterManager characterManager)
    {
        if (_characterCausingDamage != null)
        {
            
        }
        //check character for flat defences and subtract them from the damage
        
        //check character for armour absorptions, and subtract the percentage from the damage
        
        //add all the damage types together, and apply final damage
        
        
        finalDamageDealt = Mathf.RoundToInt(physicalDamage + magicDamage + fireDamage + lightingDamage + holyDamage);
        if (finalDamageDealt <= 0)
        {
            finalDamageDealt = 1;
        }
        
        
        
        //  CALCULATE POISE DAMAGE TO DETERMINE IF THE CHARACTER WILL BE STUNNED
        
        // WE SUBTRACT POISE DAMAGE FROM THE CHARACTER TOTAL 
        characterManager.characterStatManager.totalPoiseDamage -= poiseDamage;
        
        // WE STORE POISE DAMAGE TAKEN TO FOR OTHER DAMAGE INTERACTION
        characterManager.characterCombatManager.previousPoiseDamageTaken = poiseDamage;

        float remainingPoise = characterManager.characterStatManager.basePoiseDefence + characterManager.characterStatManager.offensivePoiseBonus + characterManager.characterStatManager.totalPoiseDamage;
        
        if(remainingPoise <= 0)
            poiseIsBroken = true;
        
        //Debug.Log("Remaining poise: "+ remainingPoise);
        
        // SINCE THE CHARACTER HAS BEEN HIT, WE RESET THE POISE TIMER 
        characterManager.characterStatManager.poiseResetTimer = characterManager.characterStatManager.defaultPoiseResetTime;
    }
}
