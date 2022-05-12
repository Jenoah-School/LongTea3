using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookInDirection : MonoBehaviour
{
    [SerializeField] private Vector3 lookDirection;


    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.LookRotation(lookDirection);
    }
}
