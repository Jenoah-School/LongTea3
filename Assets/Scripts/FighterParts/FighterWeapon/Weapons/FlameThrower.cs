using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FlameThrower : FighterWeapon, IWeapon
{
    private bool isShooting;

    [SerializeField, Range(10,100)] private float overheatValueMargin;
    [SerializeField] private int overheatRate;
    [SerializeField] private int overheatDamageMultiplier;
    [SerializeField] private float damageInterval;

    [SerializeField] ParticleSystem flames;
    [SerializeField] GameObject flamethrowerTip;

    float nextDamageTime;

    private float overheatValue;

    public override void ActivateWeapon(InputAction.CallbackContext context)
    {
        if (context.action.WasPerformedThisFrame())
        {
            isShooting = true;           
        }
        else
        {
            isShooting = false;
        }
    }

    private void Update()
    {
        if (isShooting)
        {
            if (Time.time >= nextDamageTime)
            {
                CheckDamage();
                nextDamageTime = Time.time + damageInterval;
                if (overheatValue > overheatValueMargin)
                {
                    fighterRoot.TakeDamage(1 * overheatDamageMultiplier, fighterRoot, true, true);
                }
            }
            flames.Play();
            if (overheatValue < 100) overheatValue += 0.02f * overheatRate;
        }
        else
        {
            flames.Stop();
            if (overheatValue > 0) overheatValue -= 0.03f * overheatRate;
        }

        Debug.Log(overheatValue);
    }

    public void CheckDamage()
    {
        foreach (GameObject fighter in GameObject.FindGameObjectsWithTag("Fighter"))
        {
            if (fighter == fighterRoot.gameObject) continue;
            if (Vector3.Angle(flamethrowerTip.transform.forward, fighter.transform.position - fighterRoot.transform.position) < (flames.shape.angle * 5))
            {
                if (Vector3.Distance(flamethrowerTip.transform.position, fighter.transform.position) < (((flames.main.startSpeed.constant * 0.75f) * flames.main.startLifetime.constant) * flames.transform.lossyScale.x))
                {
                    fighter.GetComponent<Fighter>().TakeDamage(damage, fighterRoot, true, true);
                }
            }
        }
    }
}
