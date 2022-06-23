using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Spikes : FighterWeapon
{
    [SerializeField] float cooldown;
    [SerializeField, Range(1,1.5f)] float speedRate;
    [SerializeField] float speedTime;

    float prevDamageTime;
    float prevSpeedTime;

    PlayerMovement movement;

    private void Start()
    {
        movement = fighterRoot.GetComponent<PlayerMovement>();
    }

    public override void ActivateWeapon(InputAction.CallbackContext context)
    {
        if (context.action.WasPerformedThisFrame())
        {
            if (Time.time > prevSpeedTime)
            {
                prevSpeedTime = Time.time + cooldown;
                movement.SetAccelerationSpeed(movement.GetAccelarationSpeed() * speedRate);
                movement.SetMaxMoveSpeed(movement.GetMaxMoveSpeed() * speedRate);
                StartCoroutine(ResetSpeed());
                OnAttack.Invoke();
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.GetComponent<Fighter>() && Time.time >= prevDamageTime)
        {
            prevDamageTime = Time.time + 0.5f;
            Fighter hitFighter = collision.transform.GetComponent<Fighter>();
            hitFighter.TakeDamage(damage, fighterRoot);
        }
    }

    IEnumerator ResetSpeed()
    {
        yield return new WaitForSeconds(speedTime);
        movement.SetAccelerationSpeed(movement.GetAccelarationSpeed() / speedRate);
        movement.SetMaxMoveSpeed(movement.GetMaxMoveSpeed() / speedRate);
    }
}
