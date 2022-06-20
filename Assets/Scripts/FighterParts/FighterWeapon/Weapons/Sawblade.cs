using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class Sawblade : FighterWeapon
{
    [SerializeField] GameObject sawBladeObject;
    [SerializeField] float cooldown;
    [SerializeField] float extendTime;

    float prevDamageTime;
    float prevExtendTime;

    public override void ActivateWeapon(InputAction.CallbackContext context)
    {
        if (context.action.WasPerformedThisFrame())
        {
            if (Time.time > prevExtendTime)
            {
                prevExtendTime = Time.time + cooldown;
                StartCoroutine(ExtendWeapon());
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.transform.GetComponentInParent<Fighter>() && Time.time >= prevDamageTime && other.transform.gameObject.layer != LayerMask.NameToLayer("Ignore Raycast"))
        {
            prevDamageTime = Time.time + 0.1f;
            Fighter hitFighter = other.transform.GetComponentInParent<Fighter>();
            hitFighter.TakeDamage(damage, fighterRoot, true, true);
        }
    }

    private IEnumerator ExtendWeapon()
    {
        float initialZPos = sawBladeObject.transform.localPosition.z;
        sawBladeObject.transform.DOLocalMoveZ(2, extendTime/3);
        yield return new WaitForSeconds(extendTime);
        sawBladeObject.transform.DOLocalMoveZ(initialZPos, extendTime/3);
    }

    void Start()
    {
        
    }

    void FixedUpdate()
    {
        sawBladeObject.transform.Rotate(0, 0, 1000 * Time.deltaTime);
    }
}
