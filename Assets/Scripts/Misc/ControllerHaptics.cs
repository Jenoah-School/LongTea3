using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerHaptics : MonoBehaviour
{
    [SerializeField] private PlayerInput referencePlayerInput;
    [SerializeField] private Gamepad playerGamepad;
    [SerializeField] private Fighter fighterReference;

    [Header("Small haptics")]
    [SerializeField] private float quickSmallMotorIntensity = 0.2f;
    [SerializeField] private float quickLargeMotorIntensity = 0.2f;
    [SerializeField] private float quickHapticDuration = 0.1f;

    [Header("Medium haptics")]
    [SerializeField] private float mediumSmallMotorIntensity = 0.4f;
    [SerializeField] private float mediumLargeMotorIntensity = 0.3f;
    [SerializeField] private float mediumHapticDuration = 0.1f;

    private float vibrationTimeLeft = 0f;
    private bool isVibrating = false;

    private void Awake()
    {
        if (referencePlayerInput.currentControlScheme == "Gamepad")
        {
            playerGamepad = referencePlayerInput.GetDevice<Gamepad>();
        }

        fighterReference.onAttack += QuickHaptic;
        fighterReference.onTakeDamage += MediumHaptic;
    }

    private void OnDisable()
    {
        fighterReference.onAttack -= QuickHaptic;
        fighterReference.onTakeDamage -= MediumHaptic;
    }

    public void QuickHaptic()
    {
        if (playerGamepad != null)
        {
            playerGamepad.SetMotorSpeeds(quickSmallMotorIntensity, quickLargeMotorIntensity);
            playerGamepad.ResumeHaptics();
            vibrationTimeLeft += quickHapticDuration;
            isVibrating = true;
        }
    }

    public void MediumHaptic()
    {
        if (playerGamepad != null)
        {
            playerGamepad.SetMotorSpeeds(mediumSmallMotorIntensity, mediumLargeMotorIntensity);
            playerGamepad.ResumeHaptics();
            vibrationTimeLeft += mediumHapticDuration;
            isVibrating = true;
        }
    }

    private void Update()
    {
        if (isVibrating) {
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