using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : Pickup
{
    [SerializeField] private float healPoints = 250f;

    public override void Activate()
    {
        base.Activate();
        triggeredFighter.TakeDamage(-healPoints, triggeredFighter, false, false);
    }
}
