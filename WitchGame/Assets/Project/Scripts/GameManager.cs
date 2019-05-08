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
        SceneManager.activeSceneChanged += ChangedActiveScene;

        DontDestroyOnLoad(transform.gameObject);
        if (gMan == null)
        {
            gMan = this;
        }
        else
        {
            Destroy(gameObject);
        }
        audioLibrary = GetComponent<AudioLibrary>();
        check = true;
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

        currentScene = SceneManager.GetActiveScene();

        //Audio switching on scenes
        //name i main check is true
        /*if (currentScene.name == "MainMenu" && check)
        {
            check = false; //checj us fajske
            audioLibrary.musicSounds(music.MainMenu, true);
        }
        else if(currentScene.name != "MainMenu" && !check)
        {
            check = true;
            audioLibrary.musicSounds(music.MainMenu, false);
        }

        if (currentScene.name == "Game" && check)
        {
            check = false;
            audioLibrary.musicSounds(music.Background, true);
        }
        else if (currentScene.name != "Game" && !check)
        {
            check = true;
            audioLibrary.musicSounds(music.Background, false);
        }*/

        /*if (SceneManager.sceneCount < 2)
        {
            
            countLoaded = SceneManager.sceneCount;
            loadedScene = new Scene[countLoaded];
            Debug.Log(countLoaded);
            for (int i = 0; i < countLoaded; i++)
            {
                loadedScene[i] = SceneManager.GetSceneAt(i);
            }
            Debug.Log("Scene 1: " + loadedScene[0].name + ", Scene 2: " + loadedScene[1].name);
        }*/


        //Fade out
        if (loaded)
        {
            transparent -= 0.8f * Time.deltaTime;
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
                audioLibrary.musicSounds(music.MainMenu, true);
                break;
            case "Game":
                audioLibrary.musicSounds(music.Background, true);
                break;
            default:
                audioLibrary.musicSounds(music.MainMenu, true);
                break;
        }
        Debug.Log("Current: " + current.name + ", Next: " + next.name);
    }
}
