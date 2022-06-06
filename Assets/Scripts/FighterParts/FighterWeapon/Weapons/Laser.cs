using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Laser : FighterWeapon, IWeapon
{
    [Header("References")]
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Transform rayOrigin = null;
    [SerializeField] private ParticleSystem endPointParticles = null;

    [Header("Laser settings")]
    [SerializeField] private float chargeTime = 1f;
    [SerializeField] private float laserTime = 2f;
    [SerializeField] private float damageTime = 0.1f;
    [SerializeField] private float knockbackForceSelf = 15f;
    [SerializeField] private float knockbackForceHit = 15f;

    [Header("Line settings")]
    [SerializeField, Range(2, 10)] private int lineSegmentCount = 5;
    [SerializeField] private float maxLineDistance = 30f;
    [SerializeField] private LayerMask lineHitLayers;

    [Header("Reflections")]
    [SerializeField] private bool canReflect = true;
    [SerializeField] private string reflectableSurfaces = "Mirror";
    [SerializeField] private LineRenderer reflectionLineRenderer;

    [Header("Step settings")]
    [SerializeField] private float stepSpeed = 2f;
    [SerializeField] private float maxJumpDistance = 0.5f;

    [Header("Events")]
    [SerializeField] private UnityEvent OnChargeLaser;
    [SerializeField] private UnityEvent OnStartLaser;
    [SerializeField] private UnityEvent OnStopLaser;

    private List<Vector3> lineSegments = new List<Vector3>();
    private List<Vector3> lineOffsets = new List<Vector3>();

    private List<Vector3> reflectionLineSegments = new List<Vector3>();
    private List<Vector3> reflectionLineOffsets = new List<Vector3>();

    private float nextJumpTime = 0;
    private float nextDamageTime = 0;

    private bool isFiring = false;
    private bool isCharging = false;

    AimAssist aimassist = new AimAssist();

    public override void ActivateWeapon(InputAction.CallbackContext context)
    {
        if (context.action.WasPerformedThisFrame())
        {
            ChargeLaser();
        }
        base.ActivateWeapon(context);
    }

    private void Start()
    {
        nextJumpTime = stepSpeed;

        lineRenderer.positionCount = lineSegmentCount;
        reflectionLineRenderer.positionCount = lineSegmentCount;

        lineSegments.Clear();
        reflectionLineSegments.Clear();

        for (int i = 0; i < lineSegmentCount; i++)
        {
            lineOffsets.Add(Vector3.zero);
            lineSegments.Add(Vector3.Lerp(lineRenderer.GetPosition(0), lineRenderer.GetPosition(lineRenderer.positionCount - 1) + Random.insideUnitSphere * maxJumpDistance, 1f / (float)lineSegmentCount * (float)i));
            reflectionLineOffsets.Add(Vector3.zero);
            reflectionLineSegments.Add(Vector3.Lerp(reflectionLineRenderer.GetPosition(0), reflectionLineRenderer.GetPosition(reflectionLineRenderer.positionCount - 1) + Random.insideUnitSphere * maxJumpDistance, 1f / (float)lineSegmentCount * (float)i));
        }

        lineRenderer.SetPositions(lineSegments.ToArray());
        reflectionLineRenderer.SetPositions(reflectionLineSegments.ToArray());

        SetMainEndPoint(transform.forward * 10f);
    }

    [ContextMenu("Charge laser")]
    public void ChargeLaser()
    {
        if (isCharging || isFiring) return;
        OnChargeLaser.Invoke();
        isCharging = true;
        Invoke(nameof(StartLaser), chargeTime);
    }

    private void StartLaser()
    {
        isFiring = true;
        isCharging = false;
        lineRenderer.enabled = true;
        OnStartLaser.Invoke();
        fighterRigidBody.velocity -= knockbackForceSelf * Mathf.Abs(Physics.gravity.y) * transform.forward;
        Invoke(nameof(StopLaser), laserTime);
        if (fighterRoot) fighterRoot.onAttack();
        OnAttack.Invoke();
    }

    [ContextMenu("Stop laser")]
    public void StopLaser()
    {
        lineRenderer.enabled = false;
        reflectionLineRenderer.enabled = false;
        endPointParticles.Stop();
        OnStopLaser.Invoke();
        isFiring = false;
    }

    public void SetOrigin(Vector3 origin)
    {
        lineSegments[0] = origin;
    }

    public void SetMainEndPoint(Vector3 endPoint)
    {
        lineSegments[^1] = endPoint;
    }

    private void Update()
    {
        aimassist.StartAimAssist(transform, fighterRoot, 20, 20, 45);

        if (!isFiring) return;
        SetOrigin(rayOrigin.position);

        if (Physics.Raycast(rayOrigin.position, rayOrigin.forward, out RaycastHit hit, maxLineDistance, lineHitLayers, QueryTriggerInteraction.Ignore))
        {

            SetMainEndPoint(hit.point);

            if (IsReflected(ref hit))
            {
                reflectionLineRenderer.enabled = true;
            }
            else
            {
                reflectionLineRenderer.enabled = false;
            }


            if (endPointParticles)
            {
                endPointParticles.transform.SetPositionAndRotation(hit.point, Quaternion.LookRotation(hit.normal));
                if (!endPointParticles.isPlaying) endPointParticles.Play();
            }
            else if (endPointParticles != null && endPointParticles.isPlaying) endPointParticles.Stop();

            if (hit.transform.GetComponentInParent<Fighter>())
            {
                Fighter otherFighter = hit.transform.GetComponentInParent<Fighter>();
                otherFighter.GetRigidBody().velocity += knockbackForceHit * Mathf.Abs(Physics.gravity.y) * Time.deltaTime * transform.forward;
                if (Time.time > nextDamageTime)
                {

                    otherFighter.TakeDamage(damage, fighterRoot);
                    nextDamageTime = Time.time + damageTime;
                }
            }

            if (Time.time >= nextJumpTime)
            {
                nextJumpTime = Time.time + stepSpeed;
                StepSegments();
            }

            DrawLine();
        }
    }

    private bool IsReflected(ref RaycastHit hit)
    {
        if (hit.transform.CompareTag(reflectableSurfaces))
        {
            reflectionLineSegments[0] = hit.point;
            if(Physics.Raycast(hit.point, Vector3.Reflect(rayOrigin.forward, hit.normal), out hit, maxLineDistance, lineHitLayers, QueryTriggerInteraction.Ignore)){

                reflectionLineSegments[^1] = hit.point;
                return true;
            }
        }

        return false;
    }

    private void DrawLine()
    {
        for (int i = 0; i < lineSegmentCount; i++)
        {
            if (i == 0 || i == lineSegmentCount - 1) continue;
            lineSegments[i] = Vector3.Lerp(lineSegments[0], lineSegments[^1], 1f / ((float)lineSegmentCount - 1) * (float)i) + (lineOffsets[i] * maxJumpDistance);
            reflectionLineSegments[i] = Vector3.Lerp(reflectionLineSegments[0], reflectionLineSegments[^1], 1f / ((float)lineSegmentCount - 1) * (float)i) + (reflectionLineOffsets[i] * maxJumpDistance);
        }
        lineRenderer.SetPositions(lineSegments.ToArray());
        reflectionLineRenderer.SetPositions(reflectionLineSegments.ToArray());
    }

    private void StepSegments()
    {
        for (int i = 0; i < lineSegmentCount; i++)
        {
            Vector3 randomDirection = Random.insideUnitSphere;
            randomDirection.z = 0.2f;
            Vector3 localOffset = transform.InverseTransformDirection(randomDirection);

            lineOffsets[i] = localOffset;
            reflectionLineOffsets[i] = localOffset;
        }
    }
}
