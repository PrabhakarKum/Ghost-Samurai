using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponModelInstantiationSlot : MonoBehaviour
{
    public weaponModelSlot weaponSlot;
    public GameObject currentWeaponModel;

    public void UnloadWeapon()
    {
        if (currentWeaponModel != null)
        {
            Destroy(currentWeaponModel);
            currentWeaponModel = null;
        }
    }                                                                

    public void LoadWeapon(GameObject weaponModel)
    {
        currentWeaponModel = weaponModel;
        weaponModel.transform.parent = transform;
        
        weaponModel.transform.localPosition = Vector3.zero;
        weaponModel.transform.localRotation =  Quaternion.Euler(0, -90, 0);                                                       
    }
}
