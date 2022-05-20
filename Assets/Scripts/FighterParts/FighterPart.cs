using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;
using DG.Tweening;
using TMPro;

public class FighterPart : MonoBehaviour
{
    public float healthPoints;
    public float weight;

    protected Fighter fighterRoot;
    protected Rigidbody fighterRigidBody;

    [SerializeField] protected GameObject damageText;
 
    public void SetReferences(Fighter fighter, Rigidbody rb)
    {
        fighterRoot = fighter;
        fighterRigidBody = rb;
    }

    public Rigidbody GetRigidBodyFighter()
    {
        return fighterRigidBody;
    }

    public void TakeDamage(float damage, Vector3 hitPos, Color color)
    {
        if (fighterRoot.isDead) return;
        damage = Mathf.Round(damage);
        healthPoints -= damage;
        GameObject damageTextObject = LeanPool.Spawn(damageText, hitPos, transform.rotation);
        TextMeshPro damageTextObjectText = damageTextObject.GetComponent<TextMeshPro>();
        damageTextObjectText.alpha = 1;
        damageTextObjectText.text = damage.ToString();
        damageTextObjectText.color = color;
        //damageTextObjectText.text = "Bruh";
        damageTextObject.transform.DOMoveY(transform.position.y + damageTextObject.transform.position.y + 2, 3);
        LeanPool.Despawn(damageTextObject, 3);
        fighterRoot.onTakeDamage();
        fighterRoot.CheckDeath();
    }
}
