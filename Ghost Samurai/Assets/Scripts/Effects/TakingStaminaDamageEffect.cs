using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

[CreateAssetMenu(menuName = "Character Effects/Instant Effects/Taking Stamina Damage")]
public class TakingStaminaDamageEffect : InstantCharacterEffect
{
    public float staminaDamage;
    
    public override void ProcessEffect(CharacterManager characterManager)
    {
        CalculateStaminaDamage(characterManager);
    }

    private void CalculateStaminaDamage(CharacterManager characterManager)
    {
        characterManager.currentStamina -= staminaDamage;
        
    }
}
