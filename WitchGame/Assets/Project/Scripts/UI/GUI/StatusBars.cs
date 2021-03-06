using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StatusBars : MonoBehaviour
{
    //SpriteRenderers
    public SpriteRenderer[] abilities = new SpriteRenderer[4];
    public SpriteRenderer healthBar;
    public SpriteRenderer manaBar;

    /// <summary>
    /// Function for adding or subtracting hp from the players healthbar
    /// </summary>
    /// <param name="hp"> How much HP we're adding/subtracting</param>
    public void Health(float hp)
    {
        healthBar.size = new Vector2(hp * 3.9f, .5f);
    }

    /// <summary>
    /// Function for adding or subtracting mana from the players healthbar
    /// </summary>
    /// <param name="hp"> How much MP we're adding/subtracting</param>
    public void Mana(float mp)
    {
        manaBar.size = new Vector2(mp * 3.9f, .5f);
    }

    /// <summary>
    /// Function for controlling the cooldowns for each ability
    /// </summary>
    /// <param name="sprite"> Which sprite are we applying our cooldown to?</param>
    /// <param name="activated"> Was the ability used?</param>
    public void CoolDowns(int abilityID, float curCool)
    {
        abilities[abilityID].size = new Vector2(curCool, 1);
    }
}
