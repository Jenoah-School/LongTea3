using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;
using UnityEngine.Events;

public class Rocket : MonoBehaviour
{
    [SerializeField] private float rocketForce;
    [SerializeField] private float rocketHitLaunchForce;
    [SerializeField] float rocketMaxTime;
    [SerializeField] ParticleSystem explosion;
    [SerializeField] GameObject model;
    [SerializeField] private UnityEvent OnExplode;
    [SerializeField] private LayerMask ignoreLayer;

    private float damage;

    private void Start()
    {
        StartCoroutine(DestroyAfterTime());
        GetComponent<Rigidbody>().velocity = transform.forward * rocketForce;
    }

    public void SetVariables(float damage, Fighter origin)
    {
        this.damage = damage;
        origin.IgnoreCollisionWithObject(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Ignore Raycast"))
        {
            RocketDeath();
            Debug.Log(other.gameObject.transform.root.name);
            if (other.gameObject.transform.root.CompareTag("Fighter"))
            {
                if (other.GetComponentInParent<FighterPart>())
                {
                    FighterPart part = other.GetComponentInParent<FighterPart>();
                    part.TakeDamage(damage, transform.position);
                    part.GetRigidBodyFighter().AddForceAtPosition((part.transform.up * rocketForce) * (rocketHitLaunchForce * 1.5f) * Mathf.Abs(Physics.gravity.y / 10), transform.position);
                    part.GetRigidBodyFighter().AddForceAtPosition((transform.forward * rocketForce) * rocketHitLaunchForce * Mathf.Abs(Physics.gravity.y / 10), transform.position);

                }
            }
        }
    }

    private IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(rocketMaxTime);
        RocketDeath();
    }

    private void RocketDeath()
    {
        explosion.Play();
        OnExplode.Invoke();
        model.SetActive(false);
        GetComponent<Rigidbody>().isKinematic = true;
        StartCoroutine(DespawnSequence(explosion.main.startLifetime.constant));
    }

    IEnumerator DespawnSequence(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(this.gameObject);
    }
}
