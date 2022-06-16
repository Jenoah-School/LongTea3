using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterBody : FighterPart
{
    public float fighterHealth;

    [Header("Speeds")]
    [SerializeField] private float maxMoveSpeed = 10f;
    [SerializeField] private float accelerationSpeed = 25f;

    [Header("Drag")]
    [SerializeField, Range(0f, 1f)] private float brakeDrag = .1f;
    [SerializeField, Range(0f, 1f)] private float driftDrag = 0.4f;
    [Space(10)]
    [SerializeField, Range(0f, 1f)] private float airDrag = 0.6f;
    [SerializeField, Range(0f, 1f)] private float airVerticalDrag = 0.025f;

    [Header("Objects")]
    [SerializeField] private MeshRenderer antennaMeshRenderer;

    [Header("Anchor points")]
    [SerializeField] Transform weaponLocationFront;
    [SerializeField] Transform weaponLocationTop;
    [SerializeField] Transform weaponLocationSideLeft;
    [SerializeField] Transform weaponLocationSideRight;
    [SerializeField] Transform centerOfMass;
    [SerializeField] Transform groundCheckOrigin;

    private float origionalBrakeDrag;
    private float origionalDriftDrag;

    private void Start()
    {
        origionalBrakeDrag = brakeDrag;
        origionalDriftDrag = driftDrag;
    }

    public float GetBrakeDrag()
    {
        return brakeDrag;
    }

    public float GetDriftDrag()
    {
        return driftDrag;
    }

    public float GetOrigionalBrakeDrag()
    {
        return origionalBrakeDrag;
    }

    public float GetOrigionalDriftDrag()
    {
        return origionalDriftDrag;
    }

    public void SetBrakeDrag(float drag)
    {
        brakeDrag = drag;
        fighterRoot.GetPlayerMovement().SetDrag(brakeDrag);
    }

    public void SetDriftDrag(float drag)
    {
        driftDrag = drag;
        fighterRoot.GetPlayerMovement().SetDrag(brakeDrag, driftDrag);
    }

    public float GetAirDrag()
    {
        return airDrag;
    }

    public float GetAirVerticalDrag()
    {
        return airVerticalDrag;
    }

    public float GetMoveSpeed()
    {
        return maxMoveSpeed;
    }

    public float GetAccelerationSpeed()
    {
        return accelerationSpeed;
    }

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

    public Transform GetGroundedCheckOriginTransform()
    {
        return groundCheckOrigin;
    }

    public void SetAntennaColor(Color color)
    {
        if (antennaMeshRenderer != null)
        {
            Material antennaMaterial = antennaMeshRenderer.material;
            antennaMaterial.globalIlluminationFlags = MaterialGlobalIlluminationFlags.None;
            antennaMaterial.EnableKeyword("_Emission");
            antennaMaterial.SetColor("_BaseColor", color);
            antennaMaterial.SetColor("_EmissionColor", color);
        }
    }

    public Transform GetWeaponLocation(FighterWeapon.WeaponLocations location)
    {
        if (location == FighterWeapon.WeaponLocations.FRONT)
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
