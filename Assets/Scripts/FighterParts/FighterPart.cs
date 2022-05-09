using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterPart : MonoBehaviour
{
    public float healthPoints;
    public float weight;
    public void TakeDamage(int damage)
    {
        healthPoints -= damage;
    }
}
