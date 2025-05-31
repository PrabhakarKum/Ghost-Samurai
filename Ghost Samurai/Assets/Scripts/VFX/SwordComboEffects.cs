using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SwordComboEffects : MonoBehaviour
{
    public GameObject slashEffectPrefab;
    [HideInInspector] public Transform slashPoint;

    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T)) // Press T to test attack
        {
            GetComponent<Animator>().SetTrigger("Attack");
        }
    }
    public void PlaySlash()
    {
        GameObject slashVFX = Instantiate(slashEffectPrefab, slashPoint.position, slashPoint.rotation);
        slashVFX.transform.SetParent(slashPoint);
        Destroy(slashVFX, slashVFX.GetComponent<ParticleSystem>().main.duration);
    }
    
    public void SetSlashPoint(Transform newSlashPoint)
    {
        slashPoint = newSlashPoint;
    }
}
