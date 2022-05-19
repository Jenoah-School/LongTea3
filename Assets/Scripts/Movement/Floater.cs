using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floater : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float distance = 1f;

    private Vector3 offset = Vector3.zero;
    private float elapsedTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime * speed;
        transform.position = offset + new Vector3(0, Mathf.Sin(elapsedTime) * distance, 0);
    }
}
