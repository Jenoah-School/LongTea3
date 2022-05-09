using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Flipper : FighterWeapon, IWeapon
{
    [SerializeField] float flipForce;
    [SerializeField] GameObject flipper;
    [SerializeField] LayerMask flipperMask;

    bool isFlipping;

    public override void ActivateWeapon()
    {
        if (transform.localEulerAngles.x > -1 && transform.localEulerAngles.x < 1)
        {
            float flipTime = 10 / flipForce;
            transform.DOLocalRotate(new Vector3(-90, 0, 0), flipTime, RotateMode.Fast);
            isFlipping = true;
            StartCoroutine(ResetFlipperWhenDone(flipTime));
        }
    }

    private void Update()
    {
        if(isFlipping) CheckCollision();
    }

    public override void CheckCollision()
    {
        Collider[] hits = Physics.OverlapBox(flipper.transform.position, flipper.transform.localScale / 2, flipper.transform.rotation, ~flipperMask);
        if (hits.Length > 0)
        {
            foreach (Collider hit in hits)
            {
                if (hit.gameObject.transform.root.CompareTag("Fighter") && hit.gameObject.transform.root != this.gameObject.transform.root)
                {
                    Fighter otherFighter = hit.gameObject.transform.root.GetComponent<Fighter>();
                    otherFighter.GetComponent<Rigidbody>().AddForceAtPosition(hit.transform.InverseTransformPoint(flipper.transform.position), flipper.transform.forward * 56000);
                    isFlipping = false;
                }
            }
        }
    }

    private IEnumerator ResetFlipperWhenDone(float flipTime)
    {
        yield return new WaitForSeconds(flipTime);
        transform.DOLocalRotate(new Vector3(0, 0, 0), flipTime * 4, RotateMode.Fast);
        isFlipping = false;
    }
}
