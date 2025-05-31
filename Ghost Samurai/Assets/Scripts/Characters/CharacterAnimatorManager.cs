using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimatorManager : MonoBehaviour
{
    private CharacterManager _characterManager;
    private int vertical;
    private int horizontal;
    
    [Header("Flags")]
    public bool _applyRootMotion = false;

    [Header("Damage Animation")]
    public string lastDamageAnimationPlayed;
    
    // PING HIT REACTIONS
    [SerializeField] private string hit_Forward_Ping_01 = "Hit_Forward_Ping_01";
    [SerializeField] private string hit_Forward_Ping_02 = "Hit_Forward_Ping_02";
    
    [SerializeField] private string hit_Backward_Ping_01 = "Hit_Backward_Ping_01";
    [SerializeField] private string hit_Backward_Ping_02 = "Hit_Backward_Ping_02";
    
    [SerializeField] private string hit_Left_Ping_01 = "Hit_Left_Ping_01";
    [SerializeField] private string hit_Left_Ping_02 = "Hit_Left_Ping_02";
    
    [SerializeField] private string hit_Right_Ping_01 = "Hit_Right_Ping_01";
    [SerializeField] private string hit_Right_Ping_02 = "Hit_Right_Ping_02";

    public List<string> forward_Ping_Damage = new List<string>();
    public List<string> backward_Ping_Damage = new List<string>();
    public List<string> left_Ping_Damage = new List<string>();
    public List<string> right_Ping_Damage = new List<string>();
    
    // MEDIUM HIT REACTIONS
    [SerializeField] private string hit_Forward_Medium_01 = "Hit_Forward_Medium_01";
    [SerializeField] private string hit_Forward_Medium_02 = "Hit_Forward_Medium_02";
    
    [SerializeField] private string hit_Backward_Medium_01 = "Hit_Backward_Medium_01";
    [SerializeField] private string hit_Backward_Medium_02 = "Hit_Backward_Medium_02";
    
    [SerializeField] private string hit_Left_Medium_01 = "Hit_Left_Medium_01";
    [SerializeField] private string hit_Left_Medium_02 = "Hit_Left_Medium_02";
    
    [SerializeField] private string hit_Right_Medium_01 = "Hit_Right_Medium_01";
    [SerializeField] private string hit_Right_Medium_02 = "Hit_Right_Medium_02";

    public List<string> forward_Medium_Damage = new List<string>();
    public List<string> backward_Medium_Damage = new List<string>();
    public List<string> left_Medium_Damage = new List<string>();
    public List<string> right_Medium_Damage = new List<string>();

    protected virtual void Awake()
    {
        _characterManager = GetComponent<CharacterManager>();
        vertical = Animator.StringToHash("Vertical");
        horizontal = Animator.StringToHash("Horizontal");
    }

    protected virtual void Start()
    {
        forward_Ping_Damage.Add(hit_Forward_Ping_01);
        forward_Ping_Damage.Add(hit_Forward_Ping_02);
        
        backward_Ping_Damage.Add(hit_Backward_Ping_01);
        backward_Ping_Damage.Add(hit_Backward_Ping_02);
        
        left_Ping_Damage.Add(hit_Left_Ping_01);
        left_Ping_Damage.Add(hit_Left_Ping_02);
        
        right_Ping_Damage.Add(hit_Right_Ping_01);
        right_Ping_Damage.Add(hit_Right_Ping_02);
        
        
        
        forward_Medium_Damage.Add(hit_Forward_Medium_01);
        forward_Medium_Damage.Add(hit_Forward_Medium_02);
        
        backward_Medium_Damage.Add(hit_Backward_Medium_01);
        backward_Medium_Damage.Add(hit_Backward_Medium_02);
        
        left_Medium_Damage.Add(hit_Left_Medium_01);
        left_Medium_Damage.Add(hit_Left_Medium_02);
        
        right_Medium_Damage.Add(hit_Right_Medium_01);
        right_Medium_Damage.Add(hit_Right_Medium_02);
    }

    public string GetRandomAnimationFromList(List<string> animationList)
    {
        List<string> finalList = new List<string>();
        foreach (var item in animationList)
        {
            finalList.Add(item);
        }
        
        //// CHECK IF WE ARE ALREADY played this animation
        finalList.Remove(lastDamageAnimationPlayed);
        
        // check the llist for null entries
        for (int i = finalList.Count - 1 ; i > -1 ; i--)
        {
            if (finalList[i] == null)
            {
                finalList.RemoveAt(i);
            }
        }
        int randomValue = Random.Range(0, finalList.Count);
        return finalList[randomValue];
    }
    public void UpdateAnimatorMovementParameters(float horizontalValue, float verticalValue, bool isSprinting)
    {
        float snappedHorizontal = horizontalValue;
        float snappedVertical = verticalValue;

        // THIS CHAIN WILL ROUND THE HORIZONTAL MOVEMENT TO -1, -0.5, 0, 0.5, 1
        if (horizontalValue > 0 && horizontalValue <= 0.5f)
        {
            snappedHorizontal = 0.5f;
        }
        else if (horizontalValue > 0.5f && horizontalValue <= 1f)
        {
            snappedHorizontal = 1f;
        }
        else if (horizontalValue < 0 && horizontalValue >= -0.5f)
        {
            snappedHorizontal = -0.5f;
        }
        else if (horizontalValue < -0.5 && horizontalValue >= -1f)
        {
            snappedHorizontal = -1f;
        }
        else
        {
            snappedHorizontal = 0;
        }
        
        // THIS CHAIN WILL ROUND THE HORIZONTAL MOVEMENT TO -1, -0.5, 0, 0.5, 1
        
        if (verticalValue > 0 && verticalValue <= 0.5f)
        {
            snappedVertical = 0.5f;
        }
        else if (verticalValue > 0.5f && verticalValue <= 1)
        {
            snappedVertical = 1;
        }
        else if (verticalValue < 0 && verticalValue >= -0.5f)
        {
            snappedVertical = -0.5f;
        }
        else if (verticalValue < -0.5f && verticalValue >= -1)
        {
            snappedVertical = -1;
        }
        else
        {
            snappedVertical = 0;
        }
        
        if (isSprinting)
        {
            snappedVertical = 2;
        }
        
        _characterManager.animator.SetFloat(horizontal, snappedHorizontal, 0.1f, Time.deltaTime);
        _characterManager.animator.SetFloat(vertical, snappedVertical,0.1f, Time.deltaTime);

    }

    public virtual void PlayTargetActionAnimation(string targetAnimation, bool isPerformingAction, bool applyRootMotion = true, bool canRotate = false, bool canMove = false)
    {
        _applyRootMotion = applyRootMotion;
        _characterManager.animator.CrossFade(targetAnimation, 0.2f);
        _characterManager.isPerformingAction = isPerformingAction;
        _characterManager.characterLocomotionManager.canMove = canMove;
        _characterManager.characterLocomotionManager.canRotate = canRotate;
    }
    
    public virtual void PlayTargetActionAnimationInstantly(string targetAnimation, bool isPerformingAction, bool applyRootMotion = true, bool canRotate = false, bool canMove = false)
    {
        _applyRootMotion = applyRootMotion;
        _characterManager.animator.Play(targetAnimation);
        _characterManager.isPerformingAction = isPerformingAction;
        _characterManager.characterLocomotionManager.canMove = canMove;
        _characterManager.characterLocomotionManager.canRotate = canRotate;
    }
    
    public virtual void PlayTargetAttackActionAnimation(AttackType attackType ,string targetAnimation, bool isPerformingAction, bool applyRootMotion = true, bool canRotate = false, bool canMove = false)
    {
        // Keep track of Last Attack performed(combos)
        // keep track of current attack type (light, heavy, etc)
        //update animation set of current weapon animation
        // decide if our attack can be parried
        _characterManager.characterCombatManager.currentAttackType = attackType;
        _characterManager.characterCombatManager.lastAttackAnimationPerformed = targetAnimation;
        _applyRootMotion = applyRootMotion;
        _characterManager.animator.CrossFade(targetAnimation, 0.2f);
        _characterManager.isPerformingAction = isPerformingAction;
        _characterManager.characterLocomotionManager.canMove = canMove;
        _characterManager.characterLocomotionManager.canRotate = canRotate;
    }
    
    
    
    public virtual void EnableCanDoCombo()
    {
    }
    
    public virtual void DisableCanDoCombo()
    {
        
    }
}
