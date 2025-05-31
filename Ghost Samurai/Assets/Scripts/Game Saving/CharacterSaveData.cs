using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterSaveData
{
    [Header("Character Name")]
    public string characterName;
    
    
    [Header("Time Played")]
    public float secondPlayed;


    [Header("World Coordinates")] 
    public float xPosition;
    public float yPosition;
    public float zPosition;
    
    
    [Header("Bosses")]
    public SerializableDictionary<int, bool>bossesAwakened; // THE INT IS THE BOSS I.D, THE BOOL IS THE AWAKENED STATUS
    public SerializableDictionary<int, bool>bossesDefeated; // THE INT IS THE BOSS I.D, THE BOOL IS THE DEFEATED STATUS

    public CharacterSaveData()
    {
        bossesAwakened = new SerializableDictionary<int, bool>();
        bossesDefeated = new SerializableDictionary<int, bool>();
    }
}
