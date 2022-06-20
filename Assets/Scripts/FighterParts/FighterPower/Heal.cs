using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heal : FighterPower
{
    [SerializeField] int healTime;
    [SerializeField] int healAmountPerSecond;

    [SerializeField] ParticleSystem healParticles;

    public override void Activate()
    {
        StartCoroutine(HealFighter());
        OnTrigger.Invoke();
        healParticles.Play();
    }

    public IEnumerator HealFighter()
    {
        yield return new WaitForSeconds(1);
        float startTime = Time.time;
        while(Time.time < startTime + healTime)
        {
            fighterRoot.TakeDamage(-healAmountPerSecond, fighterRoot, false, false);
            Debug.Log("HEAL " + fighterRoot.GetCurrentHealth());
            yield return new WaitForSeconds(1);
        }
        yield return new WaitForSeconds(1);
        healParticles.Stop();
    }
}
