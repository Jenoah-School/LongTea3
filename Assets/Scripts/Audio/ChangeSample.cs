using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSample : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float randomSampleRange = 30f;

    public void PlayAtSample(float playbackTime)
    {
        if (audioSource.clip != null) audioSource.time = playbackTime % audioSource.clip.length;
    }

    public void PlayAtRandomTime(float range = -1f)
    {
        if (range == -1 || range == 0) range = randomSampleRange;
        PlayAtSample(Random.Range(0, range));
    }
}
