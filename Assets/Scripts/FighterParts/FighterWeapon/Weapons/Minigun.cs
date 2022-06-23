using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Minigun : FighterWeapon, IWeapon
{
    [SerializeField] float range = 20f;
    [SerializeField] float shootInterval = 0.1f;
    [SerializeField] private bool vibrateController = true;

    [Header("Aim Assist")]
    [SerializeField] float aimAssistSpeed = 20;
    [SerializeField] float aimAssistAngle = 25;

    [SerializeField] GameObject barrelStart;
    [SerializeField] GameObject barrelTip;
    [SerializeField] ParticleSystem bulletShellsPS;

    bool isShooting;
    float nextShootTime;

    AimAssist aimAssist = new AimAssist();

    public override void ActivateWeapon(InputAction.CallbackContext context)
    {
        if (context.action.WasPerformedThisFrame())
        {
            isShooting = true;
        }
        else
        {
            isShooting = false;
        }

    }
    private void Update()
    {
        if (isShooting)
        {
            Shooting();
            barrelStart.transform.Rotate(0, 0, 1000 * Time.deltaTime);
        }
        else
        {
            //aimAssist.ResetAim(transform, aimAssistSpeed);
            bulletShellsPS.Stop();
        }

        aimAssist.StartAimAssist(transform, fighterRoot, aimAssistSpeed, range, aimAssistAngle);

        Debug.DrawLine(barrelTip.transform.position, barrelTip.transform.position + barrelTip.transform.forward * range, Color.blue);
    }

    private void Shooting()
    {
        if (Time.time >= nextShootTime)
        {
            if (!bulletShellsPS.isPlaying) bulletShellsPS.Play();
            nextShootTime = Time.time + shootInterval;
            CheckIfHit();
            OnAttack.Invoke();
            if (vibrateController && fighterRoot.controllerHaptics)
            {
                fighterRoot.controllerHaptics.QuickHaptic();
            }
        }       
    }

    private void CheckIfHit()
    {
        RaycastHit hit;
        if (Physics.Raycast(barrelTip.transform.position, barrelTip.transform.forward * range, out hit))
        {
            if (hit.transform.GetComponentInParent<Fighter>())
            {
                Fighter otherFighter = hit.transform.GetComponentInParent<Fighter>();
                otherFighter.TakeDamage(damage, fighterRoot, true, true);
            }
        }
    }
}
