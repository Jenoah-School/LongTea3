using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Hammer : FighterWeapon, IWeapon
{
    [SerializeField] float hammerForce;
    [SerializeField] GameObject hammerTip;
    [SerializeField] LayerMask hammerTipMask;

    bool isSwinging;

    public override void ActivateWeapon()
    {
        if (transform.localEulerAngles.x > -1 && transform.localEulerAngles.x < 1)
        {
            float hammerTime = 100 / hammerForce;
            transform.DOLocalRotate(new Vector3(100, 0, 0), hammerTime, RotateMode.Fast);
            isSwinging = true;
            StartCoroutine(ResetHammerWhenDone(hammerTime));
        }
    }

    public void Update()
    {
        if(isSwinging) CheckCollision();
    }

    public override void CheckCollision()
    {
        Collider[] hits = Physics.OverlapBox(hammerTip.transform.position, hammerTip.transform.localScale / 2, hammerTip.transform.rotation);
        if (hits.Length > 0)
        {
            //Debug.Log("Hit something");

            foreach (Collider hit in hits)
            {
                if (hit.gameObject.transform.root != this.gameObject.transform.root)
                {
                    transform.DOKill();
                    transform.DOLocalRotate(new Vector3(0, 0, 0), 100 / (hammerForce / 4), RotateMode.Fast);
                    isSwinging = false;

                    if (hit.gameObject.transform.root.CompareTag("Fighter"))
                    {
                        Fighter otherFighter = hit.gameObject.transform.root.GetComponent<Fighter>();
                        otherFighter.GetComponent<Rigidbody>().AddForceAtPosition(hit.transform.InverseTransformPoint(hammerTip.transform.position), hammerTip.transform.forward * 56000);
                    }
                    if (hit.GetComponentInParent<FighterPart>())
                    {
                        FighterPart part = hit.GetComponentInParent<FighterPart>();
                        part.TakeDamage(damage);
                        Debug.Log("hit part: " + part.name);
                    }
                }
            }
        }
    }

    private IEnumerator ResetHammerWhenDone(float hammerTime)
    {
        yield return new WaitForSeconds(hammerTime);
        //transform.DOLocalRotate(new Vector3(0, 0, 0), 100 / (hammerForce / 4), RotateMode.Fast);
        isSwinging = false;
    }
}
