using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Mine : Projectile
{
    [SerializeField] GameObject model;
    [SerializeField] ParticleSystem explosion;
    [SerializeField] UnityEvent OnHitTarget;
    [SerializeField] UnityEvent OnBounce;

    private float mineDamage;
    private float mineHitLaunchForce;
    float maxMineTime = 10;

    Fighter fighterRoot;

    private void Start()
    {
        StartCoroutine(DestroyAfterTime());
    }

    public void SetVariables(float damage, float launchForce, float maxMineTime, Fighter fighterRoot)
    {
        mineDamage = damage;
        mineHitLaunchForce = launchForce;
        this.maxMineTime = maxMineTime;
        this.fighterRoot = fighterRoot;
    }

    public override void OnHitFighter()
    {
        model.SetActive(false);
        GetComponent<Rigidbody>().isKinematic = true;

        Fighter hitFighter = GetHitFighter();
        OnHitTarget.Invoke();

        Vector3 forceDirectionVector = (hitFighter.transform.position - transform.position).normalized;

        hitFighter.GetRigidBody().AddForceAtPosition((hitFighter.transform.up * 20) * (mineHitLaunchForce * 1.5f) * Mathf.Abs(Physics.gravity.y / 10), transform.position);
        hitFighter.GetRigidBody().AddForceAtPosition((forceDirectionVector * 20) * mineHitLaunchForce * Mathf.Abs(Physics.gravity.y / 10), transform.position);
        hitFighter.TakeDamage(mineDamage, fighterRoot);

        explosion.Play();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject != fighterRoot.gameObject)
        {
            OnBounce.Invoke();
        }
    }

    private IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(maxMineTime);
        Destroy(this.gameObject);
    }
}
