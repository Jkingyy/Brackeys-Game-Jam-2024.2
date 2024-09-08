using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager Instance;
    
    [SerializeField] private AudioSource soundFXObject;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    
    public void PlaySoundFXClip(AudioClip audioClip, Transform spawntransform, float volume)
    {
       AudioSource audioSource = Instantiate(soundFXObject, spawntransform.position, Quaternion.identity);
       
       audioSource.clip = audioClip;
       
       audioSource.volume = volume;
       
       audioSource.Play();
       
       float clipLength = audioSource.clip.length;
       
       Destroy(audioSource.gameObject, clipLength);
    }
    
        
    public void PlayRandomSoundFXClip(AudioClip[] audioClip, Transform spawntransform, float volume)
    {
        int randomIndex = Random.Range(0, audioClip.Length);
        
        PlaySoundFXClip(audioClip[randomIndex], spawntransform, volume);
        
    }

}
