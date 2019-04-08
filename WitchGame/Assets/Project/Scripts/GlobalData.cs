using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GlobalData : MonoBehaviour
{
    public static GlobalData instance;

    /// <summary>
    /// Triggers that enemies need to check
    /// </summary>
    public IEnumerable<Trigger> enemyTriggers;
    /// <summary>
    /// Triggers that player needs to check
    /// </summary>
    public IEnumerable<Trigger> playerTriggers;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }
}
