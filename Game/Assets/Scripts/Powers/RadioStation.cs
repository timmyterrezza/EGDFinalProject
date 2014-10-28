﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]

public class RadioStation : MonoBehaviour
{
    public float frequency;
    public float signalStrength {
        get {
            // Lerp signalStrength from 0 to 1 within frequencyFadeLimit.
            // When outside of frequencyFadeLimit from the current frequency, clamp to 0.
            float frequencyDifference = Mathf.Abs(radioControl.currentFrequency - frequency);
            float strength = 1.0f - frequencyDifference / radioControl.frequencyFadeLimit;
            strength = Mathf.Clamp(strength, 0.0f, 1.0f);
            return strength;
        }
    }
    public Powerup powerup;

    [HideInInspector] public RadioControl radioControl;
    [HideInInspector] public AudioSource audioSource;

    void Awake() {
        audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.volume = 0;
    }

    void Start() {
        if (!radioControl) {
            Debug.LogWarning("There is no RadioControl linked to this RadioStation!");
        }
    }

    void Update() {
        audioSource.volume = signalStrength;

        // End if the station signal is no longer strong enough.
        if (powerup && powerup.inUse && signalStrength < radioControl.powerupMinSignalStrength) {
            powerup.EndPowerup();
        }
    }

    public void UsePowerup() {
        if (powerup) {
            powerup.TryToUsePowerup();
        } else {
            Debug.LogWarning("Found no powerup for " + this.GetPath());
        }
    }
}