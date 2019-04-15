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
    
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < abilities.Length; i++)
        {
            //abilities[i].transform.localScale
        }
    }

    // Update is called once per frame
    void Update()
    {

        //For Debuging purposes only, delete this entire function when status bars is fully integrated with the player controller
        Health(GameManager.gMan.playerHealth);
        Mana(GameManager.gMan.mana);
    }

    /// <summary>
    /// Function for adding or subtracting hp from the players healthbar
    /// </summary>
    /// <param name="hp"> How much HP we're adding/subtracting</param>
    public void Health(float hp)
    {
        healthBar.size = new Vector2(hp, 1);
    }

    /// <summary>
    /// Function for adding or subtracting mana from the players healthbar
    /// </summary>
    /// <param name="hp"> How much MP we're adding/subtracting</param>
    public void Mana(float mp)
    {
        manaBar.size = new Vector2(mp, 1);
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
