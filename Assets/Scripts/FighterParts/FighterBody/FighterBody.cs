using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterBody : FighterPart
{
    public float fighterHealth;
    [Header("Randomized settings")]
    [SerializeField] private bool hasRandomizedSettings = false;
    [Space(10)]
    [SerializeField] private Vector2 healthRange = new Vector2(800, 1200);
    [Space(10)]
    [SerializeField] private Vector2 maxRandomizedMoveSpeedRange = new Vector2(8f, 12f);
    [SerializeField] private Vector2 maxRandomizedAccelerationRange = new Vector2(20f, 30f);
    [Space(10)]
    [SerializeField] private Vector2 randomizedBrakeDrag = new Vector2(0.05f, 0.2f);
    [SerializeField] private Vector2 randomizedDriftDrag = new Vector2(0.05f, 0.6f);


    [Header("Speeds")]
    [SerializeField] private float maxMoveSpeed = 10f;
    [SerializeField] private float accelerationSpeed = 25f;

    [SerializeField, Range(0, 100)] private int damageIncreasePercentage = 0;
    [SerializeField, Range(0, 100)] private int defenceIncreasePercentage = 0;

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

    private float originalBrakeDrag;
    private float originalDriftDrag;

    private void Awake()
    {
        if (hasRandomizedSettings)
        {
            fighterHealth = Random.Range(healthRange.x, healthRange.y);

            maxMoveSpeed = Random.Range(maxRandomizedMoveSpeedRange.x, maxRandomizedMoveSpeedRange.y);
            accelerationSpeed = Random.Range(maxRandomizedAccelerationRange.x, maxRandomizedAccelerationRange.y);

            brakeDrag = Random.Range(randomizedBrakeDrag.x, randomizedBrakeDrag.y);
            driftDrag = Random.Range(randomizedDriftDrag.x, randomizedDriftDrag.y);
        }


        originalBrakeDrag = brakeDrag;
        originalDriftDrag = driftDrag;
    }

    public int GetDamageReduction()
    {
        return defenceIncreasePercentage;
    }

    public int GetDamageIncrease()
    {
        return damageIncreasePercentage;
    }

    public float GetBrakeDrag()
    {

        return brakeDrag;
    }

    public float GetDriftDrag()
    {
        return driftDrag;
    }

    public float GetOriginalBrakeDrag()
    {
        return originalBrakeDrag;
    }

    public float GetOriginalDriftDrag()
    {
        return originalDriftDrag;
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
