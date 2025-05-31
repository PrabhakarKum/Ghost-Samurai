using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI_HUDManager : MonoBehaviour
{
    [Header("STAT BAR")]
    [SerializeField] private UI_StatBar healthBar;
    [SerializeField] private UI_StatBar staminaBar;

    [Header("QUICK SLOTS")] 
    [SerializeField] private Image rightWeaponQuickSlotIcon;
    [SerializeField] private Image leftWeaponQuickSlotIcon;

    [Header("ENEMY HEALTH BAR")] 
    public Transform enemyHealthBarParent;
    public GameObject enemyHealthBarObject;
    
    
    
    private void OnEnable()
    {
        GameEvents.OnHealthChanged += UpdateHealthUI;
        GameEvents.OnStaminaChanged += UpdateStaminaUI;
        GameEvents.OnVitalityChanged += UpdateMaxHealthUI;
        GameEvents.OnEnduranceChanged += UpdateMaxStaminaUI;
    }

    private void OnDisable()
    {
        GameEvents.OnHealthChanged -= UpdateHealthUI;
        GameEvents.OnStaminaChanged -= UpdateStaminaUI;
        GameEvents.OnVitalityChanged -= UpdateMaxHealthUI;
        GameEvents.OnEnduranceChanged -= UpdateMaxStaminaUI;
    }

    public void RefreshHUD()
    {
        healthBar.gameObject.SetActive(false);
        staminaBar.gameObject.SetActive(false);
        healthBar.gameObject.SetActive(true);
        staminaBar.gameObject.SetActive(true);
    }

    private void UpdateHealthUI(CharacterManager sender,float oldHealth, float newHealth)
    {
        if(sender is PlayerManager)
            healthBar.SetStat(Mathf.RoundToInt(newHealth));
    }

    public void UpdateMaxHealthUI(int oldMaxHealth,int maxHealth)
    {
        healthBar.SetMaxStat(maxHealth);
    }
    
    public void UpdateStaminaUI(float oldStamina, float newStamina)
    {
        staminaBar.SetStat(Mathf.RoundToInt(newStamina));
    }

    public void UpdateMaxStaminaUI(int oldMaxStamina,int maxStamina)
    {
        staminaBar.SetMaxStat(maxStamina);
    }

    public void SetRightWeaponQuickSlotIcon(int weaponID)
    {
        WeaponItems weaponItems = WorldItemDatabase.instance.GetWeaponByID(weaponID);
        if ( weaponItems == null)
        {
            Debug.Log("Item is null");
            rightWeaponQuickSlotIcon.enabled = false;
            rightWeaponQuickSlotIcon.sprite = null;
            return;
        }

        if (weaponItems.itemIcon == null)
        {
            Debug.Log("No item icon");
            rightWeaponQuickSlotIcon.enabled = false;
            rightWeaponQuickSlotIcon.sprite = null;
            return;
        }
        
        rightWeaponQuickSlotIcon.enabled = true;
        rightWeaponQuickSlotIcon.sprite = weaponItems.itemIcon;
    }
    
    public void SetLeftWeaponQuickSlotIcon(int weaponID)
    {
        WeaponItems weaponItems = WorldItemDatabase.instance.GetWeaponByID(weaponID);
        if ( weaponItems == null)
        {
            Debug.Log("Item is null");
            leftWeaponQuickSlotIcon.enabled = false;
            leftWeaponQuickSlotIcon.sprite = null;
            return;
        }

        if (weaponItems.itemIcon == null)
        {
            Debug.Log("No item icon");
            leftWeaponQuickSlotIcon.enabled = false;
            leftWeaponQuickSlotIcon.sprite = null;
            return;
        }
        
        leftWeaponQuickSlotIcon.enabled = true;
        leftWeaponQuickSlotIcon.sprite = weaponItems.itemIcon;
    }
    
    
}
