using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : FighterPower
{
    [SerializeField] GameObject sphere;
    [SerializeField] float duration;

    public override void Activate()
    {
        StartCoroutine(ShieldFighter());
        OnTrigger.Invoke();
    }

    IEnumerator ShieldFighter()
    {
        sphere.SetActive(true);
        fighterRoot.canDamage = false;
        yield return new WaitForSeconds(duration);
        sphere.SetActive(false);
        fighterRoot.canDamage = true;
    }
}
