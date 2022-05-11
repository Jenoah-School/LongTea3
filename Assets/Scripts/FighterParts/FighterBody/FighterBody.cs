using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterBody : FighterPart
{
    [SerializeField] List<Transform> wheelLocations = new List<Transform>();
    [SerializeField] Transform weaponLocationFront;
    [SerializeField] Transform weaponLocationTop;
    [SerializeField] Transform centerOfMass;

    public Transform GetCenterOfMass()
    {
        return centerOfMass;
    }

    public List<Transform> GetWheelLocations()
    {
        return wheelLocations;
    }

    public Transform GetWeaponFrontLocation()
    {
        return weaponLocationFront;
    }

    public Transform GetWeaponTopLocation()
    {
        return weaponLocationTop;
    }
}
