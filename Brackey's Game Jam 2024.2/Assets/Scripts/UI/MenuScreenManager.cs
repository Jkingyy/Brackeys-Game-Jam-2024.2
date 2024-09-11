using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuScreenManager : MonoBehaviour
{   
    const string LEVEL_1_SCENE = "SampleScene";

    [SerializeField] Slider _masterVolumeSlider;
    [SerializeField] Slider _soundFXVolumeSlider;
    [SerializeField] Slider _musicVolumeSlider;
    // Start is called before the first frame update
    void Start()
    {
        _masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume");
        _soundFXVolumeSlider.value = PlayerPrefs.GetFloat("SoundFXVolume");
        _musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume");
    }

    // Update is called once per frame


    public void StartGame(){
        SceneManager.LoadScene(LEVEL_1_SCENE);
    }


}