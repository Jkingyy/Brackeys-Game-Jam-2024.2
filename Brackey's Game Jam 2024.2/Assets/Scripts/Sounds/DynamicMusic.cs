using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;

using UnityEngine;
using UnityEngine.Audio;

public class DynamicMusicSync : MonoBehaviour
{
    public AudioSource drumAndBassSource;  // AudioSource for drums and bass
    public AudioSource leadSynthSource;    // AudioSource for lead synth
    public AudioMixer audioMixer;          // AudioMixer for filters
    public Transform player;               // Reference to the player

    public float minSpeed = 0f;            // Minimum player speed for filter
    public float maxSpeed = 10f;           // Maximum player speed for filter
    public float minCutoff = 200f;         // Minimum cutoff frequency (muffled)
    public float maxCutoff = 22000f;       // Maximum cutoff frequency (clear)
    public float increaseRate = 500f;      // Rate at which the cutoff frequency increases
    public float decreaseRate = 2000f;     // Rate at which the cutoff frequency decreases

    private Vector3 lastPosition;          // To store player's previous position
    private float currentCutoff;           // To store the current cutoff frequency

    void Start()
    {
        // Store player's starting position
        lastPosition = player.position;
        
        // Initialize current cutoff to minimum
        currentCutoff = minCutoff;
    }

    void Update()
    {
        // Calculate player's speed
        float speed = (player.position - lastPosition).magnitude / Time.deltaTime;
        lastPosition = player.position;

        // Normalize the speed and calculate the target cutoff frequency
        float normalizedSpeed = Mathf.InverseLerp(minSpeed, maxSpeed, speed);
        float targetCutoff = Mathf.Lerp(minCutoff, maxCutoff, normalizedSpeed);

        // Adjust the currentCutoff towards the targetCutoff at different rates
        if (currentCutoff < targetCutoff)
        {
            currentCutoff += increaseRate * Time.deltaTime;
            if (currentCutoff > targetCutoff) // Ensure it doesn't overshoot
            {
                currentCutoff = targetCutoff;
            }
        }
        else if (currentCutoff > targetCutoff)
        {
            currentCutoff -= decreaseRate * Time.deltaTime;
            if (currentCutoff < targetCutoff) // Ensure it doesn't overshoot
            {
                currentCutoff = targetCutoff;
            }
        }

        // Set the smoothed cutoff frequency in the audio mixer
        audioMixer.SetFloat("LeadSynthCutoff", currentCutoff);
    }
}