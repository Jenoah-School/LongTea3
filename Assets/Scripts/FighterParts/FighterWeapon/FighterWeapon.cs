using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FighterWeapon : FighterPart, IWeapon
{
    public int damage;
    public enum WeaponLocations { FRONT, TOP }
    public WeaponLocations weaponLocation;
    public virtual void ActivateWeapon() { }
    public virtual void CheckCollision() { }
}
