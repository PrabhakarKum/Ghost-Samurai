using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEffectsManagers : CharacterEffectsManager
{
    [Header("Debug Player Effects Managers")] [SerializeField]
    private InstantCharacterEffect effectToTest;
    [SerializeField] private bool processEffect = false;

    private void Update()
    {
        if (processEffect)
        {
            processEffect = false;
            InstantCharacterEffect effect = Instantiate(effectToTest);
            ProcessInstantEffect(effect);
        }
    }
}
