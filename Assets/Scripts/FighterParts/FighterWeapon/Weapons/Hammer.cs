using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Hammer : FighterWeapon, IWeapon
{
    [SerializeField] float hammerForce;

    public override void ActivateWeapon()
    {     
        transform.DORotate(new Vector3(100, 0, 0), 1 / (hammerForce / 10));
    }

    private void OnCollisionEnter(Collision collision)
    {
        print("123");
    }
}
