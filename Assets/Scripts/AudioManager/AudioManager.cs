using UnityEngine.Audio;
using UnityEngine;
using System;
/// <summary>
/// Taken from an old project so this might be kinda scuffed but tbh looks stolen from a brackeys video
/// </summary>
public class AudioManager : MonoBehaviour
{
   
    public Sounds[] sounds;
    public static AudioManager instance;

    private void Awake()
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


        foreach (Sounds s in sounds)
        {
           s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;

        }
    
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        FindObjectOfType<AudioManager>().Play("music");
    }

    public void Play(string name)
    {
        Sounds s = Array.Find(sounds, sounds => sounds.name == name);
       
        if (s == null)
        {
            Debug.LogWarning("invalidSound");
            return;
        }

        s.source.Play();

    }

    public void Stop(string name)
    {
        Sounds s = Array.Find(sounds, sounds => sounds.name == name);

        if (s == null)
        {
            Debug.LogWarning("invalidSound");
            return;
        }

        s.source.Stop();
    }

    public void ChangePitch(string name, float pitch)
    {
        Sounds s = Array.Find(sounds, sounds => sounds.name == name);

        if (s == null)
        {
            Debug.LogWarning("invalidSound");
            return;
        }

        s.source.pitch = pitch;
    }

}
