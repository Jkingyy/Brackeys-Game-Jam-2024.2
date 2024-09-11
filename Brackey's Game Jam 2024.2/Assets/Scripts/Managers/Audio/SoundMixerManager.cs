using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
public class SoundMixerManager : MonoBehaviour
{
    [SerializeField] private AudioMixer _audioMixer;
    // Start is called before the first frame update

    private void Start() {
    }

    public void SetMasterVolume(float volume)
    {

        _audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20f);
        PlayerPrefs.SetFloat("MasterVolume", volume);
    }

    public void SetSoundFXVolume(float volume)
    {
        _audioMixer.SetFloat("SoundFXVolume", Mathf.Log10(volume) * 20f);
        PlayerPrefs.SetFloat("SoundFXVolume", volume);
    }

    public void SetMusicVolume(float volume)
    {
        _audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20f);
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

}
