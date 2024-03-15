using UnityEngine.Audio;
using System;
using UnityEngine;


//INSTRUCTIONS: 
//1) Make a prefab of the AudioManager object.
//2) Place de AudioManager Object in each scene.
//3) Place this line of code in any other script you want to play a sound with AudioManager:

//             FindObjectOfType<AudioManager>().Play("NameOfTheAudioFile");

//  And this if you want to play the sound from this script:

//             Play("NameOfTheAudioFile");



public class AudioManager : MonoBehaviour
{
    [SerializeField] Sound[] sounds;

    public static AudioManager instance;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else 
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        foreach (Sound s in sounds) 
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;

        }
        
    }

    private void Start()
    {
        //TASK: Add an if statement and play titlescreen sound if in menu
        //stop sound after moving to a new scene and play new sound in the new scene. 
        Play("TitleScreen");
    }

    public void Play(string name) 
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        if (s == null) 
        {
            Debug.LogWarning("Sound: " + name + " not found!. Check for typos.");
            return;
        }
            

        s.source.Play();
    }

    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!. Check for typos.");
            return;
        }

        s.source.Stop();
    }


}
