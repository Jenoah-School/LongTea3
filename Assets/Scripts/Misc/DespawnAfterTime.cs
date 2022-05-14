using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;

public class DespawnAfterTime : MonoBehaviour
{
    [SerializeField] private float despawnTime = 3f;
    [SerializeField] private bool despawnUsingLeanPool = true;

    private void Start()
    {
        if (despawnUsingLeanPool)
        {
            LeanPool.Despawn(gameObject, despawnTime);
        }
        else
        {
            Destroy(gameObject, despawnTime);
        }
    }
}
