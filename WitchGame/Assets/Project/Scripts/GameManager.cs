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

    // Start is called before the first frame update
    void Awake()
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

        if (currentScene.name == "MainMenu" && check)
        {
            check = false;
            audioLibrary.musicSounds(music.MainMenu, true);
        }
        else if(currentScene.name != "MainMenu" && !check)
        {
            check = true;
            audioLibrary.musicSounds(music.MainMenu, false);
        }

        /*if (SceneManager.GetSceneByName("Game") != null && SceneManager.GetSceneByName("MainMenu") == null) audioLibrary.musicSounds(music.Background, true, false);
        else audioLibrary.musicSounds(music.Background, false, true);*/

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

        if (clickableScene.loaded)
        {
            clickableScene.transparent -= 0.2f * Time.deltaTime;
            Debug.Log(clickableScene.transparent);
            GameManager.gMan.fadeInOut.color = new Color(0f, 0f, 0f, (float)clickableScene.transparent);
            if (clickableScene.transparent <= 0)
            {
                Debug.Log("loaded");
                clickableScene.loaded = false;
            }
        }
    }
}
