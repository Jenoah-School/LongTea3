using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Lean.Pool;

public class Flipper : FighterWeapon, IWeapon
{
    [SerializeField] float flipForce;
    [SerializeField] float flipLaunchForce;
    [SerializeField] GameObject flipper;
    [SerializeField] LayerMask flipperMask;
    [SerializeField] GameObject hitParticles;

    bool isFlipping;
    bool canHit;

    Coroutine flipperFlipRotationRoutine;

    public override void ActivateWeapon()
    {
        if (transform.localEulerAngles.x > -1 && transform.localEulerAngles.x < 1)
        {
            float flipTime = 10 / flipForce;
            flipperFlipRotationRoutine = RotateObject.instance.RotateObjectToAngle(this.transform.gameObject, new Vector3(-90, 0, 0), flipTime);
            isFlipping = true;
            canHit = true;
            OnAttack.Invoke();
            if (fighterRoot) fighterRoot.onAttack();
            StartCoroutine(ResetFlipperWhenDone(flipTime));
        }
    }

    private void Update()
    {
        if (isFlipping) CheckCollision();
    }

    public override void CheckCollision()
    {
        Collider[] hits = Physics.OverlapBox(flipper.transform.position, flipper.transform.lossyScale / 2, flipper.transform.rotation, ~flipperMask);
        if (hits.Length > 0)
        {
            foreach (Collider hit in hits)
            {
                if (hit.gameObject.transform.root.CompareTag("Fighter") && hit.gameObject.transform.root != this.gameObject.transform.root && canHit)
                {
                    Fighter otherFighter = hit.gameObject.transform.root.GetComponent<Fighter>();
                    otherFighter.GetComponent<Rigidbody>().AddForceAtPosition((fighterRoot.transform.up) * (flipForce * 250) * flipLaunchForce * Mathf.Abs(Physics.gravity.y / 10), otherFighter.transform.position);
                    otherFighter.GetComponent<Rigidbody>().AddRelativeTorque(transform.forward * flipForce * 100 * Mathf.Abs(Vector3.Distance(otherFighter.transform.position, transform.position) * 2));
                    if (hitParticles) LeanPool.Spawn(hitParticles, flipper.transform.position, Quaternion.Euler(-90f, 0, 0));
                    isFlipping = false;
                    canHit = false;
                }
            }
        }
    }

    private IEnumerator ResetFlipperWhenDone(float flipTime)
    {
        yield return new WaitForSeconds(flipTime);
        flipperFlipRotationRoutine = RotateObject.instance.RotateObjectToAngle(this.transform.gameObject, new Vector3(0, 0, 0), flipTime * 4);
        isFlipping = false;
    }
}
