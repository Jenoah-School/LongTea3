using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchMines : FighterPower
{
    [SerializeField] private Mine mineObject;

    [SerializeField] private int mineAmount;
    [SerializeField] private float mineSpeed;
    [SerializeField] private float mineLaunchForce;
    [SerializeField] private float mineLifetime;
    [SerializeField] private float damage;

    public override void Activate()
    {
        StartCoroutine(FireMines());
        fighterRoot.onUsePowerup();
        OnTrigger.Invoke();  
    }

    IEnumerator FireMines()
    {
        for (int i = 0; i < mineAmount; i++)
        {
            Mine mine = Instantiate(mineObject);
            mine.transform.position = fighterRoot.transform.position + new Vector3(0, 1, 0);

            mine.transform.Rotate(0, Random.Range(0, 360), 0);
            mine.GetComponent<Rigidbody>().velocity = mine.transform.forward * mineSpeed;
            mine.GetComponent<Rigidbody>().AddTorque(new Vector3(Random.Range(-1, 0), Random.Range(-1, 0), Random.Range(-1, 0)) * Random.Range(100,500));
            mine.SetVariables(damage, mineLaunchForce, mineLifetime, fighterRoot);

            fighterRoot.IgnoreCollisionWithObject(mine.gameObject);

            yield return new WaitForSeconds(0.25f);
        }
    }
}
