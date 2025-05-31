using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Actions/ Attack Actions")]
public class AICharacterAttackAction : ScriptableObject
{
    [Header("Attack Animation")] 
    [SerializeField] private string attackAnimation;
    
    [Header("Combo Action")]
    public bool actionHasCombo = false; // if this action has a combo action
    public AICharacterAttackAction comboAction; // the combo action of this attack action

    [Header("Action Values")] 
    [SerializeField] private AttackType attackType;
    
    public int attackWeight = 50;
    public float actionRecoveryTime = 0.5f;
    public float minimumAttackAngle = -35;
    public float maximumAttackAngle = 35;
    public float minimumAttackDistance = 0;
    public float maximumAttackDistance = 4;
    
    
    public void AttemptToPerformAction(AICharacterManager aiCharacter)
    {
        aiCharacter.characterAnimatorManager.PlayTargetAttackActionAnimation(attackType , attackAnimation, true);
    }
}
