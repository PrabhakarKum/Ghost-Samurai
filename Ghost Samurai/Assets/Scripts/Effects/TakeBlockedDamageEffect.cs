using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Character Effects/Instant Effects/Take Blocked Damage")]
public class TakeBlockedDamageEffect : InstantCharacterEffect
{
    [Header("Character Causing Damage")]
    public CharacterManager _characterCausingDamage; //If the damage is caused by another character attack it will be stored here
    
    [Header("Damage")]
    public float physicalDamage = 0;
    public float magicDamage = 0;
    public float fireDamage = 0;
    public float lightningDamage = 0;
    public float holyDamage = 0;
    
    [Header("Final Damage")]
    public int finalDamageDealt = 0; // the damage the character takes after all calculations have been made
    
    [Header("Poise")]
    public float poiseDamage = 0;
    public bool poiseIsBroken = false; //If a character pose is broke, they will play stunned and play a damage Animation
    
    [Header("Stamina")]
    public float staminaDamage = 0;
    public float finalStaminaDamage = 0;
    
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
        
        Debug.Log("Hit was blocked");
        //If the character is dead, no additional damage effects should be processed
        if (characterManager.isDead)
            return;
        
        CalculateDamage(characterManager);
        CalculateStaminaDamage(characterManager);
        PlayDirectionalBasedDamageAnimation(characterManager);
        PlayDamageSFX(characterManager);
        
        
        CheckForGuardBreak(characterManager);
    }
    
    private void CalculateDamage(CharacterManager characterManager)
    {
        if (_characterCausingDamage != null)
        {
            
        }
        //check character for flat defences and subtract them from the damage
        
        //check character for armour absorptions, and subtract the percentage from the damage
        
        //add all the damage types together, and apply final damage
        
        Debug.Log("original physical damage before: "+ physicalDamage);
        
        
        Debug.Log("original physical damage absoprtion: "+ characterManager.characterStatManager.blockingPhysicalAbsorption);
        
        physicalDamage -= (physicalDamage * (characterManager.characterStatManager.blockingPhysicalAbsorption/100));
        magicDamage -= (magicDamage * (characterManager.characterStatManager.blockingMagicAbsorption/100));
        fireDamage -= (fireDamage * (characterManager.characterStatManager.blockingFireAbsorption/100));
        lightningDamage -= (lightningDamage * (characterManager.characterStatManager.blockingLightningAbsorption/100)); 
        holyDamage -= (holyDamage * (characterManager.characterStatManager.blockingHolyAbsorption/100));
        
        Debug.Log("original physical damage after: "+ physicalDamage);
        
        finalDamageDealt = Mathf.RoundToInt(physicalDamage + magicDamage + fireDamage + lightningDamage + holyDamage);
        if (finalDamageDealt <= 0)
        {
            finalDamageDealt = 0;
        }
        
        Debug.Log("Final blocked damage dealt: " + finalDamageDealt);
        
        characterManager.currentHealth -= finalDamageDealt;
        Debug.Log("Enemy current health after block: "+ characterManager.currentHealth);
    }
    private void CalculateStaminaDamage(CharacterManager characterManager)
    {
        finalStaminaDamage = staminaDamage;

        float staminaDamageAbsorption = finalStaminaDamage * (characterManager.characterStatManager.blockingStability / 100);
        float staminaDamageAfterAbsorption =  finalStaminaDamage - staminaDamageAbsorption;
        
        characterManager.currentStamina -= staminaDamageAfterAbsorption;

    }

    private void CheckForGuardBreak(CharacterManager characterManager)
    {
        // if (characterManager.currentStamina <= 0)
        //PLAY SFX
        
        if (characterManager.currentStamina <= 0)
        {
            characterManager.characterAnimatorManager.PlayTargetActionAnimation("Guard_Break_01", true);
            characterManager.isBlocking = false;
            
        }
    }

    private void PlayDamageSFX(CharacterManager characterManager)
    {
        characterManager.characterSoundFXManager.PlayBlockSFX();

        // 1. Playing Blocking SFX on blocking weapon
    }

    private void PlayDirectionalBasedDamageAnimation(CharacterManager characterManager)
    {
        //To do calculate if poise is broken
        poiseIsBroken = true;
        
        if(characterManager.isDead)
            return;
        
        DamageIntensity damageIntensity = WorldUtiityManagers.Instance.GetDamageIntensityBasedOnPoiseDamage(poiseDamage);

        switch (damageIntensity)
        {
            case DamageIntensity.Ping:
                damageAnimation = "Block_Ping_01";
                break;
            case DamageIntensity.Light:
                damageAnimation = "Block_Light_01";
                break;
            case DamageIntensity.Medium:
                damageAnimation = "Block_Medium_01";
                break;
            case DamageIntensity.Heavy:
                damageAnimation = "Block_Heavy_01";
                break;
            case DamageIntensity.Colossal:
                damageAnimation = "Block_Colossal_01";
                break;
        }
        
        // IF POISE IS BROKEN, PLAY A STAGGERING DAMAGE ANIMATION
        characterManager.characterAnimatorManager.lastDamageAnimationPlayed = damageAnimation;
        characterManager.characterAnimatorManager.PlayTargetActionAnimation(damageAnimation, true);

    }
}
