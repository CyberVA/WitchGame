using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager gMan;

    // Start is called before the first frame update
    void Start()
    {

        DontDestroyOnLoad(transform.gameObject);
        if (gMan == null)
        {
            gMan = this;
        }
        else
        {
            DestroyObject(gameObject);
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
