using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelPauseMenu : MonoBehaviour
{

    const string MAIN_MENU_SCENE = "MainMenu";

    [SerializeField] GameObject _pauseMenu;
    [SerializeField] GameObject _settingsMenu;

    [SerializeField] Slider _masterVolumeSlider;
    [SerializeField] Slider _soundFXVolumeSlider;
    [SerializeField] Slider _musicVolumeSlider;
    bool isPaused = false;

    // Start is called before the first frame update
    void Start()
    {
        _masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume");
        _soundFXVolumeSlider.value = PlayerPrefs.GetFloat("SoundFXVolume");
        _musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume");
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)){
            if(isPaused){
                ResumeGame();
            } else{
                PauseGame();
            }
        }
    }
    void PauseGame(){
        Time.timeScale = 0;
        isPaused = true;
        _pauseMenu.SetActive(true);
    }

    void UnpauseGame(){
        Time.timeScale = 1;
        isPaused = false;
    }

    public void ResumeGame(){
        UnpauseGame();
        CloseAllMenus();
    }
    void CloseAllMenus(){
        _pauseMenu.SetActive(false);
        _settingsMenu.SetActive(false);
    }
    public void ReturnToMenu(){
        ResumeGame();
        SceneManager.LoadScene(MAIN_MENU_SCENE);
    }

    public void RestartLevel(){
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
