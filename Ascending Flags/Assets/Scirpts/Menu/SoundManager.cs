using System.Collections;
using System.Collections.Generic;
using Mirror.Examples.Basic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public Sound[] musicSound, SFXSound;
    public AudioSource musicSource, SFXSource;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PlayMusic("BGMusic");
    }
    public void PlayMusic(string name)
    {
        Sound s = Array.Find(musicSound, x => x.name == name);

        if (s == null)
        {
            print("Music was not found");
        }
        else
        {
            musicSource.clip = s.clip;
            musicSource.Play();
        }
    }

    public void PlaySoundEffect(string name)
    {
        Sound s = Array.Find(SFXSound, x => x.name == name);

        if (s == null)
        {
            print("SFX was not found");
        }
        else
        {
            SFXSource.PlayOneShot(s.clip);
        }
    }

    public void MusicVolume(float volume)
    {
        musicSource.volume = volume;
    }
    public void SFXVolume(float volume)
    {
        SFXSource.volume = volume;
    }
}