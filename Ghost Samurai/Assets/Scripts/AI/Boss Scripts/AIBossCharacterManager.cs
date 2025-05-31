using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBossCharacterManager : AICharacterManager
{
    public int bossID = 0;
    [SerializeField] bool hasBeenDefeated = false;
    
    
    // [Header("Test")]
    //
    // public void onNetworkSpawn()
    // {
    //     if (isserver)
    //     {
    //         if (!WorldSaveGameManager.instance.characterSaveData.bossesAwakened.ContainsKey(bossID))
    //         {
    //             WorldSaveGameManager.instance.characterSaveData.bossesAwakened.Add(bossID, false);
    //         }
    //         else
    //         {
    //             hasBeenDefeated = WorldSaveGameManager.instance.characterSaveData.bossesDefeated[bossID];
    //
    //
    //             if (hasBeenDefeated)
    //             {
    //                 gameObject.SetActive(false);
    //             }
    //         }
    //     }
    // }
    
}
