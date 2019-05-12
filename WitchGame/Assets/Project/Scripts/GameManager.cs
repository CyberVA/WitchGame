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

    //GameObjects
    public Clickable_SliderNode VolumeSlider;

    //SpriteRenderers
    public SpriteRenderer fadeInOut;

    //Scene management
    public Scene currentScene;

    //Floats
    public float[] currentCooldowns = new float[5];

    [Range(0, 1)]
    public float mana;

    [Range(0, 1)]
    public float playerHealth;

    public float volume;

    //Integers
    int countLoaded;

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
        if(SceneManager.sceneCount < 2)
        {
            VolumeSlider = FindObjectOfType<Clickable_SliderNode>();
        }
        if(VolumeSlider != null) volume = VolumeSlider.output;
        gMan.audioLibrary.musicPlayer.volume = volume;
        //Debug.Log("Volume: " + volume);
        //vince.......... this kills me to see here.
    }

    private void ChangedActiveScene(Scene current, Scene next)
    {
        Time.timeScale = 1f;
        switch(next.name)
        {
            case "MainMenu":
                audioLibrary.musicSounds(music.MainMenu, true, 5 * volume);
                break;
            case "Game":
                audioLibrary.musicSounds(music.Background, true, 3 * volume);
                break;
            default:
                audioLibrary.musicSounds(music.MainMenu, true, 5 * volume);
                break;
        }
        Debug.Log("Current: " + current.name + ", Next: " + next.name);
    }
}
