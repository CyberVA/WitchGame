﻿using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "New CombatSettings", menuName = "CombatSettings")]
public class CombatSettings : ScriptableObject
{
    public CombatStats player, armShroom;
    public float playerMana;
    public Ability playerMelee, playerShroom;
    public float playerMeleeLength, playerShroomSpeed;
    public float minVelocity;
}

[System.Serializable]
public struct CombatStats
{
    public float hp, damage, moveSpeed, inertia, vMulitplierMelee, vMultiplirRange, flashLength, invincibleLength;
}

[System.Serializable]
public struct Ability
{
    public float cost, cooldown, damage;
}

