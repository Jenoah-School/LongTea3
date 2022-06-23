using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class LaunchDisks : FighterPower
{
    [SerializeField] Disk diskObject;
    [SerializeField] float diskAmount;
    [SerializeField] float diskSpeed;
    [SerializeField] float diskDamage;
    [SerializeField, Range(0.01f, 0.1f)] float diskAccuracy;
    [SerializeField] float diskLaunchDelay;

    Vector3 initialPos;

    List<GameObject> currentFighters = new List<GameObject>();

    void Start()
    {
        initialPos = transform.position;
    }

    public override void Activate()
    {
        currentFighters = GameObject.FindGameObjectsWithTag("Fighter").ToList();
        currentFighters.Remove(fighterRoot.gameObject);
        if (currentFighters.Count > 0)
        {
            StartCoroutine(FireDisks());
            OnTrigger.Invoke();
            fighterRoot.onUsePowerup();
        }
    }

    IEnumerator FireDisks()
    {
        float totalDegrees = 0;

        for (int i = 0; i < diskAmount; i++)
        {
            Disk disk = Instantiate(diskObject);
            disk.SetVariables(diskDamage, diskSpeed, diskLaunchDelay, diskAccuracy, fighterRoot);
            disk.transform.position = fighterRoot.transform.position + new Vector3(0, 1, 0);
            disk.transform.eulerAngles = new Vector3(0, totalDegrees, 0);
            fighterRoot.IgnoreCollisionWithObject(disk.gameObject);
            totalDegrees = totalDegrees + (360 / diskAmount);
            yield return new WaitForSeconds(0f);

            try
            {
                disk.SetTarget(currentFighters[i].GetComponent<Fighter>());
            }
            catch
            {
                disk.SetTarget(currentFighters[currentFighters.Count - 1].GetComponent<Fighter>());
            }

        }
    }
}
