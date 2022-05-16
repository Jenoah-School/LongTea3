using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class FighterWeapon : FighterPart, IWeapon
{
    public enum WeaponLocations { FRONT, TOP, SIDE }
    public enum WeaponOrder { PRIMARY, SECONDARY }

    [HideInInspector] public WeaponOrder weaponOrder;

    public bool isPair;
    public WeaponLocations weaponLocation;
    public int damage;
    public UnityEvent OnAttack;

    public virtual void ActivateWeapon() { }
    public virtual void CheckCollision() { }
}
