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
                affectedFighter.SetBrakeDrag(affectedFighter.GetOriginalBrakeDrag());
                affectedFighter.SetDriftDrag(affectedFighter.GetOriginalDriftDrag());
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponentInParent<Fighter>())
        {
            FighterBody affectedFighter = other.GetComponentInParent<Fighter>().GetBody();
            affectedFighter.SetBrakeDrag(affectedFighter.GetOriginalBrakeDrag());
            affectedFighter.SetDriftDrag(affectedFighter.GetOriginalDriftDrag());
        }
    }

    public void DisableOil()
    {
        disabling = true;
    }
}
