using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveInDirectionOverTime : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private bool canMove = true;

    private void Update()
    {
        if(canMove) transform.position += transform.forward * speed * Time.deltaTime;
    }
}
