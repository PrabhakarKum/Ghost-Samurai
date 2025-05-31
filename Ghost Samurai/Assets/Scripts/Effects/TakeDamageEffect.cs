using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.TextCore.Text;

[CreateAssetMenu(menuName = "Character Effects/Instant Effects/Taking Damage")]
public class TakeDamageEffect : InstantCharacterEffect
{
    [Header("Character Causing Damage")]
    public CharacterManager _characterCausingDamage; //If the damage is caused by another character attack it will be stored here
    
    [Header("Damage")]
    public float physicalDamage = 0;
    public float magicDamage = 0;
    public float fireDamage = 0;
    public float lightingDamage = 0;
    public float holyDamage = 0;
    
    [Header("Final Damage")]
    protected int finalDamageDealt = 0; // the damage the character takes after all calculations have been made
    
    [Header("Poise")]
    public float poiseDamage = 0;
    public bool poiseIsBroken = false; //If a character pose is broke, they will play stunned and play a damage Animation
    
    //(to do) Build Ups
    //build up effects poison, bleed

    [Header("Animation")] 
    public bool playDamageAnimation = true;
    public bool manuallySelectDamageAnimation = false;
    public string damageAnimation;

    [Header("Sound FX")] 
    public bool willPlayDamageSFX = true;
    public AudioClip elementalDamageSoundFX; //Used on top of regular sfx if there is elemental damage present(magic/fire/lighting/holy)
    
    [Header("Direction Damage Taken From")]
    public float angleHitFrom = 0; //Used to determine  what damage animation to play
    public Vector3 contactPoint; //used to determine the blood FX instantiate
    public override void ProcessEffect(CharacterManager characterManager)
    {
        if(characterManager.isVulnerable)
            return;
        
        base.ProcessEffect(characterManager);
        //If the character is dead, no additional damage effects should be processed
        if (characterManager.isDead)
            return;
        
        CalculateDamage(characterManager);
        PlayDirectionalBasedDamageAnimation(characterManager);
        PlayDamageSFX(characterManager);
        PlayDamageVFX(characterManager);
        
        // RUN THIS AFTER ALL OTHER FUNCTIONS THAT WOULD ATTEMPT TO PLAY AN ANIMATION UPON BEING DAMAGED & AFTER POISE/DAMAGE IS CALCULATED
        CalculateStanceDamage(characterManager);
    }
    
    protected virtual void CalculateDamage(CharacterManager characterManager)
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
        
        
        characterManager.currentHealth -= finalDamageDealt;
        
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


    protected void CalculateStanceDamage(CharacterManager characterManager)
    {
        AICharacterManager aiCharacterManager = characterManager as AICharacterManager;

        // YOU CAN OPTIONALLY GIVE WEAPONS THEIR OWN STANCE DAMAGE VALUES, OR USE POISE DAMAGE
        int stanceDamage = Mathf.RoundToInt(poiseDamage);
        
        if(aiCharacterManager != null)
        {
            aiCharacterManager.aiCharacterCombatManager.DamageStance(stanceDamage);
        }
        
    }

    protected void PlayDamageVFX(CharacterManager characterManager)
    {
        characterManager.characterEffectsManager.PlayBloodSplatterVFX(contactPoint);
    }

    protected void PlayDamageSFX(CharacterManager characterManager)
    {
        AudioClip physicalDamageSFX = WorldSoundFXManager.instance.ChooseRandomSfxFromArray(WorldSoundFXManager.instance.physicalDamageSFX);
        characterManager.characterSoundFXManager.PlaySoundFX(physicalDamageSFX);
        characterManager.characterSoundFXManager.PlayDamageGruntSoundFX();
    }

    protected void PlayDirectionalBasedDamageAnimation(CharacterManager characterManager)
    {
        if(characterManager.isDead)
            return;


        if (poiseIsBroken)
        {
            if (angleHitFrom >= 145 && angleHitFrom <= 180 || angleHitFrom <= -145 && angleHitFrom >= -180)
            {
                //PLAY FORWARD ANIMATION
                damageAnimation = characterManager.characterAnimatorManager.GetRandomAnimationFromList(characterManager.characterAnimatorManager.forward_Medium_Damage);
            }
            else if (angleHitFrom >= -45 && angleHitFrom <= 45)
            {
                //PLAY BACK ANIMATION
                damageAnimation = characterManager.characterAnimatorManager.GetRandomAnimationFromList(characterManager.characterAnimatorManager.backward_Medium_Damage);
            }
            else if (angleHitFrom >= -144 && angleHitFrom <= -46)
            {
                //PLAY LEFT ANIMATION
                damageAnimation = characterManager.characterAnimatorManager.GetRandomAnimationFromList(characterManager.characterAnimatorManager.left_Medium_Damage);
            }
            else if(angleHitFrom >= 45 && angleHitFrom <= 144)
            {
                //PLAY RIGHT ANIMATION
                damageAnimation = characterManager.characterAnimatorManager.GetRandomAnimationFromList(characterManager.characterAnimatorManager.right_Medium_Damage);
            }
        }
        else
        {
            if (angleHitFrom >= 145 && angleHitFrom <= 180 || angleHitFrom <= -145 && angleHitFrom >= -180)
            {
                //PLAY FORWARD ANIMATION
                damageAnimation = characterManager.characterAnimatorManager.GetRandomAnimationFromList(characterManager.characterAnimatorManager.forward_Ping_Damage);
            }
            else if (angleHitFrom >= -45 && angleHitFrom <= 45)
            {
                //PLAY BACK ANIMATION
                damageAnimation = characterManager.characterAnimatorManager.GetRandomAnimationFromList(characterManager.characterAnimatorManager.backward_Ping_Damage);
            }
            else if (angleHitFrom >= -144 && angleHitFrom <= -46)
            {
                //PLAY LEFT ANIMATION
                damageAnimation = characterManager.characterAnimatorManager.GetRandomAnimationFromList(characterManager.characterAnimatorManager.left_Ping_Damage);
            }
            else if(angleHitFrom >= 45 && angleHitFrom <= 144)
            {
                //PLAY RIGHT ANIMATION
                damageAnimation = characterManager.characterAnimatorManager.GetRandomAnimationFromList(characterManager.characterAnimatorManager.right_Ping_Damage);
            }
            
        }
        
        // IF POISE IS BROKEN, PLAY A STAGGERING DAMAGE ANIMATION
        characterManager.characterAnimatorManager.lastDamageAnimationPlayed = damageAnimation;

        if (poiseIsBroken)
        {
            // IF WE ARE POISE BROKEN RESTRICT OUR MOVEMENT AND ACTIONS 
            characterManager.characterAnimatorManager.PlayTargetActionAnimation(damageAnimation, true);
        }
        else
        {
            // IF WE ARE NOT POISE BROKEN SIMPLE PLAY AN UPPER BODY FLINCH ANIMATION WITHOUT RESTRICTING ANY MOVEMENTS OR ACTIONS
            characterManager.characterAnimatorManager.PlayTargetActionAnimation(damageAnimation, false, false, true, true);
        }
       
    }
}
