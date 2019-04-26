using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static GameManager gMan;

    //Amount of player health
    [Range(0, 1)]
    public float playerHealth;
    //Amount of player Mana
    [Range(0,1)]
    public float mana;
    public float[] currentCooldowns = new float[5];

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
            Destroy(gameObject);
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            SceneManager.LoadScene("LevelEditor");
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}
