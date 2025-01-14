﻿using UnityEngine;

public class ParticlesFollowSpline : MonoBehaviour
{
    [SerializeField] private ParticleSystem system;
    [SerializeField] private BezierSpline spline;

    [SerializeField] [Tooltip("Higher values interpolate to target position faster.")]
    [Range(0f, 1f)] private float smoothing = 0.2f;

    [SerializeField] [Tooltip("Ignored if the spline does not loop.")]
    private float loopTime = 1f;

    private void Reset() {
        system = gameObject.GetComponentInChildren<ParticleSystem>();
        spline = gameObject.GetComponentInChildren<BezierSpline>();

        if (system) { Debug.Log("Found " + system.GetPath() + " for " + this.GetPath()); }
        if (spline) { Debug.Log("Found " + spline.GetPath() + " for " + this.GetPath()); }
    }

    private void Awake() {
        system.simulationSpace = ParticleSystemSimulationSpace.World;

        if (spline.loop) {
            system.emissionRate = system.maxParticles / loopTime;

            // Work within a lifetime range of [loopTime, 2 * loopTime].
            // When lifetime falls below loopTime, add loopTime.

            // We could set startLifetime to be infinite then calculate elapsed
            // time and mod that againt loopTime, but that could introduce
            // numerical inaccuracies.
            system.startLifetime = 2 * loopTime;
        }
    }

    private void Start() {

    }

    private void Update() {
        if (!system.isPlaying) { return; }

        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[system.particleCount];
        int particleCount = system.GetParticles(particles);

        for (int i = 0; i < particleCount; i++) {
            ParticleSystem.Particle particle = particles[i];

            float t;
            if (spline.loop) {
                if (particle.lifetime < particle.startLifetime / 2f) {
                    particle.lifetime += particle.startLifetime / 2f;
                }
                t = (1f - (particle.lifetime / particle.startLifetime)) * 2f;
            } else {
                t = 1f - (particle.lifetime / particle.startLifetime);
            }
            particle.position = Vector3.Lerp(particle.position, spline.GetPoint(t), smoothing);

            particles[i] = particle;
        }

        system.SetParticles(particles, particleCount);
    }
}
