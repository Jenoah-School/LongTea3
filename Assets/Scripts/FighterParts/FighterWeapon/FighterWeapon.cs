using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public abstract class FighterWeapon : FighterPart, IWeapon
{
    public enum WeaponLocations { FRONT, TOP, SIDE }
    public enum WeaponOrder { PRIMARY, SECONDARY }

    [HideInInspector] public WeaponOrder weaponOrder;

    public float damage;
    public bool isPair;
    public WeaponLocations weaponLocation;
    public UnityEvent OnAttack;
    public UnityEvent OnStop;

    public virtual void ActivateWeapon(InputAction.CallbackContext context) { }
    public virtual void CheckCollision() { }
}
