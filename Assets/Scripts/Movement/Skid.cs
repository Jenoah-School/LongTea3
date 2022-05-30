using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skid : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody movementRigidbody = null;
    [SerializeField] private PlayerMovement playerMovement = null;

    [Header("Skid settings")]
    [SerializeField, Range(0, 180)] private float maxAngleDifference = 5f;
    [SerializeField] private float minimumMoveSpeed = 1f;
    [SerializeField] private bool isSkidding = false;

    [Header("Audio")]
    [SerializeField] private AudioSource skidAudioSource = null;
    [SerializeField] private float pitchFluctiation = 0.2f;
    [SerializeField] private float skidAudioFadeTime = 0.3f;
    private float skidAudioSourceMaxVolume = 1f;

    [SerializeField] private ParticleSystem[] skidParticleSystems = null;
    [SerializeField] private TrailRenderer[] skidTrailRenderers = null;

    private Vector3 moveVelocity = Vector3.zero;
    // Start is called before the first frame update

    void Start()
    {
        if (movementRigidbody == null || playerMovement == null) enabled = false;
        skidAudioSourceMaxVolume = skidAudioSource.volume;
        skidAudioSource.volume = 0f;
        foreach (ParticleSystem particleSystem in skidParticleSystems)
        {
            particleSystem.Stop();
        }
        foreach (TrailRenderer trailRenderer in skidTrailRenderers)
        {
            trailRenderer.emitting = false;
        }
    }

    private void Update()
    {
        if (isSkidding)
        {
            PitchAudio();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        moveVelocity = movementRigidbody.velocity;
        if (moveVelocity.magnitude < minimumMoveSpeed || !playerMovement.IsGrounded())
        {
            if (isSkidding)
            {
                isSkidding = false;
                foreach (ParticleSystem particleSystem in skidParticleSystems)
                {
                    particleSystem.Stop();
                }
                foreach (TrailRenderer trailRenderer in skidTrailRenderers)
                {
                    trailRenderer.emitting = false;
                }
                StopAllCoroutines();
                StartCoroutine(StopSkidSound());
            }

            return;
        }

        float forwardDot = Vector3.Dot(transform.forward, moveVelocity.normalized);
        if (Mathf.Acos(forwardDot) * Mathf.Rad2Deg > maxAngleDifference)
        {
            if (!isSkidding)
            {
                isSkidding = true;
                foreach (ParticleSystem particleSystem in skidParticleSystems)
                {
                    particleSystem.Play();
                }
                foreach (TrailRenderer trailRenderer in skidTrailRenderers)
                {
                    trailRenderer.emitting = true;
                }
                StopAllCoroutines();
                StartCoroutine(StartSkidSound());
            }
        }
        else
        {
            if (isSkidding)
            {
                isSkidding = false;
                foreach (ParticleSystem particleSystem in skidParticleSystems)
                {
                    particleSystem.Stop();
                }
                foreach (TrailRenderer trailRenderer in skidTrailRenderers)
                {
                    trailRenderer.emitting = false;
                }
                StopAllCoroutines();
                StartCoroutine(StopSkidSound());
            }
        }
    }

    void PitchAudio()
    {
        skidAudioSource.pitch = 1f + (Mathf.PerlinNoise(123, Time.time) * 2f - 1f) * pitchFluctiation;
    }

    IEnumerator StopSkidSound()
    {
        float elapsedTime = 0f;
        float currentVolume = skidAudioSource.volume;

        while(elapsedTime < skidAudioFadeTime)
        {
            elapsedTime += Time.deltaTime;
            skidAudioSource.volume = Mathf.SmoothStep(currentVolume, 0f, elapsedTime / skidAudioFadeTime);
            yield return new WaitForEndOfFrame();
        }

        skidAudioSource.volume = 0f;
    }

    IEnumerator StartSkidSound()
    {
        float elapsedTime = 0f;
        float currentVolume = skidAudioSource.volume;

        while (elapsedTime < skidAudioFadeTime)
        {
            elapsedTime += Time.deltaTime;
            skidAudioSource.volume = Mathf.SmoothStep(currentVolume, skidAudioSourceMaxVolume, elapsedTime / skidAudioFadeTime);
            yield return new WaitForEndOfFrame();
        }

        skidAudioSource.volume = skidAudioSourceMaxVolume;
    }
}
