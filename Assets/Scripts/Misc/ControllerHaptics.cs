using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerHaptics : MonoBehaviour
{
    [SerializeField] private PlayerInput referencePlayerInput;
    [SerializeField] private Gamepad playerGamepad;
    [SerializeField] private Fighter fighterReference;

    [Header("Tap haptics")]
    [SerializeField, Range(0f, 1f)] private float tapLowFrequency = 0.2f;
    [SerializeField, Range(0f, 1f)] private float tapHighFrequencyIntensity = 0.6f;
    [SerializeField] private float tapHapticDuration = 0.05f;

    [Header("Quick haptics")]
    [SerializeField, Range(0f, 1f)] private float quickLowFrequency = 0.2f;
    [SerializeField, Range(0f, 1f)] private float quickHighFrequencyIntensity = 0.2f;
    [SerializeField] private float quickHapticDuration = 0.1f;

    [Header("Medium haptics")]
    [SerializeField, Range(0f, 1f)] private float mediumLowFrequencyIntensity = 0.4f;
    [SerializeField, Range(0f, 1f)] private float mediumHighFrequencyIntensity = 0.3f;
    [SerializeField] private float mediumHapticDuration = 0.1f;

    private float vibrationTimeLeft = 0f;
    private bool isVibrating = false;

    #region Starting and ending

    private void Start()
    {
        if(referencePlayerInput) LinkPlayerInput(referencePlayerInput);

        if (fighterReference)
        {
            fighterReference.onAttack += QuickHaptic;
            fighterReference.onTakeDamage += MediumHaptic;
        }
    }

    public void LinkPlayerInput(PlayerInput playerInput)
    {
        referencePlayerInput = playerInput;
        if (referencePlayerInput.currentControlScheme == "Gamepad")
        {
            playerGamepad = referencePlayerInput.GetDevice<Gamepad>();
        }
    }

    private void OnDisable()
    {
        if (fighterReference)
        {
            fighterReference.onAttack -= QuickHaptic;
            fighterReference.onTakeDamage -= MediumHaptic;
        }
        if (playerGamepad != null) playerGamepad.ResetHaptics();
    }

    #endregion

    #region Haptics

    [ContextMenu("Trigger Tap haptics")]
    public void TapHaptic()
    {
        if (playerGamepad != null)
        {
            playerGamepad.SetMotorSpeeds(tapLowFrequency, tapHighFrequencyIntensity);
            playerGamepad.ResumeHaptics();
            vibrationTimeLeft = tapHapticDuration;
            isVibrating = true;
        }
    }

    [ContextMenu("Trigger Quick haptics")]
    public void QuickHaptic()
    {
        if (playerGamepad != null)
        {
            playerGamepad.SetMotorSpeeds(quickLowFrequency, quickHighFrequencyIntensity);
            playerGamepad.ResumeHaptics();
            vibrationTimeLeft = quickHapticDuration;
            isVibrating = true;
        }
    }

    [ContextMenu("Trigger Medium haptics")]
    public void MediumHaptic()
    {
        if (playerGamepad != null)
        {
            playerGamepad.SetMotorSpeeds(mediumLowFrequencyIntensity, mediumHapticDuration);
            playerGamepad.ResumeHaptics();
            vibrationTimeLeft = mediumHapticDuration;
            isVibrating = true;
        }
    }

    #endregion

    private void Update()
    {
        if (isVibrating)
        {
            if (vibrationTimeLeft > 0)
            {
                vibrationTimeLeft -= Time.deltaTime;
            }
            else
            {
                playerGamepad.ResetHaptics();
                isVibrating = false;
            }
        }
    }
}
