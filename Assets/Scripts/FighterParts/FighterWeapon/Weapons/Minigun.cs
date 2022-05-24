using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Minigun : FighterWeapon, IWeapon
{
    [SerializeField] float shootInterval = 0.1f;

    [SerializeField] GameObject barrelStart;
    [SerializeField] GameObject barrelTip;
    [SerializeField] ParticleSystem bulletShellsPS;

    bool isShooting;
    float nextShootTime;

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

    public override void CheckCollision()
    {
        base.CheckCollision();
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
            bulletShellsPS.Stop();
        }
    }

    private void Shooting()
    {
        if (Time.time >= nextShootTime)
        {
            if (!bulletShellsPS.isPlaying) bulletShellsPS.Play();
            nextShootTime = Time.time + shootInterval;            
        }
    }

    private void AimAssist()
    {

    }
}
