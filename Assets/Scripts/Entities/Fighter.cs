using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Fighter : MonoBehaviour
{
    FighterBody body;
    FighterWheels wheels;
    FighterWeapon primaryWeapon;
    FighterWeapon secondaryWeapon;

    public void AssembleFighter(FighterBody body, FighterWheels wheels, FighterWeapon primaryWeapon, FighterWeapon secondaryWeapon = null)
    {
        this.body = body;
        this.wheels = wheels;
        this.primaryWeapon = primaryWeapon;
        this.secondaryWeapon = secondaryWeapon;

        Instantiate(body, transform);
        body.transform.localPosition = new Vector3(0, 0, 0);

        for (int i = 0; i < body.GetWheelLocations().Count; i++)
        {
            Instantiate(wheels, transform);
            wheels.transform.localPosition = body.GetWheelLocations().ElementAt(i).transform.position;
            wheels.transform.localEulerAngles = body.GetWheelLocations().ElementAt(i).transform.eulerAngles;
        }

        Instantiate(primaryWeapon, transform);
        if (primaryWeapon.weaponLocation == FighterWeapon.WeaponLocations.FRONT)
        {
            primaryWeapon.transform.localPosition = body.GetWeaponFrontLocation().position;
            primaryWeapon.transform.localEulerAngles = body.GetWeaponFrontLocation().eulerAngles;
        }
        else
        {
            primaryWeapon.transform.localPosition = body.GetWeaponTopLocation().position;
            primaryWeapon.transform.localEulerAngles = body.GetWeaponTopLocation().eulerAngles;
        }
        
        if (secondaryWeapon)
        {
            Instantiate(secondaryWeapon, transform);
            if (secondaryWeapon.weaponLocation == FighterWeapon.WeaponLocations.FRONT)
            {
                secondaryWeapon.transform.localPosition = body.GetWeaponFrontLocation().position;
                secondaryWeapon.transform.localEulerAngles = body.GetWeaponFrontLocation().eulerAngles;
            }
            else
            {
                secondaryWeapon.transform.localPosition = body.GetWeaponTopLocation().position;
                secondaryWeapon.transform.localEulerAngles = body.GetWeaponTopLocation().eulerAngles;
            }
        }
    }
}
