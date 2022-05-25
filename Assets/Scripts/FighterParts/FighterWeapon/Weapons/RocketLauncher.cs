using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;
using UnityEngine.InputSystem;

public class RocketLauncher : FighterWeapon, IWeapon
{
    [SerializeField] private Rocket rocket;
    [SerializeField] private GameObject launcherTip;
    [SerializeField] private float cooldown;

    private float nextUseTime;

    public override void ActivateWeapon(InputAction.CallbackContext context)
    { 
        if(Time.time >= nextUseTime && context.action.WasPerformedThisFrame())      
        {
            nextUseTime = Time.time + cooldown;
            Rocket rocketClone = Instantiate(rocket, launcherTip.transform.position + launcherTip.transform.forward / 3, launcherTip.transform.rotation);
            rocketClone.SetVariables(damage, fighterRoot);
            fighterRigidBody.velocity += -transform.right * Mathf.Abs(Physics.gravity.y) * 5;
            if (fighterRoot) fighterRoot.onAttack();
            OnAttack.Invoke();
        }
    }
}
