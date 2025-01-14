﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EffectManager : MonoBehaviour
{
    private Effect[] effects;

    private void Awake() {
        effects = gameObject.GetComponents<Effect>();
    }

    private void Start() {

    }

    private void Update() {

    }

    // Triggers all effects on this object.
    public void StartEvent() {
        if (!enabled) { return; }

        foreach (Effect effect in effects) {
            effect.TriggerEffect();
        }
    }

    // Prematurely end all effects (ahead of their own durations).
    // Useful for interrupting abilities, when an object dies, etc.
    public void StopEvent() {
        foreach (Effect effect in effects) {
            effect.EndEffect();
        }
    }
}
