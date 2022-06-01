using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchMines : FighterPower
{
    [SerializeField] private GameObject mineObject;
    [SerializeField] private int mineAmount;

    public override void Activate()
    {
        base.Activate();
        for (int i = 0; i < mineAmount; i++)
        {
            GameObject mine = Instantiate(mineObject);
            mine.transform.position = fighterRoot.transform.position + new Vector3(0,3,0);
            mine.GetComponent<Rigidbody>().velocity = new Vector3(Random.Range(0, 1), Random.Range(0, 1), Random.Range(0, 1)).normalized * 10;
        }
    }
}
