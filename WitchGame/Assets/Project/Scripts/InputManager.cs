using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager IM;

    //This chunk is dedicated to setting up keycodes for all the player's actions. Any Script will be able to access these.
    public KeyCode Up { get; set; }
    public KeyCode Down { get; set; }
    public KeyCode Left { get; set; }
    public KeyCode Right { get; set; }
    public KeyCode Melee { get; set; }
    public KeyCode SpellPri { get; set; }
    public KeyCode SpellSec { get; set; }
    public KeyCode SpellTer { get; set; }

    void Awake()
    {
        IM = this;

        //Up = (KeyCode) and heres where i write the parts i dont understand but im not cheap and i want to understand what im looking at first so give me a hot second
        Up = (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("upKey", "W"));
        Down = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("downKey", "S"));
        Left = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("leftKey", "A"));
        Right = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("rightKey", "D"));
        Melee = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("melee", "Q"));
        SpellPri = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("fire1", "1"));
        SpellSec = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("fire2", "2"));
        SpellTer = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("fire3", "3"));
    }
}
