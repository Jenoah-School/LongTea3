using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ToggleOverTime : MonoBehaviour
{
    [Header("Time")]
    [SerializeField] protected float offTime = 30f;
    [SerializeField] protected float onTime = 10f;
    [SerializeField] protected Vector2 startDelay = new Vector2(0f, 0f);
    [Space(10)]
    [SerializeField] public bool isOnTimer = true;

    [Header("Toggle events")]
    [SerializeField] protected UnityEvent onTurnOn;
    [SerializeField] protected UnityEvent onTurnOff;

    protected bool canTrigger = true;
    protected bool isFiring = true;
    protected float nextInitTime = 0f;

    protected virtual void Start()
    {
        Random.InitState(Mathf.RoundToInt(transform.position.x + transform.position.y + transform.position.z));
        StopFire();
        nextInitTime = Time.time + Random.Range(startDelay.x, startDelay.y);
    }

    public virtual void StartFire()
    {
        onTurnOn.Invoke();
    }

    public virtual void StopFire()
    {
        onTurnOff.Invoke();
    }

    protected virtual void Update()
    {
        if (!canTrigger || !isOnTimer) return;

        if (Time.time > nextInitTime)
        {
            if (!isFiring)
            {
                isFiring = true;
                nextInitTime = Time.time + onTime;
                StartFire();
            }
            else
            {
                isFiring = false;
                nextInitTime = Time.time + offTime;
                StopFire();
            }
        }
    }
}
