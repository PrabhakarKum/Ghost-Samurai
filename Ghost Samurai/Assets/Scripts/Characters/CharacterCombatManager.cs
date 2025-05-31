using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class CharacterCombatManager : MonoBehaviour
{
    public CharacterManager characterManager;

    [Header("Last Attack Animation Performed")]
    public string lastAttackAnimationPerformed;
    
    [Header("Previous Poise Damage Taken")]
    public float previousPoiseDamageTaken;
    
    [Header("Attack Target")]
    public CharacterManager currentTarget;
    
    [Header("Attack Type")]
    public AttackType currentAttackType;
    
    [Header("Lock on Transform")]
    public Transform lockOnTransform;

    [Header("Attack Flags")] 
    public bool canBlock = true;

    [Header("Critical Attack")]
    private Transform riposteReceiverTransform;
    [SerializeField] private float criticalAttackDistanceCheck = 0.7f;
    [SerializeField] public int pendingCriticalDamage;
    protected virtual void Awake()
    {
        characterManager = GetComponent<CharacterManager>();
    }

    protected virtual void Update()
    {
        
    }

    public virtual void SetTarget(CharacterManager newTarget)
    {
        if (newTarget != null)
        {
            currentTarget = newTarget;
            
        }
        else
        {
            currentTarget = null;
        }
    }

    //  USED TO ATTEMPT BACKSTAB/RIPOSTE
    public  void AttemptCriticalAttack()
    {
        if(characterManager.isPerformingAction)
            return;
        
        if(characterManager.currentStamina <= 0)
            return;
        
        RaycastHit[] hits = Physics.RaycastAll(characterManager.characterCombatManager.lockOnTransform.position, 
            characterManager.transform.TransformDirection(Vector3.forward), criticalAttackDistanceCheck, 
            WorldUtiityManagers.Instance.GetCharacterLayers());
        
        for (int i = 0; i < hits.Length; i++)
        {
            // CHECK EACH OF THE HITS 1 BY 1, CHECK THEM THEIR OWN VARIABLE
            RaycastHit hit = hits[i];
            
            CharacterManager targetCharacter = hit.transform.GetComponent<CharacterManager>();

            if (targetCharacter != null)
            {
                // IF THE CHARACTER  IS THE ONE ATTEMPTING THE CRITICAL STRIKE, GO TO THE NEXT HIT IN THE ARRAY OF TOTAL HITS
                if(targetCharacter == characterManager)
                    continue;

                // IF WE CANNOT DAMAGE THE CHARACTER THAT IS TARGETED CONTINUE TO CHECK THE NEXT HIT IN THE ARRAY OF TOTAL HITS
                if (!WorldUtiityManagers.Instance.CanIDamageThisTarget(characterManager.characterGroup, targetCharacter.characterGroup))
                    continue;
            
                Vector3 directionFromCharacterToTarget = characterManager.transform.position - targetCharacter.transform.position;
                float targetViewableAngle = Vector3.SignedAngle(directionFromCharacterToTarget,
                    targetCharacter.transform.forward, Vector3.up);
                

                if (targetCharacter.isRipostable)
                {
                    
                    if (targetViewableAngle >= -90 && targetViewableAngle <= 90)
                    {
                       
                        AttemptRiposte(characterManager,hit);
                        return;
                    }
                }
                
                // TO DO ADD BACKSTAB CHECK
            }
        }
    }

    public virtual void AttemptRiposte(CharacterManager characterManager ,RaycastHit hit)
    {
        
    }
    
    public void ApplyCriticalDamage()
    {
        characterManager.characterEffectsManager.PlayCriticallyBloodSplatterVFX(characterManager.characterCombatManager.lockOnTransform.position);
        characterManager.characterSoundFXManager.PlayCriticallyStrikeSoundFX();
        characterManager.currentHealth -= pendingCriticalDamage;
    }


    public IEnumerator ForceMoveEnemyCharacterToRipostePosition(CharacterManager enemyCharacter, Vector3 ripostePosition)
    {
        float timer = 0;

        while (timer < 0.5f)
        {
            timer += Time.deltaTime;
            if (riposteReceiverTransform == null)
            {
                GameObject riposteReceiverGameObject = new GameObject("Riposte Transform");
                riposteReceiverGameObject.transform.position = Vector3.zero;
                
                riposteReceiverTransform = riposteReceiverGameObject.transform;
            }
            
            riposteReceiverTransform.localPosition = ripostePosition;
            enemyCharacter.transform.position = riposteReceiverTransform.position;
            transform.rotation = Quaternion.LookRotation(-enemyCharacter.transform.forward);
            
            yield return null;
        }
        
    }

    public void EnableIsInvulnerable()
    {
        characterManager.isVulnerable = true;
    }
    
    public void DisableIsInvulnerable()
    {
        characterManager.isVulnerable = false;
    }

    public void EnableIsRipostable()
    {
        characterManager.isRipostable = true;
    }
}
