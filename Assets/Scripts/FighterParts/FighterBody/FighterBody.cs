using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterBody : FighterPart
{
    [SerializeField] List<Transform> wheelLocations = new List<Transform>();
    [SerializeField] Transform weaponLocationFront;
    [SerializeField] Transform weaponLocationTop;

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
