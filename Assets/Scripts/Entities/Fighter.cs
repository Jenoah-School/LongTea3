using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public class Fighter : MonoBehaviour
{
    FighterBody body;
    List<FighterWheels> wheels = new List<FighterWheels>();
    [SerializeField] FighterWeapon primaryWeapon;
    FighterWeapon secondaryWeapon;

    [SerializeField] InputActionReference reference;

    public void AssembleFighterParts(FighterBody body, FighterWheels wheels, FighterWeapon primaryWeapon, FighterWeapon secondaryWeapon = null)
    {
        FighterBody bodyObject = Instantiate(body, transform);
        bodyObject.transform.localPosition = new Vector3(0, 0, 0);
        this.body = bodyObject;

        for (int i = 0; i < body.GetWheelLocations().Count; i++)
        {
            FighterWheels wheelsObject = Instantiate(wheels, transform);
            wheelsObject.transform.localPosition = bodyObject.GetWheelLocations().ElementAt(i).transform.position;
            wheelsObject.transform.localEulerAngles = bodyObject.GetWheelLocations().ElementAt(i).transform.eulerAngles;
            this.wheels.Add(wheelsObject);
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
        }
    }

    private void Update()
    {
        if(reference.action.WasPressedThisFrame())
        {
            print("hoi");
            primaryWeapon.ActivateWeapon();
        }
    }
}
