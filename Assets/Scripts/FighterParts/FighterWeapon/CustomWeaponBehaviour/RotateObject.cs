using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    public static RotateObject instance;

    void Start()
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
        Vector3 startRotation = rotateObject.transform.localEulerAngles;
        float currentRotateTime = 0;     
        while(currentRotateTime < rotateTime)
        {
            currentRotateTime += Time.deltaTime;
            rotateObject.transform.localEulerAngles = Vector3.Lerp(startRotation, rotationPointEnd, currentRotateTime / rotateTime);
            yield return new WaitForEndOfFrame();
        }
    }
}
