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
    [SerializeField] float diskLaunchDelay;
    Fighter fighterRoot;

    bool isMoving = false;

    public void SetVariables(float damage, float moveSpeed, float diskLaunchDelay, Fighter fighterRoot)
    {
        this.damage = damage;
        this.moveSpeed = moveSpeed;
        this.fighterRoot = fighterRoot;
        this.diskLaunchDelay = diskLaunchDelay;
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

        if(isMoving)
        {
            var step = moveSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, step);
        }
    }

    IEnumerator FireDisk()
    {
        yield return new WaitForSeconds(diskLaunchDelay);
        isMoving = true;
    }
}
