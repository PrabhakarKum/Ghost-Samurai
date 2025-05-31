using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldAIManager : MonoBehaviour
{
    public static WorldAIManager instance;

    [Header("Characters")] 
    [SerializeField] private List<AICharacterSpawner> aiCharacterSpawners;
    [SerializeField] private List<GameObject> spawnedInCharacters;

    [Header("Patrol Paths")]
    [SerializeField] private List<AIPatrolPath> aiPatrolPaths = new List<AIPatrolPath>();
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
    public void SpawnCharacter(AICharacterSpawner aiCharacterSpawner)
    {
        aiCharacterSpawners.Add(aiCharacterSpawner);
        aiCharacterSpawner.AttemptToSpawnCharacter();
    }

    private void DespawnAllCharacters()
    {
        foreach (var character in spawnedInCharacters)
        {
            Destroy(character);
        }
        spawnedInCharacters.Clear();
    }

    private void DisableAllCharacters()
    {
        // TO DO DISABLE CHARACTERS GAME OBJECT
        // CHARACTERS CAN BE ENABLED BY AREA 
    }

    public void AddPatrolPathToList(AIPatrolPath patrolPath)
    {
        if(aiPatrolPaths.Contains(patrolPath))
            return;
        
        aiPatrolPaths.Add(patrolPath);
    }

    public AIPatrolPath GetAIPatrolPathByID(int patrolPathID)
    {
        AIPatrolPath patrolPath = null;
        
        for (int i = 0; i < aiPatrolPaths.Count; i++)
        {
            if (aiPatrolPaths[i].patrolPathID == patrolPathID)
                patrolPath =  aiPatrolPaths[i];
        }
        return patrolPath;
    }
}
