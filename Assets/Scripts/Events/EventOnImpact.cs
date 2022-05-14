using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Lean.Pool;

public class EventOnImpact : MonoBehaviour
{
    [SerializeField] private float minimumImpactSpeed = 3f;
    [SerializeField] private List<GameObject> spawnOnImpactPosition = new List<GameObject>();
    [SerializeField] private List<GameObject> objectsToIgnoreInCollison = new List<GameObject>();
    [SerializeField] private UnityEvent OnImpact;
    [SerializeField] private float eventCoolDown = 0.5f;

    private float nextImpactTime = 0f;

    private void OnCollisionEnter(Collision collision)
    {
        if(Time.time >= nextImpactTime && collision.relativeVelocity.magnitude > minimumImpactSpeed)
        {
            OnImpact.Invoke();
            if(spawnOnImpactPosition.Count > 0)
            {
                foreach(GameObject objectToSpawn in spawnOnImpactPosition)
                {
                    LeanPool.Spawn(objectToSpawn, collision.contacts[0].point, Quaternion.LookRotation(collision.contacts[0].normal), collision.transform);
                }
            }
            nextImpactTime = Time.time + eventCoolDown;
        }
    }
}
