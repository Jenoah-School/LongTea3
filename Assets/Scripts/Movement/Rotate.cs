using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    [SerializeField]
    private float rotationSpeed = 5f;
    [SerializeField] private Vector3 axisWeight = Vector3.forward;

    // Update is called once per frame
    void Update()
    {
        transform.localEulerAngles += rotationSpeed * Time.deltaTime * axisWeight;
    }
}
