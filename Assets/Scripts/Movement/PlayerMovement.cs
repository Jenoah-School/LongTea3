using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float inputSmoothing = 10f;
    [SerializeField] private float rotationSmoothing = 8f;

    private Rigidbody rb = null;

    private Vector2 movementVector = Vector2.zero;
    private Vector2 inputVector = Vector2.zero;
    private Quaternion targetRotation = Quaternion.identity;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void UpdateMovementInput(InputAction.CallbackContext context)
    {
        inputVector = context.ReadValue<Vector2>();
    }

    private void Update()
    {
        movementVector = Vector2.Lerp(movementVector, inputVector, inputSmoothing * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSmoothing * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        Move();
    }


    private void Move()
    {
        Vector3 moveDirection = new Vector3(movementVector.x, 0, movementVector.y);

        moveDirection = moveDirection.sqrMagnitude > 1 ? moveDirection.normalized : moveDirection;

        if (moveDirection.magnitude > 0.1f) { targetRotation = Quaternion.LookRotation(moveDirection.normalized, Vector3.up); }

        rb.MovePosition(rb.position + moveSpeed * Time.fixedDeltaTime * moveDirection);
    }
}

