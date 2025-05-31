using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSaveGameManager : MonoBehaviour
{
    
    public static WorldSaveGameManager instance;
    public CharacterSaveData characterSaveData;
    
    
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
    }
}
