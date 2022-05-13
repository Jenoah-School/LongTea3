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
    [SerializeField] private float maxRotationSpeed = 3f;
    [SerializeField, Range(0f,1f)] private float driftDrag = 0.3f;
    [SerializeField] private bool isGrounded = false;

    [Header("Grounded state")]
    [SerializeField] private Transform groundedTransform = null;
    [SerializeField] private float groundedCheckLength = 0.1f;
    [SerializeField] private bool useWorldDirection = true;
    [SerializeField] private LayerMask groundedLayers = Physics.AllLayers;

    [Header("Smoothing")]
    [SerializeField] private float inputSmoothing = 10f;
    [SerializeField] private float rotationSmoothing = 8f;

    [Header("References")]
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Transform centerOfMass;

    private Rigidbody rb = null;

    private Vector2 movementVector = Vector2.zero;
    private float movementInput = 0;
    private Vector2 rotationVector = Vector2.zero;
    private Quaternion targetRotation = Quaternion.identity;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if(centerOfMass) rb.centerOfMass = centerOfMass.localPosition;
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
        float moveDot = Vector3.Dot(transform.forward, rb.velocity.normalized);

        float moveMultiplier = movementInput >= 0 ? movementInput : movementInput * backwardsMultiplier;
        movementVector = Vector2.Lerp(movementVector, moveInput * moveMultiplier, inputSmoothing * Time.deltaTime);
        //transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSmoothing * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (IsGrounded())
        {
            Rotate();
            Move();
            RemoveDrift();
        }
    }

    private void Rotate()
    {
        //if(playerInput.currentControlScheme != "PC")
        //{
        float rotationMultiplier = 0f;
        if (Mathf.Abs(rb.angularVelocity.y) < maxRotationSpeed)
        {
            rotationMultiplier = movementInput >= 0 ? Mathf.Max(minRotationSpeed, Mathf.Abs(movementInput)) : Mathf.Max(minRotationSpeed, Mathf.Abs(movementInput) * backwardsMultiplier);
        }

        //targetRotation.eulerAngles += new Vector3(0, rotationVector.x * rotationSpeed, 0) * rotationMultiplier;
        rb.MoveRotation(rb.rotation * Quaternion.Euler(rotationMultiplier * rotationSpeed * rotationVector.x * transform.up));
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

    private void RemoveDrift()
    {
        Vector3 localVelocity = transform.InverseTransformDirection(rb.velocity);
        localVelocity.x *= 1f - driftDrag;
        rb.velocity = transform.TransformDirection(localVelocity);
    }

    public bool IsGrounded()
    {
        isGrounded = Physics.Raycast(groundedTransform.position, useWorldDirection ? Vector3.down : -groundedTransform.up, groundedCheckLength, groundedLayers);
        return isGrounded;
    }
}

