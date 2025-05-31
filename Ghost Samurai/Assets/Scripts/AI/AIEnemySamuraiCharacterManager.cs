using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIEnemySamuraiCharacterManager : AICharacterManager
{
    [Header("Phase Shift")] 
    public float minimumHealthPercentageToShift = 50;
    [SerializeField] AI_CombatStanceState phase02CombatStanceState;
    
    protected override void OnEnable()
    {
        GameEvents.EnemyFightIsActive += OnEnemyIsActiveChanged;
    }
    
    private void OnEnemyIsActiveChanged(bool oldStatus, bool newStatus)
    {
        if (enemyFightIsActive)
        {
            GameObject enemyHealthBar = 
                Instantiate(PlayerUIManager.instance.hudManager.enemyHealthBarObject, PlayerUIManager.instance.hudManager.enemyHealthBarParent);

            UIEnemyHPBar enemyHpBar = enemyHealthBar.GetComponentInChildren<UIEnemyHPBar>();
            enemyHpBar.EnableEnemyHPBar(this);
        }
        
    }
    
    public override IEnumerator ProcessDeathEvent(bool manuallySelectDeathAnimation = false)
    {
        PlayerUIManager.instance.popUpManager.SendBossDefeatedPopup();
        return base.ProcessDeathEvent(manuallySelectDeathAnimation);
    }

    protected void PhaseShift()
    {
        Debug.Log("Phase 2 started");
        combatStanceStateTemplate = Instantiate(phase02CombatStanceState);
        combatStance = combatStanceStateTemplate;
        currentState = combatStanceStateTemplate;
    }

    protected override void CheckHP(CharacterManager characterManager, int aicurrentHealth)
    {
        base.CheckHP(this, aicurrentHealth);
        
        if(aicurrentHealth <= 0)
            return;
        
        float healthNeededToShift = maxHealth * (minimumHealthPercentageToShift / 100f);
        if (aicurrentHealth <= healthNeededToShift)
        {
            PhaseShift();
        }
        
    }
}
