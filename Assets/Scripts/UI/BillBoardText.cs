using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BillBoardText : MonoBehaviour
{
    Transform targetCamera;

    [SerializeField] private bool highQuality = false;
    [SerializeField] private bool inverse = false;

    void Start()
    {
        targetCamera = Camera.main.transform;
    }

    void Update()
    {
        if (highQuality)
        {
            Vector3 relativePos;

            if (inverse)
            {
                relativePos = targetCamera.position - transform.position;
            }
            else
            {
                relativePos = transform.position - targetCamera.position;
            }

            Quaternion rotation = Quaternion.LookRotation(relativePos, targetCamera.up);
            transform.rotation = rotation;
        }
        else
        {
            if (inverse)
            {
                transform.forward = -targetCamera.forward;
            }
            else
            {
                transform.forward = targetCamera.forward;
            }
        }
    }
}
