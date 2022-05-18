using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangePitch : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    [Header("Random pitch")]
    [SerializeField] private bool changePitchOnEnable = false;
    [SerializeField] private float defaultRandomPitchValue = 0.1f;

    private void OnEnable()
    {
        if (changePitchOnEnable) SetRandomPitch();
    }

    public void SetPitch(float audioPitch)
    {
        if (audioSource) audioSource.pitch = audioPitch;
    }

    public void SetRandomPitch(float range = -1)
    {
        if (range == -1) range = defaultRandomPitchValue;
        float randPitch = Random.Range(1f - range, 1f + range);
        SetPitch(randPitch);
    }
}
