using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

//===========================================================================================
// OVERVIEW 
//===========================================================================================

//LOCATION:      This script is placed at the sound manager that is parented to the camera
//USE:           It plays all the sound effect in the game. Supports 2D and 3D spacialised sound
//INFO:          This script is fine as it is but it might be replaced by Musehive's Wwise solution which will be more practical
public class SoundManager : MonoBehaviour
{
    // VARIABLES
    public static SoundManager instance;

    [Header("Sounds")]
    public Sound[] sounds;
    public SoundGroup[] soundGroups;

    [Header("Other Stuff")]
    public int maxSounds = 20;

    public AudioMixerGroup soundEffectMixer;

    //===========================================================================================
    // Awake is called the frame before start
    //===========================================================================================
    private void Awake()
    {
        //Set the instance
        instance = this;
    }

    //===========================================================================================
    // The next methods here are all for playing sound effects. Each one takes different parameters
    //===========================================================================================

    //2D sound with randomized pitch. Only takes one argument: sound name
    public static void PlaySound(string name)
    {
        PlaySound(name, Vector3.zero, Random.Range(0.8f, 1.2f), false, 0);
    }

    //2D sound. Only takes two arguments: sound name and pitch
    public static void PlaySound(string name, float pitch)
    {
        PlaySound(name, Vector3.zero, pitch, false, 0);
    }

    //3D sound with randomized pitch. Only takes three argument: sound name, sound position and sounds range
    public static void PlaySound(string name, Vector3 position, float range)
    {
        PlaySound(name, position, Random.Range(0.8f, 1.2f), true, range);
    }

    //Will play a sound with customizable options
    public static void PlaySound(string name, Vector3 position, float pitch, bool is3D, float range)
    {
        //If the string is null or empty then just don't play anything
        if (string.IsNullOrEmpty(name))
            return;

        //Get the sound manager and store it in a variable
        SoundManager soundManager = instance;

        //If the SoundManager doesn't exist then return
        if (soundManager == null)
            return;

        //If the limit of sounds have been reached then return
        if (GameObject.FindGameObjectsWithTag("Sound").Length >= soundManager.maxSounds)
            return;

        //Set the volume to 0
        float volume = 0;

        //Find right audio clip
        AudioClip clip = null;
        foreach (Sound s in soundManager.sounds)
        {
            //Check if the names are the same. If it is then the right sound has been found!
            if (s.name == name)
            {
                //Set the clip data
                clip = s.audio;
                volume = s.soundVolume;
                pitch += s.pitchSlider - 1;
                break;
            }
        }

        //If there was no clip found then just return
        if (clip == null)
            return;

        //Create a new game object and set it's name, tag and position
        GameObject sound = new GameObject();
        sound.transform.position = position;
        sound.name = name + " sound";
        sound.tag = "Sound";

        //Add a sound source component to the game object and set it to be the right sound options
        AudioSource audio = sound.AddComponent<AudioSource>();
        audio.volume = volume;
        audio.pitch = pitch;
        audio.clip = clip;
        audio.playOnAwake = false;
        audio.outputAudioMixerGroup = soundManager.soundEffectMixer;
        audio.spatialBlend = is3D ? 1 : 0;
        audio.minDistance = range;

        //Play the sound
        audio.Play();

        //Add a kill script to the sound to clean up and make sure there isn't any dead game objects taking up precious space
        KillObjectsAfterTime killScript = sound.AddComponent<KillObjectsAfterTime>();
        killScript.unscaledTime = true;

        //Set the time to kill to be the sounds lenght + 0,01 seconds
        killScript.lifetime = clip.length + 0.01f;
    }

    //===========================================================================================
    //  Sound groups pretty much do the same thing as regular sounds except they have a randomized sound clip
    //===========================================================================================
    public static void PlayGroupSound(string name)
    {
        PlayGroupSound(name, Vector3.zero, Random.Range(0.8f, 1.2f), false, Vector2.one);
    }

    public static void PlayGroupSound(string name, float pitch)
    {
        PlayGroupSound(name, Vector3.zero, pitch, false, Vector2.one);
    }

    public static void PlayGroupSound(string name, Vector3 position, Vector2 range)
    {
        PlayGroupSound(name, position, Random.Range(0.8f, 1.2f), true, range);
    }

    public static void PlayGroupSound(string name, Vector3 position, float pitch, bool is3D, Vector2 range)
    {
        if (string.IsNullOrEmpty(name))
            return;

        SoundManager soundManager = instance;
        if (soundManager == null)
            return;
        
        if (GameObject.FindGameObjectsWithTag("Sound").Length >= soundManager.maxSounds)
            return;

        float volume = 0;

        AudioClip clip = null;
        foreach (SoundGroup s in soundManager.soundGroups)
        {
            if (s.groupName == name)
            {
                int currentSound = Random.Range(0, s.sounds.Length);
                clip = s.sounds[currentSound];
                volume = s.soundVolume;
                pitch += s.pitchSlider - 1;
                break;
            }
        }

        if (clip == null)
            return;

        GameObject sound = new GameObject();
        sound.transform.position = position;
        sound.name = name + " sound";
        sound.tag = "Sound";

        AudioSource audio = sound.AddComponent<AudioSource>();
        audio.volume = volume;
        audio.pitch = pitch;
        audio.clip = clip;
        audio.playOnAwake = false;
        audio.outputAudioMixerGroup = soundManager.soundEffectMixer;
        audio.spatialBlend = is3D ? 1 : 0;
        audio.minDistance = Mathf.RoundToInt(range.x);
        audio.maxDistance = Mathf.RoundToInt(range.y);
        audio.Play();

        KillObjectsAfterTime killScript = sound.AddComponent<KillObjectsAfterTime>();
        killScript.unscaledTime = true;

        killScript.lifetime = clip.length + 0.01f;
    }

    //Is simply just the public version of play sound made for UI elements to reference
    public void PlayUISound(string name)
    {
        PlaySound(name);
    }
}

// The sound specific classes

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip audio;
    [Range(0, 2)]
    public float soundVolume = 1;
    [Range(0, 2)]
    public float pitchSlider = 1;
}

[System.Serializable]
public class SoundGroup
{
    public string groupName;
    public AudioClip[] sounds;
    [Range(0, 2)]
    public float soundVolume = 1;
    [Range(0, 2)]
    public float pitchSlider = 1;
}
