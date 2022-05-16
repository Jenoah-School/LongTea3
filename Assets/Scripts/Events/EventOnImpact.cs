using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Lean.Pool;

public class EventOnImpact : MonoBehaviour
{
    [SerializeField] private float minimumImpactSpeed = 3f;
    [SerializeField] private List<GameObject> spawnOnImpactPosition = new List<GameObject>();
    [SerializeField] private UnityEvent OnImpact;
    [SerializeField] private float eventCoolDown = 0.5f;

    [Header("Ignore direction")]
    [SerializeField] private bool ignoreBounds = false;
    [SerializeField] private Collider checkCollider = null;

    private float nextImpactTime = 0f;

    private void OnCollisionEnter(Collision collision)
    {
        bool isInBounds = IsInside(collision.contacts[0].point, checkCollider);

        if (Time.time >= nextImpactTime && collision.relativeVelocity.magnitude > minimumImpactSpeed && (!ignoreBounds || isInBounds))
        {
            OnImpact.Invoke();
            if (spawnOnImpactPosition.Count > 0)
            {
                foreach (GameObject objectToSpawn in spawnOnImpactPosition)
                {
                    LeanPool.Spawn(objectToSpawn, collision.contacts[0].point, Quaternion.LookRotation(collision.contacts[0].normal), collision.transform);
                }
            }

            nextImpactTime = Time.time + eventCoolDown;
        }
    }

    public static bool IsInside(Vector3 p_Point, Collider p_Collider)
    {
        SphereCollider l_SphereCollider = p_Collider as SphereCollider;
        if (l_SphereCollider != null)
        {
            return IsInside(p_Point, l_SphereCollider);
        }

        BoxCollider l_BoxCollider = p_Collider as BoxCollider;
        if (l_BoxCollider != null)
        {
            return IsInside(p_Point, l_BoxCollider);
        }

        return false;
    }

    public static bool IsInside(Vector3 p_Point, BoxCollider p_Box)
    {
        p_Point = p_Box.transform.InverseTransformPoint(p_Point) - p_Box.center;
        float l_HalfX = (p_Box.size.x * 0.5f);
        float l_HalfY = (p_Box.size.y * 0.5f);
        float l_HalfZ = (p_Box.size.z * 0.5f);
        return (p_Point.x < l_HalfX && p_Point.x > -l_HalfX &&
            p_Point.y < l_HalfY && p_Point.y > -l_HalfY &&
            p_Point.z < l_HalfZ && p_Point.z > -l_HalfZ);
    }

    public static bool IsInside(Vector3 p_Point, SphereCollider p_Sphere)
    {
        p_Point = p_Sphere.transform.InverseTransformPoint(p_Point) - p_Sphere.center;
        return p_Point.sqrMagnitude <= p_Sphere.radius * p_Sphere.radius;
    }
}
