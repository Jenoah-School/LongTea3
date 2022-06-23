using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FighterPower : MonoBehaviour
{
    protected Fighter fighterRoot;
    public FighterPowerupInformation fighterPowerInformation;
    public float cooldown;
    public UnityEvent OnTrigger;

    public void SetFighterRoot(Fighter fighter)
    {
        if(!fighterRoot) fighterRoot = fighter;
    }

    public virtual void Activate()
    {
        fighterRoot.onUsePowerup();
    }
}
