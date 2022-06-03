using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirepitCircleLoop : MonoBehaviour
{
    List<FirePit> firePits = new List<FirePit>();
    [SerializeField] float fireDuration;

    // Start is called before the first frame update
    void Start()
    {
        foreach(FirePit firepit in GetComponentsInChildren<FirePit>())
        {
            firePits.Add(firepit);
            firepit.isOnTimer = false;
        }

        StartCoroutine(StartLoop());
    }

    IEnumerator StartLoop()
    {
        foreach(FirePit firepit in firePits)
        {
            firepit.StartFire();
            yield return new WaitForSeconds(fireDuration);
            firepit.StopFire();
        }

        StartCoroutine(StartLoop());
    }

}
