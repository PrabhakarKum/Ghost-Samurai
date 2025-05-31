using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldActionManager : MonoBehaviour
{
   public static WorldActionManager Instance;

   [Header("World Item Actions")] 
   public WeaponItemAction[] weaponItemAction;

   private void Awake()
   {
      if (Instance == null)
      {
         Instance = this;
      }
      else
      {
         Destroy(gameObject);
      }
      
      DontDestroyOnLoad(gameObject);
   }

   private void Start()
   {
      for (int i = 0; i < weaponItemAction.Length; i++)
      {
         weaponItemAction[i].actionID = i;
      }
   }

   public WeaponItemAction GetWeaponItemAction(int ID)
   {
      return weaponItemAction.FirstOrDefault(action => action.actionID == ID);
   }
}
