using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(AudioSource))]
public class FadeAudio : MonoBehaviour
{
    private AudioSource audioSource;
    private float maxVolume = 1f;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if(audioSource) maxVolume = audioSource.volume;
    }

    public void FadeIn(float fadeTime)
    {
        audioSource?.DOFade(maxVolume, fadeTime);
    }

    public void FadeOut(float fadeTime)
    {
        audioSource?.DOFade(0, fadeTime);
    }
}
