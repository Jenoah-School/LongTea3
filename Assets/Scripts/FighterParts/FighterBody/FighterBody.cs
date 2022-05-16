using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterBody : FighterPart
{
    [SerializeField] Transform weaponLocationFront;
    [SerializeField] Transform weaponLocationTop;
    [SerializeField] Transform weaponLocationSideLeft;
    [SerializeField] Transform weaponLocationSideRight;
    [SerializeField] Transform centerOfMass;

    public Transform GetCenterOfMass()
    {
        return centerOfMass;
    }

    private Transform GetWeaponFrontLocation()
    {
        return weaponLocationFront;
    }

    private Transform GetWeaponTopLocation()
    {
        return weaponLocationTop;
    }

    private Transform GetWeaponLeftSideLocation()
    {
        return weaponLocationSideLeft;
    }

    public Transform GetWeaponRightSideLocation()
    {
        return weaponLocationSideRight;
    }

    public Transform GetWeaponLocation(FighterWeapon.WeaponLocations location)
    {
        if(location == FighterWeapon.WeaponLocations.FRONT)
        {
            return GetWeaponFrontLocation();
        }
        if (location == FighterWeapon.WeaponLocations.TOP)
        {
            return GetWeaponTopLocation();
        }
        if (location == FighterWeapon.WeaponLocations.SIDE)
        {
            return GetWeaponLeftSideLocation();
        }
        return null;
    }
}
