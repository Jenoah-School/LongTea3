using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FirePit : ToggleOverTime
{
    [Header("References")]
    [SerializeField] private ParticleSystem fireParticle;
    [SerializeField] private DamageOverTime damageOverTime;

    [Header("Audio")]
    [SerializeField] private AudioSource fireAudioSource;
    [SerializeField, Range(0f, 1f)] private float targetFireVolume = 1f;
    [SerializeField] private float audioFadeDuration = 0.5f;

    public override void StartFire()
    {
        base.StartFire();
        fireParticle.Play();
        damageOverTime.canDamage = true;
        fireAudioSource.DOFade(targetFireVolume, audioFadeDuration);
    }

    public override void StopFire()
    {
        base.StopFire();
        fireParticle.Stop();
        damageOverTime.canDamage = false;
        fireAudioSource.DOFade(0f, audioFadeDuration);
    }
}
