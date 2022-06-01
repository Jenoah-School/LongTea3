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

    public void TakeDamage(float damage, Vector3 hitPos, bool showDamage = true, bool doStack = false)
    {
        if (fighterRoot.isDead) return;
        damage = (float)System.Math.Round(damage, 2);
        healthPoints -= damage;
        
        if(showDamage) fighterRoot.DamageIndication(damage, hitPos, fighterRoot, doStack);

        fighterRoot.CheckDeath();
    }
}
