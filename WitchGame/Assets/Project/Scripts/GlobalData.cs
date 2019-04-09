using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GlobalData : MonoBehaviour
{
    public static GlobalData instance;

    /// <summary>
    /// Objects in level that can be physically hurt
    /// </summary>
    public List<IHurtable> combatTriggers;
    
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            combatTriggers = new List<IHurtable>();
        }
        else
        {
            Debug.LogWarning("There are multiple globaldata objects. dont do that");
        }
    }
}
