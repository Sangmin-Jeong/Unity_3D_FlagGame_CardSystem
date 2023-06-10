using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIControl : MonoBehaviour
{
    public Slider MusicSlider, SoundSlider;

    void Awake()
    {
        if (!PlayerPrefs.HasKey("musicVolume"))
        {
            PlayerPrefs.SetFloat("musicVolume", 1);
            LoadMusic();
        }
        else
        {
            LoadMusic();
        }

        if (!PlayerPrefs.HasKey("sfxVolume"))
        {
            PlayerPrefs.SetFloat("sfxVolume", 1);
            LoadSFX();
        }
        else
        {
            LoadSFX();
        }
    }
    public void MusicVolume()
    {
        SoundManager.instance.MusicVolume(MusicSlider.value);
        SaveMusic();
    }
    public void SFXVolume()
    {
        SoundManager.instance.SFXVolume(SoundSlider.value);
        SaveSFX();

    }
    private void LoadMusic()
    {
        //MusicSlider.value = PlayerPrefs.GetFloat("musicVolume");
    }

    private void SaveMusic()
    {
        PlayerPrefs.SetFloat("musicVolume", MusicSlider.value);
    }

    private void LoadSFX()
    {
        //SoundSlider.value = PlayerPrefs.GetFloat("sfxVolume");

    }

    private void SaveSFX()
    {
        PlayerPrefs.SetFloat("sfxVolume", SoundSlider.value);
    }
}