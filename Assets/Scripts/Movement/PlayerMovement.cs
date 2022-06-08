using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Speed")]
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField, Range(0f, 1f)] private float minRotationSpeed = 0.2f;
    [SerializeField] private float backwardsMultiplier = 0.5f;
    [SerializeField] private float maxRotationSpeed = 3f;
    [SerializeField] private bool isGrounded = false;

    [Header("Grounded state")]
    [SerializeField, Tooltip("The fallback of the origin for the grounded checkbox")] private Transform groundedTransform = null;
    [SerializeField] private Vector3 groundedCheckBox = Vector3.one;
    [SerializeField] private LayerMask groundedLayers = Physics.AllLayers;

    [Header("Smoothing")]
    [SerializeField] private float inputSmoothing = 10f;
    [SerializeField] private float rotationSmoothing = 8f;

    [Header("Audio")]
    [SerializeField] private AnimationCurve volumeCurve;
    [SerializeField] private AnimationCurve pitchCurve;
    [SerializeField] private AudioSource engineAudioSource;

    [Header("References")]
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private bool canMove = true;

    private Rigidbody rb = null;

    //Speeds
    private float accelerationSpeed = 60f;
    private float maximumSpeed = 10f;

    //Drag
    private float brakeDrag = .2f;
    private float driftDrag = 0.3f;
    private float airDrag = 0.4f;
    private float airVerticalDrag = 0.2f;

    private Vector2 movementVector = Vector2.zero;
    private float movementInput = 0;
    private float targetSpeed = 0f;
    private float moveMultiplier = 1f;
    private Vector2 rotationVector = Vector2.zero;
    private Quaternion targetRotation = Quaternion.identity;

    private bool wasGrounded = false;

    public delegate void OnTouchGround();
    public OnTouchGround onTouchGround;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        onTouchGround += SetTargetRotationToCurrentRotation;
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

        moveMultiplier = movementInput >= 0 ? movementInput : movementInput * backwardsMultiplier;

        movementVector = Vector2.Lerp(movementVector, moveInput * moveMultiplier, inputSmoothing * Time.deltaTime);

        if (engineAudioSource)
        {
            float currentSpeed = rb.velocity.magnitude;
            engineAudioSource.pitch = pitchCurve.Evaluate(currentSpeed / maximumSpeed);
            engineAudioSource.volume = volumeCurve.Evaluate(currentSpeed / maximumSpeed);
        }
    }

    private void FixedUpdate()
    {
        if (IsGrounded())
        {
            if (canMove)
            {
                if (playerInput.currentControlScheme == "PC")
                {
                    Rotate();
                    Move();
                }
                else
                {
                    MoveAndRotate();
                }
            }
            ApplyGroundedDrag();
        }
        else
        {
            ApplyAirDrag();
        }
    }

    #region PC Controls

    private void Rotate()
    {
        float rotationMultiplier = 0f;
        if (Mathf.Abs(rb.angularVelocity.y) < maxRotationSpeed)
        {
            rotationMultiplier = movementInput >= 0 ? Mathf.Max(minRotationSpeed, Mathf.Abs(movementInput)) : Mathf.Max(minRotationSpeed, Mathf.Abs(movementInput) * backwardsMultiplier);
        }

        //targetRotation.eulerAngles += new Vector3(0, rotationVector.x * rotationSpeed, 0) * rotationMultiplier;
        rb.MoveRotation(rb.rotation * Quaternion.Euler(rotationMultiplier * rotationSpeed * rotationVector.x * transform.up));
    }


    private void Move()
    {
        Vector3 moveDirection = new Vector3(movementVector.x, 0, movementVector.y);

        moveDirection = moveDirection.sqrMagnitude > 1 ? moveDirection.normalized : moveDirection;

        targetSpeed = (brakeDrag * (movementInput == 0 ? 1f : 0f));

        rb.velocity += accelerationSpeed * Time.fixedDeltaTime * moveDirection;
    }

    #endregion

    #region Gamepad Controls

    private void MoveAndRotate()
    {
        //Rotation
        Vector3 lookDirection = new Vector3(rotationVector.x, 0, rotationVector.y);

        lookDirection = lookDirection.sqrMagnitude > 1 ? lookDirection.normalized : lookDirection;

        if (lookDirection.magnitude > 0.1f) { targetRotation = Quaternion.LookRotation(lookDirection.normalized, Vector3.up); }

        rb.MoveRotation(Quaternion.Lerp(transform.rotation, targetRotation, rotationSmoothing * Time.deltaTime));


        //Position
        Vector3 moveDirection = new Vector3(transform.forward.x, 0, transform.forward.z);
        moveDirection = moveDirection.sqrMagnitude > 1 ? moveDirection.normalized : moveDirection;
        rb.velocity += accelerationSpeed * lookDirection.magnitude * Time.fixedDeltaTime * moveDirection;

        targetSpeed = (brakeDrag * (lookDirection.magnitude == 0 ? 1f : 0f));
    }

    private void SetTargetRotationToCurrentRotation()
    {
        targetRotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
    }

    #endregion

    private void ApplyGroundedDrag()
    {
        Vector3 localVelocity = transform.InverseTransformDirection(rb.velocity);
        localVelocity.x *= 1f - driftDrag;
        localVelocity.z *= 1f - targetSpeed;
        localVelocity.z = Mathf.Clamp(localVelocity.z, -maximumSpeed, maximumSpeed);
        rb.velocity = transform.TransformDirection(localVelocity);
    }

    private void ApplyAirDrag()
    {
        Vector3 airedVelocity = rb.velocity;
        airedVelocity.x *= 1f - airDrag;
        airedVelocity.y *= 1f - airVerticalDrag;
        airedVelocity.z *= 1f - airDrag;
        rb.velocity = airedVelocity;
    }

    public bool IsGrounded()
    {
        if (Physics.CheckBox(groundedTransform.position, groundedCheckBox / 2, transform.rotation, groundedLayers) && Mathf.Abs(rb.velocity.y) < 2)
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }

        if (wasGrounded == false && isGrounded == true)
        {
            onTouchGround();
        }

        wasGrounded = isGrounded;
        return isGrounded;
    }

    public void SetMoveState(bool newMoveState)
    {
        canMove = newMoveState;
    }

    public void SetMoveSpeed(float newMoveSpeed)
    {
        maximumSpeed = newMoveSpeed;
    }

    public void SetAccelerationSpeed(float newAccelerationSpeed)
    {
        accelerationSpeed = newAccelerationSpeed;
    }

    public void SetDrag(float brakeDrag = -1, float driftDrag = -1f, float airDrag = -1f, float airVerticalDrag = -1f)
    {
        if (brakeDrag != -1) this.brakeDrag = brakeDrag;
        if (driftDrag != -1) this.driftDrag = driftDrag;
        if (airDrag != -1) this.airDrag = airDrag;
        if (airVerticalDrag != -1) this.airVerticalDrag = airVerticalDrag;
    }

    public void SetGroundedTransform(Transform groundedCheckTransform)
    {
        groundedTransform = groundedCheckTransform;
    }

    private void OnDrawGizmosSelected()
    {
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(groundedTransform.position, transform.rotation, transform.lossyScale);
        Gizmos.matrix = rotationMatrix;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(Vector3.zero, groundedCheckBox);
    }
}

