using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private Vector3 axisWeight = Vector3.forward;
    [SerializeField] private bool canRotate = true;

    private Quaternion startRotation = Quaternion.identity;

    private void Start()
    {
        startRotation = transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (canRotate) transform.Rotate(rotationSpeed * Time.deltaTime * axisWeight, Space.Self);
    }

    public void ResetAngle(float rotationTime)
    {
        StopAllCoroutines();
        StartCoroutine(ResetAngleEnum(rotationTime));
    }

    public IEnumerator ResetAngleEnum(float rotationTime)
    {
        canRotate = false;
        Quaternion currentRotation = transform.localRotation;
        float elapsedTime = 0f;

        while (elapsedTime < rotationTime)
        {
            elapsedTime += Time.deltaTime;
            transform.localRotation = Quaternion.Slerp(currentRotation, startRotation, elapsedTime / rotationTime);
            yield return new WaitForEndOfFrame();
        }
        transform.localRotation = startRotation;
        canRotate = true;
    }
}
