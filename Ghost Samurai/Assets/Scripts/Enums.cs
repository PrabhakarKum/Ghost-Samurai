using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enums : MonoBehaviour
{
    
}

public enum weaponModelSlot
{
    LeftHand,
    RightHand
}

public enum CharacterGroup
{
    Team1,
    Team2,
}

public enum WeaponClass
{
    Katana,
    StraightSword,
}

// THIS IS USED TO CALCULATE DAMAGE BASED ON ATTACK TYPE

public enum AttackType
{
    LightAttack01,
    LightAttack02,
    HeavyAttack01
    
}

public enum DamageIntensity
{
    Ping, 
    Light,
    Medium,
    Heavy,
    Colossal
}

// AI STATES
public enum IdleStateMode
{
    Idle,
    Patrol,

}
