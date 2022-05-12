using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;

public class Hammer : FighterWeapon, IWeapon
{
    [SerializeField] float hammerForce;
    [SerializeField] float hammerLaunchForceMultiplier;
    [SerializeField] GameObject hammerTip;
    [SerializeField] LayerMask hammerTipMask;

    bool isSwinging;

    Coroutine hammerSwingRotationRoutine;

    public override void ActivateWeapon()
    {
        if (transform.localEulerAngles.x > -1 && transform.localEulerAngles.x < 1)
        {
            float hammerTime = (100 / hammerForce) / 2;
            hammerSwingRotationRoutine = RotateObject.instance.RotateObjectToAngle(this.transform.gameObject, new Vector3(90, 0, 0), hammerTime);
            isSwinging = true;
            StartCoroutine(ResetHammerWhenDone(hammerTime));
        }
    }

    public void Update()
    {
        if (isSwinging) CheckCollision();
    }

    public override void CheckCollision()
    {
        List<Collider> tipHits = new List<Collider>();
        List<Collider> hits = new List<Collider>();

        foreach (Collider childCollider in GetComponentsInChildren<Collider>())
        {
            if (childCollider.gameObject != hammerTip.gameObject)
            {
                hits.AddRange(Physics.OverlapBox(childCollider.transform.position, childCollider.transform.lossyScale / 2, childCollider.transform.rotation, Physics.AllLayers, QueryTriggerInteraction.Collide).ToList());
            }
        }
        tipHits.AddRange(Physics.OverlapBox(hammerTip.transform.position, hammerTip.transform.lossyScale / 2, hammerTip.transform.rotation, Physics.AllLayers, QueryTriggerInteraction.Collide).ToList());

        if (hits.Count > 0)
        {
            //TODO only add alot of force when hammertip or maybe even fix for stick
            foreach (Collider hit in hits)
            {
                if (hit.gameObject.transform.root != this.gameObject.transform.root)
                {
                    //Debug.Log("Hit something " + hit.name);

                    RotateObject.instance.StopCoroutine(hammerSwingRotationRoutine);
                    isSwinging = false;
                    hammerSwingRotationRoutine = RotateObject.instance.RotateObjectToAngle(this.transform.gameObject, new Vector3(0, 0, 0), transform.localEulerAngles.x / 90);
                }
            }
        }

        if (tipHits.Count > 0)
        {
            foreach (Collider hit in tipHits)
            {
                if (hit.gameObject.transform.root != this.gameObject.transform.root && hit.gameObject.transform.root.CompareTag("Fighter"))
                {
                    Fighter otherFighter = hit.gameObject.transform.root.GetComponent<Fighter>();
                    otherFighter.GetComponent<Rigidbody>().AddForceAtPosition(hit.transform.InverseTransformPoint(hammerTip.transform.position), hammerTip.transform.forward * ((hammerForce * hammerLaunchForceMultiplier) * transform.localEulerAngles.x));

                    if (hit.GetComponentInParent<FighterPart>())
                    {
                        FighterPart part = hit.GetComponentInParent<FighterPart>();
                        part.TakeDamage(damage, hammerTip.transform.position);
                    }             
                }
            }
        }
    }

    private IEnumerator ResetHammerWhenDone(float hammerTime)
    {
        yield return new WaitForSeconds(hammerTime);
        isSwinging = false;
        hammerSwingRotationRoutine = RotateObject.instance.RotateObjectToAngle(this.transform.gameObject, new Vector3(0, 0, 0), transform.localEulerAngles.x / 90);
    }
}
