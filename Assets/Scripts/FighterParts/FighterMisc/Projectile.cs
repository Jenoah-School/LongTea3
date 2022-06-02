using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Projectile : MonoBehaviour
{
    private Fighter hitFighter;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.gameObject.layer != LayerMask.NameToLayer("Ignore Raycast"))
        {
            OnHitObject();
            Debug.Log("1 " + collision.transform.name);
            if (collision.transform.GetComponent<Fighter>())
            {
                hitFighter = collision.transform.GetComponent<Fighter>();
                OnHitFighter();
            }
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.gameObject.layer != LayerMask.NameToLayer("Ignore Raycast"))
        {
            OnHitObject();
            Debug.Log("2 " + other.transform.parent.name);
            if (other.GetComponentInParent<Fighter>())
            {
                hitFighter = other.GetComponentInParent<Fighter>();
                OnHitFighter();
            }
        }
    }

    public Fighter GetHitFighter()
    {
        return hitFighter;
    }

    public virtual void OnHitObject() { }
    public virtual void OnHitFighter() { }
}
