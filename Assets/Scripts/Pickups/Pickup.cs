using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Pickup : MonoBehaviour
{
    [Header("Trigger settings")]
    [SerializeField] protected bool singleUse = true;
    [SerializeField] private bool destroySelfAfterTrigger = true;
    [SerializeField] private float destroyDelay = 0f;

    [Header("Events")]
    [SerializeField] protected UnityEvent OnTrigger;
    [SerializeField] protected UnityEvent OnSpawn;

    protected Fighter triggeredFighter;
    protected bool canTrigger = true;

    public virtual void Awake()
    {
        OnSpawn.Invoke();
    }

    public virtual void Activate()
    {
        OnTrigger.Invoke();
        Debug.Log($"Pickup triggered by {triggeredFighter.name}");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (canTrigger)
        {
            triggeredFighter = other.GetComponentInParent<Fighter>();
            if (triggeredFighter != null && !triggeredFighter.isDead)
            {
                Activate();
                if (singleUse) canTrigger = false;
                if (destroySelfAfterTrigger) Destroy(gameObject, destroyDelay);
            }
        }
    }
}
