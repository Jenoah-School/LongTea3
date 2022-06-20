using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Disk : Projectile
{
    [SerializeField] GameObject model;

    float rotationSpeed;
    float initialModelScale;

    [SerializeField] Fighter target;

    float damage;
    float moveSpeed;
    float diskLaunchDelay;
    float diskAccuracy;
    Fighter fighterRoot;

    bool isMoving = false;

    public void SetVariables(float damage, float moveSpeed, float diskLaunchDelay, float diskAccuracy, Fighter fighterRoot)
    {
        this.damage = damage;
        this.moveSpeed = moveSpeed;
        this.diskLaunchDelay = diskLaunchDelay;
        this.diskLaunchDelay = diskLaunchDelay;
        this.diskAccuracy = diskAccuracy;
        this.fighterRoot = fighterRoot;
    }

    private void Start()
    {
        initialModelScale = model.transform.lossyScale.x;
        model.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        model.transform.DOScale(initialModelScale, diskLaunchDelay);
        fighterRoot.IgnoreCollisionWithObject(this.gameObject);
        transform.DOMove(transform.position + transform.forward, diskLaunchDelay);
        StartCoroutine(FireDisk());
    }

    public void SetTarget(Fighter target)
    {
        this.target = target;
    }

    public override void OnHitFighter()
    {
        Debug.Log("HIT FIGHTER");
        Fighter hitFighter = GetHitFighter();
        Destroy(this.gameObject);
        hitFighter.TakeDamage(damage, fighterRoot);
    }

    private void FixedUpdate()
    {
        rotationSpeed += 0.1f;
        model.transform.Rotate(transform.up, rotationSpeed);

        if (isMoving)
        {
            var step = moveSpeed * Time.deltaTime;
            transform.position += transform.forward * (moveSpeed / 100);
            transform.forward = Vector3.Lerp(transform.forward, (target.transform.position - transform.position).normalized, diskAccuracy);
        }
    }

    IEnumerator FireDisk()
    {
        yield return new WaitForSeconds(diskLaunchDelay);
        isMoving = true;
    }
}
