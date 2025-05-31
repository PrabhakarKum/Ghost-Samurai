using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class CharacterEffectsManager : MonoBehaviour
{
    CharacterManager _characterManager;

    [Header("VFX")] 
    [SerializeField] private GameObject bloodSplatterVFX;
    [SerializeField] private GameObject criticalBloodSplatterVFX;

    protected virtual void Awake()
    {
        _characterManager = GetComponent<CharacterManager>();
    }

    public virtual void ProcessInstantEffect(InstantCharacterEffect effect)
    {
        effect.ProcessEffect(_characterManager);
    }

    public void PlayBloodSplatterVFX(Vector3 contactPoint)
    {
        //IF WE MANUALLY HAVE PLACED A BLOOD SPLATTER VFX ON THIS MODEL, PLAY ITS VERSION
        if (bloodSplatterVFX != null)
        {
            GameObject bloodSplatter = Instantiate(bloodSplatterVFX, contactPoint, Quaternion.identity);
            
        }
        
        //WE USE THE DEFAULT BLOOD SPLATTER
        else
        {
            GameObject bloodSplatter = Instantiate(WorldCharacterEffectsManager.instance.bloodSplatterVFX, contactPoint, Quaternion.identity);
        }
    }
    
    public void PlayCriticallyBloodSplatterVFX(Vector3 contactPoint)
    {
        //IF WE MANUALLY HAVE PLACED A BLOOD SPLATTER VFX ON THIS MODEL, PLAY ITS VERSION
        if (bloodSplatterVFX != null)
        {
            GameObject bloodSplatter = Instantiate(criticalBloodSplatterVFX, contactPoint, Quaternion.identity);
            
        }
        
        //WE USE THE DEFAULT BLOOD SPLATTER
        else
        {
            GameObject bloodSplatter = Instantiate(WorldCharacterEffectsManager.instance.criticalBloodSplatterVFX, contactPoint, Quaternion.identity);
        }
    }
}
