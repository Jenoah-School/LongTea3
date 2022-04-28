using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Speed")]
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField, Range(0f, 1f)] private float minRotationSpeed = 0.2f;
    [SerializeField] private float backwardsMultiplier = 0.5f;

    [Header("Smoothing")]
    [SerializeField] private float inputSmoothing = 10f;
    [SerializeField] private float rotationSmoothing = 8f;

    [Header("References")]
    [SerializeField] private PlayerInput playerInput;

    private Rigidbody rb = null;

    private Vector2 movementVector = Vector2.zero;
    private float movementInput = 0;
    private Vector2 rotationVector = Vector2.zero;
    private Quaternion targetRotation = Quaternion.identity;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void UpdateMovementInput(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<float>();
    }

    public void UpdateRotationInput(InputAction.CallbackContext context)
    {
        rotationVector = context.ReadValue<Vector2>();
    }

    private void Update()
    {
        Vector2 moveInput = new Vector2(transform.forward.x, transform.forward.z);

        float moveMultiplier = movementInput >= 0 ? movementInput : movementInput * backwardsMultiplier;
        movementVector = Vector2.Lerp(movementVector, moveInput * moveMultiplier, inputSmoothing * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSmoothing * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        Rotate();
        Move();
    }

    private void Rotate()
    {
        //if(playerInput.currentControlScheme != "PC")
        //{
        float rotationMultiplier = movementInput >= 0 ? Mathf.Max(0.2f, Mathf.Abs(movementInput)) : Mathf.Max(0.2f, Mathf.Abs(movementInput) * backwardsMultiplier);
        targetRotation.eulerAngles += new Vector3(0, rotationVector.x * rotationSpeed, 0) * rotationMultiplier;
        //}
        //else
        //{
        //    Vector3 lookDirection = new Vector3(rotationVector.x, 0, rotationVector.y);

        //    lookDirection = lookDirection.sqrMagnitude > 1 ? lookDirection.normalized : lookDirection;

        //    if (lookDirection.magnitude > 0.1f) { targetRotation = Quaternion.LookRotation(lookDirection.normalized, Vector3.up); }
        //}
    }


    private void Move()
    {
        Vector3 moveDirection = new Vector3(movementVector.x, 0, movementVector.y);

        moveDirection = moveDirection.sqrMagnitude > 1 ? moveDirection.normalized : moveDirection;

        rb.velocity += moveSpeed * Time.fixedDeltaTime * moveDirection;
    }
}

