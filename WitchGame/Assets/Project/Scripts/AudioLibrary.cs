using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;

public enum playerEffects { Attack, Damaged, Idle, MushMancy, RootWall, Cure }
public enum mushroomEffects { Attack, Damaged, Idle}
public enum goblinEffects { Attack, Damage, Idle}
public enum walk { WalkLight, WalkMedium, WalkHeavy}
public enum miscEffects { ButtonSelected }
public enum music { MainMenu, Background, Win, Lose}

public class AudioLibrary : MonoBehaviour
{

    public AudioClip[] player;
    public AudioClip[] mushroom;
    public AudioClip[] goblin;
    public AudioClip[] walk;
    public AudioClip[] misc;
    public AudioClip[] music;

    [Range(0, 1)]
    float volume;

    public AudioSource musicPlayer;

    private void Awake()
    {
        
    }

    /// <summary>
    /// Plays a combat sound at a certain point
    /// </summary>
    /// <param name="s">Combat sound</param>
    public void PlayerSounds(playerEffects s)
    {
        AudioSource.PlayClipAtPoint(player[(int)s], Vector3.zero);
    }
    /// <summary>
    /// Plays a combat sound at a certain point
    /// </summary>
    /// <param name="s">Combat sound</param>
    public void MushroomSounds(mushroomEffects s)
    {
        AudioSource.PlayClipAtPoint(mushroom[(int)s], Vector3.zero);
    }
    /// <summary>
    /// Plays a combat sound at a certain point
    /// </summary>
    /// <param name="s">Combat sound</param>
    public void GoblinSounds(goblinEffects s)
    {
        AudioSource.PlayClipAtPoint(goblin[(int)s], Vector3.zero);
    }

    public void WalkingSounds(walk s)
    {
        AudioSource.PlayClipAtPoint(walk[(int)s], Vector3.zero);
    }

    /// <summary>
    /// Plays a miscelanious sound at a certain point
    /// </summary>
    /// <param name="s">Miscelanious sound</param>
    public void miscSounds(miscEffects s)
    {
        AudioSource.PlayClipAtPoint(misc[(int)s], Vector3.zero);
    }

    /// <summary>
    /// Plays different music on the games dedicated audio source
    /// </summary>
    /// <param name="s">What clip are we playing</param>
    /// <param name="playing">Is the clip playing</param>
    /// <param name="volume">What is the audio sources volume</param>
    public void musicSounds(music s, bool playing, float volume)
    {
        musicPlayer.clip = music[(int)s];
        Debug.Log("current s: " + s.ToString() + ", music player: " + musicPlayer.name);
        if (playing && s.ToString() == musicPlayer.name)
        {
            musicPlayer.Play();
            musicPlayer.volume = volume;
        }
    }
    /// <summary>
    /// Pauses the music
    /// </summary>
    /// <param name="paused">is thre music paused or not?</param>
    public void musicPause(bool paused)
    {
        if(paused)
        {
            musicPlayer.Pause();
        }
        else
        {
            musicPlayer.UnPause();
        }
    }
}
