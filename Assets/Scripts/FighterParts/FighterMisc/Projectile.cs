using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Projectile : MonoBehaviour
{
    private Fighter hitFighter;
    private GameObject hitObject = null;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.gameObject.layer != LayerMask.NameToLayer("Ignore Raycast"))
        {
            hitObject = collision.transform.gameObject;
            OnHitObject();
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
            hitObject = other.transform.gameObject;
            OnHitObject();
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

    public GameObject GetHitObject()
    {
        return hitObject;
    }

    public virtual void OnHitObject() { }
    public virtual void OnHitFighter() { }
}
