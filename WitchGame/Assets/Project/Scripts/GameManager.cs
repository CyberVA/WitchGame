using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //References
    public static GameManager gMan;
    public AudioLibrary audioLibrary;
    Clickable_SceneNode clickableScene;

    //Sprites
    public SpriteRenderer fadeInOut;

    //Amount of player health
    [Range(0, 1)]
    public float playerHealth;
    //Amount of player Mana
    [Range(0,1)]
    public float mana;
    //cooldowns
    public float[] currentCooldowns = new float[5];
    //Scene management
    int countLoaded;
    public Scene currentScene;

    bool check;
    public bool loaded = false;
    double transparent = 1;

    // Start is called before the first frame update
    void Awake()
    {
        if (gMan == null)
        {
            gMan = this;
            DontDestroyOnLoad(transform.gameObject);
            SceneManager.activeSceneChanged += ChangedActiveScene;
            audioLibrary = GetComponent<AudioLibrary>();
            check = true;
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

        //Fade out
        if (loaded)
        {
            transparent -= 0.8f * Time.unscaledDeltaTime;
            //Debug.Log(transparent);
            fadeInOut.color = new Color(0f, 0f, 0f, (float)transparent);
            //Debug.Log(fadeInOut.color.a);
            if (transparent <= 0)
            {
                Debug.Log("loaded");
                transparent = 1;
                loaded = false;
            }
        }
    }

    private void ChangedActiveScene(Scene current, Scene next)
    {
        switch(next.name)
        {
            case "MainMenu":
                audioLibrary.musicSounds(music.MainMenu, true, 0.5f);
                break;
            case "Game":
                audioLibrary.musicSounds(music.Background, true, 0.3f);
                break;
            case "Win":
                audioLibrary.musicSounds(music.Win, true, 0.6f);  
                break;
            default:
                audioLibrary.musicSounds(music.MainMenu, true, 0.5f);
                break;
        }
        Debug.Log("Current: " + current.name + ", Next: " + next.name);
    }
}
