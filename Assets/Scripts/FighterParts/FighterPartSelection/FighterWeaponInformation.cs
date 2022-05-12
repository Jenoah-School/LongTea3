using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Fighter Weapon info", menuName = "Fighter/Fighter Weapon Info", order = 2)]
public class FighterWeaponInformation : FighterPartInformation
{
    public string weaponName = "Weapon name";
    public float damage = 0;
    public float range = 0;
}
