using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FirePit : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ParticleSystem fireParticle;
    [SerializeField] private DamageOverTime damageOverTime;

    [Header("Time")]
    [SerializeField] private float offTime = 30f;
    [SerializeField] private float onTime = 10f;

    [Header("Audio")]
    [SerializeField] private AudioSource fireAudioSource;
    [SerializeField, Range(0f, 1f)] private float targetFireVolume = 1f;
    [SerializeField] private float audioFadeDuration = 0.5f;

    private bool canTrigger = true;
    private bool isFiring = true;
    private float nextInitTime = 0f;

    private void Start()
    {
        StopFire();
    }

    public void StartFire()
    {
        fireParticle.Play();
        damageOverTime.canDamage = true;
        fireAudioSource.DOFade(targetFireVolume, audioFadeDuration);
    }

    private void Update()
    {
        if (!canTrigger) return;

        if(Time.time > nextInitTime)
        {
            if (!isFiring)
            {
                isFiring = true;
                nextInitTime = Time.time + onTime;
                StartFire();
            }
            else
            {
                isFiring = false;
                nextInitTime = Time.time + offTime;
                StopFire();
            }
        }
    }

    public void StopFire()
    {
        fireParticle.Stop();
        damageOverTime.canDamage = false;
        fireAudioSource.DOFade(0f, audioFadeDuration);
    }
}
