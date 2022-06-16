using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OilSlick : FighterPower
{
    [SerializeField] float hitDamage;
    [SerializeField] OilBarrel barrel;
    [SerializeField] float shootForce;
    [SerializeField] float oilDuration;
    [SerializeField] float oilMaxSize;

    public override void Activate()
    {
        ShootBarrel();
    }

    private void ShootBarrel()
    {
        OilBarrel barrelClone = Instantiate(barrel);
        barrelClone.SetVariables(hitDamage, oilDuration, oilMaxSize, fighterRoot);
        barrelClone.transform.position = fighterRoot.transform.position + new Vector3(0,1,0);
        fighterRoot.IgnoreCollisionWithObject(barrelClone.gameObject);
        barrelClone.GetComponent<Rigidbody>().AddForce(((fighterRoot.transform.forward / 2) + fighterRoot.transform.up) * shootForce);
        barrelClone.GetComponent<Rigidbody>().AddTorque((fighterRoot.transform.right) * shootForce / Random.Range(1,4));
    }
}
