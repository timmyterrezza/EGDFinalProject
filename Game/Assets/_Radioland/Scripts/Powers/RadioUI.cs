﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class RadioUI : MonoBehaviour
{
    [SerializeField] private Slider radioSlider;
    [SerializeField] private RectTransform radioKnob;
    [SerializeField] private Image radioEnergyGlowImage;
    [SerializeField] private List<GameObject> energyThresholdObjects;
    [SerializeField] private List<float> energyThresholdLevels;
    [SerializeField] private bool fadeOut = false;
    [SerializeField] private float lingerTime = 3.0f; // Full strength after inactivity time.
    [SerializeField] private float fadeTime = 1.0f; // Fade after linger over this time.

    [SerializeField] private bool invert = false;

    private Image[] images;
    private float currentAlpha;
    private float currentGlowAlpha;

    private float lastActiveTime;
    private float lastDecreaseAlpha;
    private float lastIncreaseAlpha;

    private void Awake() {
        images = gameObject.GetComponentsInChildren<Image>();

        currentAlpha = 1.0f;

        lastActiveTime = -1000.0f;

        if (energyThresholdObjects.Count != energyThresholdLevels.Count) {
            Debug.LogWarning("Length mismatch between energy threshold objects and levels on " +
                             this.GetPath() + ".");
        }
    }

    private void Start() {

    }

    private void Update() {
        if (fadeOut) {
            float alpha;
            if (Time.time - lastActiveTime < lingerTime) {
                // Fade in.
                float tStart = (Time.time - lastActiveTime) / fadeTime;
                alpha = Mathf.Lerp(lastDecreaseAlpha, 1.0f, Mathf.Clamp01(tStart));
                lastIncreaseAlpha = alpha;
            } else {
                // Fade out.
                float tEnd = (Time.time - lingerTime - lastActiveTime) / fadeTime;
                alpha = Mathf.Lerp(lastIncreaseAlpha, 0.0f, Mathf.Clamp01(tEnd));
                lastDecreaseAlpha = alpha;
            }
            SetAlpha(alpha);
        }
    }

    public void TriggerActivity() {
        if (Time.time - lastActiveTime > lingerTime) {
            lastActiveTime = Time.time;
        }
    }

    public void SetSliderValue(float newValue) {
        if (radioSlider) {
            radioSlider.value = (invert ? 1 - newValue : newValue);
        }
    }

    public void SetKnobRotation(float rotationDegrees) {
        if (radioKnob) {
            radioKnob.localRotation = Quaternion.Euler(0, 0, rotationDegrees * (invert ? -1 : 1));
        }
    }

    public void SetEnergy(float energy) {
        SetGlowAlpha(energy);

        int objectCount = Mathf.Min(energyThresholdObjects.Count, energyThresholdLevels.Count);
        for (int i = 0; i < objectCount; i++) {
            if (energy < energyThresholdLevels[i]) {
                energyThresholdObjects[i].SetActive(false);
            } else {
                energyThresholdObjects[i].SetActive(true);
            }
        }
    }

    public void SetGlowAlpha(float newAlpha) {
        currentGlowAlpha = newAlpha;

        if (radioEnergyGlowImage) {
            Color color = radioEnergyGlowImage.color;
            color.a = newAlpha * currentAlpha;
            radioEnergyGlowImage.color = color;
        }
    }

    public void SetAlpha(float alpha) {
        currentAlpha = alpha;

        foreach (Image image in images) {
            Color color = image.color;
            color.a = alpha;
            image.color = color;
        }

        SetGlowAlpha(currentGlowAlpha);
    }
}
