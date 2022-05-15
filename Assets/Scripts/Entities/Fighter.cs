using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public class Fighter : MonoBehaviour
{
    [Header("Vehicle")]
    [SerializeField] private FighterWeapon primaryWeapon;
    [SerializeField] private FighterWeapon secondaryWeapon;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private FighterBody body;
    [SerializeField] private PlayerMovement playerMovement;

    private List<FighterPart> fighterParts = new List<FighterPart>();

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

    }

    public void PostAssemblyStart()
    {
        GetPartReferences();
        IgnoreCollisionOnItself();
        SetFighterPartRigidBodies();
        SetCenterOfMass();
    }

    private void GetPartReferences()
    {
        fighterParts.AddRange(GetComponentsInChildren<FighterPart>());
    }

    private void SetCenterOfMass()
    {
        if (body.GetCenterOfMass() != null)
        {
            rb.centerOfMass = body.GetCenterOfMass().localPosition;
        }
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

    private void OnDrawGizmosSelected()
    {
        if (rb != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(rb.position + rb.centerOfMass, 0.2f);
        }
    }
}
