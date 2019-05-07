using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;

public enum combatEffects { yeet }
public enum miscEffects { }
public enum music { }

public class AudioLibrary : MonoBehaviour
{

    AudioClip[] combat;
    AudioClip[] misc;
    AudioClip[] music;

    public AudioSource musicPlayer;

    /// <summary>
    /// Plays a combat sound at a certain point
    /// </summary>
    /// <param name="s">Combat sound</param>
    public void CombatSounds(combatEffects s)
    {
        AudioSource.PlayClipAtPoint(combat[(int)s], Vector3.zero);
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
