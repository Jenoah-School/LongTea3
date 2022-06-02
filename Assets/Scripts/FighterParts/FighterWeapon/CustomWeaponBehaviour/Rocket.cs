using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;
using UnityEngine.Events;

public class Rocket : Projectile
{
    [SerializeField] private float rocketForce;
    [SerializeField] private float rocketHitLaunchForce;
    [SerializeField] float rocketMaxTime;
    [SerializeField] ParticleSystem explosion;
    [SerializeField] GameObject model;
    [SerializeField] private UnityEvent OnExplode;
    [SerializeField] private LayerMask ignoreLayer;

    private float damage;
    private Fighter fighterRoot;

    private void Start()
    {
        StartCoroutine(DestroyAfterTime());
        GetComponent<Rigidbody>().velocity = transform.forward * rocketForce;
    }

    public void SetVariables(float damage, Fighter origin)
    {
        this.damage = damage;
        fighterRoot = origin;
        origin.IgnoreCollisionWithObject(gameObject);
    }

    public override void OnHitObject()
    {
        RocketDeath();
    }

    public override void OnHitFighter()
    {
        Fighter hitFighter = GetHitFighter();
        Debug.Log(hitFighter);
        hitFighter.TakeDamage(damage, fighterRoot);
        hitFighter.GetRigidBody().AddForceAtPosition((hitFighter.transform.up * rocketForce) * (rocketHitLaunchForce * 1.5f) * Mathf.Abs(Physics.gravity.y / 10), transform.position);
        hitFighter.GetRigidBody().AddForceAtPosition((transform.forward * rocketForce) * rocketHitLaunchForce * Mathf.Abs(Physics.gravity.y / 10), transform.position);
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
