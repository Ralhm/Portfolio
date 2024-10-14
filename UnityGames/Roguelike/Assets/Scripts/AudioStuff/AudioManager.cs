using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Quick and dirty audio manager
public class AudioManager : MonoBehaviour
{
    public List<Sound> sounds;


    private static AudioManager instance;
    public static AudioManager Instance { get { return instance; } }


    public AudioSource SpellSource;

    public void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }


    public void PlaySpellSound(AudioClip sfx)
    {

        SpellSource.PlayOneShot(sfx);
    }

    public void PlaySound(string name)
    {

        //Sound s = List.Find(sounds, sound => sound.name == name);
        Sound s = null;
        for (int i = 0;i < sounds.Count;i++)
        {
            if (sounds[i].name == name)
            {
                s = sounds[i];
            }
        }


        if (s == null)
        {
            Debug.Log("Sound Not Found");
            return;
        }
        s.source.Play();
    }

}
