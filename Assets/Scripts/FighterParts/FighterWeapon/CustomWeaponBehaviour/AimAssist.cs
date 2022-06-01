using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimAssist : MonoBehaviour
{
    private bool isAiming;

    public void StartAimAssist(Transform ownerTransform, Fighter ownerFighter, float aimSpeed, float range, float FOV = 180) 
    {
        GameObject aimTarget = null; 

        float minDist = Mathf.Infinity;

        foreach (GameObject fighter in GameObject.FindGameObjectsWithTag("Fighter"))
        {
            if (fighter == ownerFighter.gameObject) continue;

            if (Vector2.Angle(new Vector2(ownerFighter.transform.forward.x, ownerFighter.transform.forward.z), new Vector2(fighter.transform.position.x - ownerFighter.transform.position.x, fighter.transform.position.z - ownerFighter.transform.position.z)) < FOV)
            {
                float dist = Vector3.Distance(ownerTransform.transform.position, fighter.transform.position);
                if (dist < range && dist < minDist)
                {
                    minDist = dist;
                    aimTarget = fighter;
                }
            }          
        }
        if (aimTarget)
        {
            isAiming = true;
            ownerTransform.transform.rotation = Quaternion.RotateTowards(ownerTransform.transform.rotation, Quaternion.LookRotation((aimTarget.transform.position - ownerFighter.transform.position).normalized), Time.deltaTime * aimSpeed);
        }
        else
        {
            isAiming = false;
            ResetAim(ownerTransform, aimSpeed);
        }
    }

    public bool IsAiming()
    {
        return isAiming;
    }

    public void ResetAim(Transform owner, float aimSpeed)
    {
        owner.transform.localRotation = Quaternion.RotateTowards(owner.transform.localRotation, Quaternion.Euler(0,0,0), Time.deltaTime * aimSpeed);
    }
}
