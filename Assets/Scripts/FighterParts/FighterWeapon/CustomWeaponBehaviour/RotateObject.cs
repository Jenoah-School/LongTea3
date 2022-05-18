using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    public static RotateObject instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance);
            return;
        }
    }

    public Coroutine RotateObjectToAngle(GameObject rotateObject, Vector3 rotationPointEnd, float rotateTime)
    {
        return StartCoroutine(RotateObjectToAngleEnum(rotateObject, rotationPointEnd, rotateTime));      
    }    

    private IEnumerator RotateObjectToAngleEnum(GameObject rotateObject, Vector3 rotationPointEnd, float rotateTime)
    {
        Quaternion startRotation = rotateObject.transform.localRotation;
        float currentRotateTime = 0;     
        while(currentRotateTime < rotateTime)
        {
            currentRotateTime += Time.deltaTime;
            rotateObject.transform.localRotation = Quaternion.Lerp(startRotation, Quaternion.Euler(rotationPointEnd), currentRotateTime / rotateTime);
            yield return new WaitForEndOfFrame();
        }
        rotateObject.transform.localEulerAngles = rotationPointEnd;
    }
}
