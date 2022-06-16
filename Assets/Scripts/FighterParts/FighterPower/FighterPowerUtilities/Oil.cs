using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oil : MonoBehaviour
{
    bool disabling;
    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponentInParent<Fighter>())
        {
            FighterBody affectedFighter = other.GetComponentInParent<Fighter>().GetBody();

            affectedFighter.SetBrakeDrag(0);
            affectedFighter.SetDriftDrag(0);

            if(disabling)
            {
                affectedFighter.SetBrakeDrag(affectedFighter.GetOrigionalBrakeDrag());
                affectedFighter.SetDriftDrag(affectedFighter.GetOrigionalDriftDrag());
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponentInParent<Fighter>())
        {
            FighterBody affectedFighter = other.GetComponentInParent<Fighter>().GetBody();
            affectedFighter.SetBrakeDrag(affectedFighter.GetOrigionalBrakeDrag());
            affectedFighter.SetDriftDrag(affectedFighter.GetOrigionalDriftDrag());
        }
    }

    public void DisableOil()
    {
        disabling = true;
    }
}
