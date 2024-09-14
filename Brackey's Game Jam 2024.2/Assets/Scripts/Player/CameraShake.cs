using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{
    CinemachineVirtualCamera cinemachineVirtualCamera;
    CinemachineBasicMultiChannelPerlin noisePerlin;

    private void Awake() {
        cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
        noisePerlin = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public void ShakeCamera(float time){
        StartCoroutine(Shake(time));
    }


    IEnumerator Shake(float time){
        noisePerlin.m_AmplitudeGain = 3;
        noisePerlin.m_FrequencyGain = 2;
        yield return new WaitForSecondsRealtime(time);
        noisePerlin.m_AmplitudeGain = 0;
        noisePerlin.m_FrequencyGain = 0;
    }
}
