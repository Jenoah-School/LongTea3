using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToTarget : MonoBehaviour
{
    [SerializeField] private float rotationTime = 0.4f;

    private Vector3 targetRotation = Vector3.zero;

    private void Start()
    {
        targetRotation = transform.eulerAngles;
    }

    public void IncreaseX(float incrementAmount)
    {
        targetRotation.x += incrementAmount;
    }

    public void IncreaseY(float incrementAmount)
    {
        targetRotation.y += incrementAmount;
    }

    public void IncreaseZ(float incrementAmount)
    {
        targetRotation.z += incrementAmount;
    }

    public void Rotate()
    {
        StopAllCoroutines();
        StartCoroutine(RotateToTargetEnum());
    }

    private IEnumerator RotateToTargetEnum()
    {
        Quaternion startRotation = transform.rotation;
        float elapsedTime = 0f;
        while (elapsedTime < rotationTime)
        {
            elapsedTime += Time.deltaTime;
            transform.rotation = Quaternion.Lerp(startRotation, Quaternion.Euler(targetRotation), elapsedTime / rotationTime);
            yield return new WaitForEndOfFrame();
        }
        transform.rotation = Quaternion.Euler(targetRotation);
    }
}
