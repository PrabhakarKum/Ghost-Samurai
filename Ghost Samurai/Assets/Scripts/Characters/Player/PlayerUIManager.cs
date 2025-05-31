using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUIManager : MonoBehaviour
{
   public static PlayerUIManager instance;
   [HideInInspector] public PlayerUI_HUDManager hudManager;
   [HideInInspector] public PopUpManager popUpManager;
   
   private void Awake()
   {
      if (instance == null)
      {
         instance = this;
      }
      else
      {
         Destroy(gameObject);
      }
      
      hudManager = GetComponentInChildren<PlayerUI_HUDManager>();
      popUpManager = GetComponentInChildren<PopUpManager>();
   }
}
