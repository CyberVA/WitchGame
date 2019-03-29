using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : Clickable
{

    public string[] scenes = new string[2];

    // Start is called before the first frame update
    void Start()
    {
        scenes[1] = "Project/Scenes/Official";
        scenes[2] = "Project/scenes/Vincent";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnClick()
    {
        SceneManager.LoadScene(scenes[1]);
    }
}
