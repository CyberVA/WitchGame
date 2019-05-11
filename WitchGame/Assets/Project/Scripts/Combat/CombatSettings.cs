using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "New CombatSettings", menuName = "CombatSettings")]
public class CombatSettings : ScriptableObject
{
    public CombatStats player, armShroom, geblin;
    public float playerMana;
    public Ability playerMelee, playerShroom, playerHeal, playerCloud;
    public float playerMeleeLength, playerShroomSpeed, playerCloudLife, playerCloudMoveTime, playerCloudSpeed;
    public float armShroomAttackTriggerRange, armShroomAttackRange, armShroomAttackCooldown, armShroomAttackPrep, armShroomAttackDamage;
    public float geblinStabDelay, geblinStabDamage, geblinStabRecover, geblinStabBeginRange, geblinStopMoveRange, geblinStabRange;
    public float slideSpeed;
    public float slowEffectLength;
    public float slowEffectMultiplier;
    public float minVelocity;
}

[System.Serializable]
public struct CombatStats
{
    public float hp, moveSpeed, inertia, vMulitplierMelee, vMultiplierRange, flashLength, invincibleLength;
}

[System.Serializable]
public struct Ability
{
    public float cost, cooldown, damage;
}

