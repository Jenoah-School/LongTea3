using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class FighterWeapon : FighterPart, IWeapon
{
    public enum WeaponLocations { FRONT, TOP }

    public WeaponLocations weaponLocation;
    public int damage;
    public int IFrameAmount;
    public UnityEvent OnAttack;

    public virtual void ActivateWeapon() { }
    public virtual void CheckCollision() { }
}
