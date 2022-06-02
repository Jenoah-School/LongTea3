using Lean.Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DamageOverTime : MonoBehaviour
{
    [SerializeField] private int damageAmount = 10;
    [SerializeField] private float cooldown = 0.5f;
    [SerializeField] private UnityEvent OnHit;
    [SerializeField] private List<Transform> hitThisTick = new List<Transform>();
    [SerializeField] private ParticleSystem particlesToSpawn;
    public bool canDamage = true;

    private float nextDamageTime = 0f;

    private void OnTriggerStay(Collider other)
    {
        if (!canDamage) return;
        if (Time.time > nextDamageTime)
        {
            hitThisTick.Clear();

            if (other.transform.GetComponentInParent<Fighter>())
            {
                Fighter otherFighter = other.transform.GetComponentInParent<Fighter>();

                if (!hitThisTick.Contains(otherFighter.transform))
                {
                    hitThisTick.Add(other.gameObject.transform.root);
                    FighterPart fighterPart = other.gameObject.GetComponentInParent<FighterPart>();
                    if (fighterPart != null)
                    {
                        otherFighter.TakeDamage(damageAmount);
                        OnHit.Invoke();
                        if (particlesToSpawn != null) LeanPool.Spawn(particlesToSpawn, other.ClosestPointOnBounds(transform.position), Quaternion.LookRotation((other.transform.position - transform.position).normalized));
                        nextDamageTime = Time.time + cooldown;
                    }
                }
            }
        }
    }
}
