using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;

public class Rocket : MonoBehaviour
{
    [SerializeField] private float rocketForce;
    [SerializeField] private float rocketHitLaunchForce;
    [SerializeField] float rocketMaxTime;
    [SerializeField] ParticleSystem explosion;
    [SerializeField] GameObject model;

    private int damage;

    private void Start()
    {
        StartCoroutine(DestroyAfterTime());
        GetComponent<Rigidbody>().velocity = transform.forward * rocketForce;
    }

    public void SetVariables(int damage, Fighter origin)
    {
        this.damage = damage;
        origin.IgnoreCollisionWithObject(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.gameObject.layer != LayerMask.NameToLayer("Ignore Raycast"))
        {
            RocketDeath();
            if (other.gameObject.transform.root.CompareTag("Fighter"))
            {
                if (other.GetComponentInParent<FighterPart>())
                {
                    FighterPart part = other.GetComponentInParent<FighterPart>();
                    part.TakeDamage(damage, transform.position);
                    part.GetRigidBodyFighter().AddForceAtPosition((part.transform.up * rocketForce) * (rocketHitLaunchForce * 1.5f), transform.position);
                    part.GetRigidBodyFighter().AddForceAtPosition((transform.forward * rocketForce) * rocketHitLaunchForce, transform.position);
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
