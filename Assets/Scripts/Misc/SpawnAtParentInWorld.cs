using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;

public class SpawnAtParentInWorld : MonoBehaviour
{
    [SerializeField] private GameObject objectToSpawn;
    [SerializeField] private bool spawnUsingLeanPool;

    public void Spawn()
    {
        if (objectToSpawn != null)
        {
            GameObject spawnedObject;
            if (spawnUsingLeanPool)
            {
                spawnedObject = LeanPool.Spawn(objectToSpawn, transform.position, transform.rotation, null);
            }
            else
            {
                spawnedObject = Instantiate(objectToSpawn, transform.position, transform.rotation, null);
            }
        }
    }
}
