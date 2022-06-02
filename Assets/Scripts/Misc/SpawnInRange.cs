using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;

public class SpawnInRange : MonoBehaviour
{
    [SerializeField] private GameObject objectToSpawn;
    [SerializeField] private float spawnRadius = 5f;
    [SerializeField] private Mesh previewMesh;
    [SerializeField] private float ySpawningRange = 2f;
    [SerializeField] private Vector2 spawnDelay = new Vector2(3f, 10f);
    [SerializeField] private bool canSpawn = true;
    [SerializeField] private bool instantiateUsingLeanPool = true;

    private void Start()
    {
        StartCoroutine(StartSpawning());
    }

    private void SpawnObject()
    {
        if (!canSpawn) return;
        Vector2 spawnPos2D = Random.insideUnitCircle * (spawnRadius / 2);
        Vector3 spawnPosition = transform.position + new Vector3(spawnPos2D.x, Random.Range(0, ySpawningRange), spawnPos2D.y);
        GameObject spawnedObject;
        if (instantiateUsingLeanPool)
        {
            spawnedObject = LeanPool.Spawn(objectToSpawn, spawnPosition, Quaternion.identity, null);
        }
        else {
            spawnedObject = Instantiate(objectToSpawn, spawnPosition, Quaternion.identity, null);
        }
    }

    IEnumerator StartSpawning()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(spawnDelay.x, spawnDelay.y));
            SpawnObject();
            yield return new WaitForEndOfFrame();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireMesh(previewMesh, 0, transform.position + Vector3.up * (ySpawningRange / 2f), Quaternion.identity, new Vector3(spawnRadius, ySpawningRange / 2f, spawnRadius));
    }
}
