using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public class Fighter : MonoBehaviour
{
    [Header("Vehicle")]
    private List<FighterPart> fighterParts = new List<FighterPart>();
    [SerializeField] List<FighterWheels> wheels = new List<FighterWheels>();
    [SerializeField] FighterWeapon primaryWeapon;
    [SerializeField] FighterWeapon secondaryWeapon;
    [SerializeField] Rigidbody rb;
    [SerializeField] FighterBody body;

    [Header("Driving")]
    [SerializeField] List<FighterWheels> steerableWheels = new List<FighterWheels>();
    [SerializeField] List<FighterWheels> motorWheels = new List<FighterWheels>();
    [SerializeField] float steerSmoothing = 10f;
    [SerializeField] float backwardsMultiplier = 0.5f;

    private float targetSteeringAngle = 0f;

    public void AssembleFighterParts(FighterBody body, FighterWheels wheels, FighterWeapon primaryWeapon, FighterWeapon secondaryWeapon = null)
    {
        FighterBody bodyObject = Instantiate(body, transform);
        bodyObject.transform.localPosition = new Vector3(0, 0, 0);
        this.body = bodyObject;
        fighterParts.Add(bodyObject);

        for (int i = 0; i < body.GetWheelLocations().Count; i++)
        {
            FighterWheels wheelsObject = Instantiate(wheels, transform);
            wheelsObject.transform.localPosition = bodyObject.GetWheelLocations().ElementAt(i).transform.position;
            wheelsObject.transform.localEulerAngles = bodyObject.GetWheelLocations().ElementAt(i).transform.eulerAngles;
            wheelsObject.name = $"Wheel {i}";
            motorWheels.Add(wheelsObject);
            if (wheelsObject.transform.localPosition.z > 0) steerableWheels.Add(wheelsObject);
            this.wheels.Add(wheelsObject);
            fighterParts.Add(wheelsObject);
        }

        FighterWeapon primaryWeaponObject = Instantiate(primaryWeapon, transform);
        if (primaryWeaponObject.weaponLocation == FighterWeapon.WeaponLocations.FRONT)
        {
            primaryWeaponObject.transform.localPosition = bodyObject.GetWeaponFrontLocation().position;
            primaryWeaponObject.transform.localEulerAngles = bodyObject.GetWeaponFrontLocation().eulerAngles;
        }
        else
        {
            primaryWeaponObject.transform.localPosition = bodyObject.GetWeaponTopLocation().position;
            primaryWeaponObject.transform.localEulerAngles = bodyObject.GetWeaponTopLocation().eulerAngles;
        }
        this.primaryWeapon = primaryWeaponObject;
        fighterParts.Add(primaryWeaponObject);

        if (secondaryWeapon)
        {
            FighterWeapon secondaryWeaponObject = Instantiate(secondaryWeapon, transform);
            if (secondaryWeaponObject.weaponLocation == FighterWeapon.WeaponLocations.FRONT)
            {
                secondaryWeaponObject.transform.localPosition = bodyObject.GetWeaponFrontLocation().position;
                secondaryWeaponObject.transform.localEulerAngles = bodyObject.GetWeaponFrontLocation().eulerAngles;
            }
            else
            {
                secondaryWeaponObject.transform.localPosition = bodyObject.GetWeaponTopLocation().position;
                secondaryWeaponObject.transform.localEulerAngles = bodyObject.GetWeaponTopLocation().eulerAngles;
            }
            this.secondaryWeapon = secondaryWeaponObject;
            fighterParts.Add(secondaryWeaponObject);
        }

        if(this.body.GetCenterOfMass() != null)
        {
            rb.centerOfMass = rb.transform.InverseTransformPoint(this.body.GetCenterOfMass().localPosition);
        }

        IgnoreCollisionOnItself();
        SetFighterPartRigidBodies();
    }

    private void IgnoreCollisionOnItself()
    {
        foreach (Collider col1 in GetComponentsInChildren<Collider>())
        {
            foreach (Collider col2 in GetComponentsInChildren<Collider>())
            {
                Physics.IgnoreCollision(col1, col2);
            }
        }
    }

    private void SetFighterPartRigidBodies()
    {
        foreach (FighterPart part in fighterParts)
        {
            part.SetReferenceRigidBody(rb);
        }
    }

    public float GetTotalPartHealth()
    {
        float totalHealth = 0;
        foreach (FighterPart part in fighterParts)
        {
            totalHealth += part.healthPoints;
        }
        return totalHealth;
    }

    public void ApplyMotorTorque(InputAction.CallbackContext context)
    {
        foreach (FighterWheels wheel in motorWheels)
        {
            wheel.wheelCollider.motorTorque = wheel.speed * Mathf.Max(-backwardsMultiplier, context.ReadValue<float>());
        }
    }

    public void ApplyWheelRotation(InputAction.CallbackContext context)
    {
        targetSteeringAngle = steerableWheels[0].maxSteeringAngle * context.ReadValue<Vector2>().x;
    }

    public void ExecutePrimary(InputAction.CallbackContext context)
    {
        if (context.action.WasPerformedThisFrame() && primaryWeapon != null)
        {
            primaryWeapon.ActivateWeapon();
        }
    }

    public void ExecuteSecondary(InputAction.CallbackContext context)
    {
        if (context.action.WasPerformedThisFrame() && secondaryWeapon != null)
        {
            secondaryWeapon.ActivateWeapon();
        }
    }

    private void Update()
    {
        foreach (FighterWheels wheel in steerableWheels)
        {
            wheel.wheelCollider.steerAngle = Mathf.Lerp(wheel.wheelCollider.steerAngle, targetSteeringAngle, steerSmoothing * Time.deltaTime);
        }

        foreach (FighterWheels wheel in wheels)
        {
            if (wheel.wheelMesh != null && wheel.wheelCollider != null)
            {
                wheel.wheelCollider.GetWorldPose(out Vector3 wheelPosition, out Quaternion wheelRotation);
                wheel.wheelMesh.transform.SetPositionAndRotation(wheelPosition, wheelRotation);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (rb != null && body != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(rb.position + body.GetCenterOfMass().localPosition, 0.2f);
        }
    }
}
