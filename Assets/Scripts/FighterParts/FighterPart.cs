using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterPart : MonoBehaviour
{
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
}
