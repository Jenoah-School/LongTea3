using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterPart : MonoBehaviour
{
    public float healthPoints;
    public float weight;

    protected Fighter fighterRoot;
    protected Rigidbody fighterRigidBody;
    
    public void SetReferences(Fighter fighter, Rigidbody rb)
    {
        fighterRoot = fighter;
        fighterRigidBody = rb;
    }

    public Rigidbody GetRigidBodyFighter()
    {
        return fighterRigidBody;
    }

    public void TakeDamage(float damage, Vector3 hitPos, bool showDamage = true)
    {
        if (fighterRoot.isDead) return;
        damage = Mathf.Round(damage);
        healthPoints -= damage;

        if(showDamage) fighterRoot.DamageIndication(damage, hitPos);

        fighterRoot.CheckDeath();
    }
}
