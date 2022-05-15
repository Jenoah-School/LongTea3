using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Lean.Pool;

public class FlipWhenTilted : MonoBehaviour
{
    [SerializeField] private float flipInitiationTime = 2f;
    [SerializeField, Range(0f, 1f)] private float flipThreshold = 0.4f;
    [SerializeField] private float rotateTime = 0.5f;
    [SerializeField] private UnityEvent OnFlip;

    [Header("Effects")]
    [SerializeField] private ParticleSystem particlesOnFlip;

    private bool isTilted = false;
    private bool isRotation = false;
    private PlayerMovement playerMovement;
    private float flipTime = 0f;
    private bool useGroundedState = false;

    private void Start()
    {
        if (playerMovement != null) useGroundedState = true;
    }

    // Update is called once per frame
    void Update()
    {
        float upDot = Vector3.Dot(transform.up, Vector3.up);
        if (upDot < 1f - flipThreshold && !isRotation)
        {
            if (!isTilted)
            {
                isTilted = true;
                flipTime = Time.time + flipInitiationTime;
            }
            if (isTilted && Time.time >= flipTime)
            {
                SetUpright();
                OnFlip.Invoke();
                if (particlesOnFlip) LeanPool.Spawn(particlesOnFlip.gameObject, transform.position, Quaternion.Euler(new Vector3(-90f, 0, 0)));
            }
        }
        else
        {
            isTilted = false;
        }
    }

    public void SetUpright()
    {
        isTilted = false;
        StopAllCoroutines();
        StartCoroutine(RotateTo(Quaternion.Euler(0f, transform.eulerAngles.y, 0f), rotateTime));
        Debug.Log("This is when it would flip");
    }

    IEnumerator RotateTo(Quaternion targetRotation, float rotateTime)
    {
        isRotation = true;
        float elapsedTime = 0f;
        Quaternion startRotation = transform.rotation;
        while (elapsedTime < rotateTime)
        {
            elapsedTime += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime / rotateTime);
            yield return new WaitForEndOfFrame();
        }
        transform.rotation = targetRotation;
        isRotation = false;
    }
}
