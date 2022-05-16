using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;

public class RocketLauncher : FighterWeapon, IWeapon
{
    [SerializeField] private Rocket rocket;
    [SerializeField] private GameObject launcherTip;  
    
    public override void ActivateWeapon()
    {
        Rocket rocketClone = Instantiate(rocket, launcherTip.transform.position + launcherTip.transform.forward / 3, launcherTip.transform.rotation);      
        rocketClone.SetVariables(damage, transform.root.gameObject);
    }
}
