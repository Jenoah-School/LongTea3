using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterPower : MonoBehaviour
{
    protected Fighter fighterRoot;
    public float cooldown;

    public void SetFighterRoot(Fighter fighter)
    {
        if(!fighterRoot) fighterRoot = fighter;
    }

    public virtual void Activate()
    {

    }
}
