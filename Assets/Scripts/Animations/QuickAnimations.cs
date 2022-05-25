using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Lean.Pool;
using UnityEngine.UI;
using UnityEngine.Events;

public class QuickAnimations : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MeshRenderer[] meshRenderers;
    [SerializeField] private Collider[] colliders;
    [SerializeField] private GameObject rootObject = null;

    [Header("Toggles")]
    [SerializeField] private bool disableCollisionOnGrow = true;
    [SerializeField] private bool destroyAfterFade = true;

    [Header("Targets")]
    [SerializeField] private float growTarget = 2.5f;
    [SerializeField] private float shrinkTarget = 0.5f;
    [SerializeField] private float squishTarget = 0.1f;
    [SerializeField] private float shakeStrength = .5f;
    [SerializeField] private Vector3 targetOffset = new Vector3(0f, 20f, 0f);
    [SerializeField, ColorUsage(true, true)] private Color colorTarget = Color.white;

    [Header("Continues pulsing size")]
    [SerializeField] private bool isPulsingSize;
    [SerializeField] private float scaleDifference = 0.2f;
    [SerializeField] private float pulseSizeSpeed = 4f;

    [Header("Continues pulsing rotation")]
    [SerializeField] private bool isPulsingRotation;
    [SerializeField] private float rotationAngleDifference = 30f;
    [SerializeField] private float pulseRotationSpeed = 4f;
    [SerializeField] private Vector3 affectedAngle = Vector3.one;

    [Header("Ground settings")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float maxGroundDistance = 100f;
    [SerializeField] private Vector3 groundOffset = Vector3.up;
    [SerializeField] private UnityEvent onHitGround;

    private List<Material> materials = new List<Material>();
    private List<Color> defaultMaterialColor = new List<Color>();
    private Image image = null;
    private Color startColor;

    //Pulse
    private Vector3 startScale = Vector3.one;
    private Vector3 startRotation = Vector3.zero;
    private float currentPulseSizeTime;
    private float currentPulseRotationTime;


    private void Start()
    {
        if (meshRenderers.Length > 0)
        {
            for (int i = 0; i < meshRenderers.Length; i++)
            {
                for (int j = 0; j < meshRenderers[i].materials.Length; j++)
                {
                    materials.Add(meshRenderers[i].materials[j]);
                    defaultMaterialColor.Add(meshRenderers[i].materials[j].color);
                }
            }
        }

        if (gameObject.TryGetComponent(out image)) startColor = image.color;
        startRotation = transform.localEulerAngles;
    }

    private void Update()
    {
        if (isPulsingSize)
        {
            currentPulseSizeTime += Time.deltaTime * pulseSizeSpeed;
            transform.localScale = startScale * ((Mathf.Sin(currentPulseSizeTime) * scaleDifference) + 1f);
        }

        if (isPulsingRotation)
        {
            currentPulseRotationTime += Time.deltaTime * pulseRotationSpeed;
            transform.localEulerAngles = startRotation + affectedAngle * ((Mathf.Sin(currentPulseRotationTime) * rotationAngleDifference) + 1f);
        }
    }

    public void Squish(float speed)
    {
        transform.DOPunchScale(-Vector3.one * squishTarget, speed, 10, 0.25f);
    }

    public void SetImageColor(float speed)
    {
        if (image != null)
        {
            image.DOColor(colorTarget, speed);
        }
    }

    public void OffsetObject(float speed)
    {
        transform.DOMove(transform.position + transform.TransformPoint(targetOffset), speed, false);
    }

    public void MoveToGround(float speed)
    {
        StartCoroutine(MoveToGroundWithEvent(speed));
    }

    IEnumerator MoveToGroundWithEvent(float speed)
    {
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, maxGroundDistance, groundLayer))
        {
            transform.DOMove(hit.point + transform.TransformDirection(groundOffset), speed, false);
            yield return new WaitForSeconds(speed);
            onHitGround.Invoke();
        }
    }

    public void FlashMaterial(float speed)
    {
        StopCoroutine(FlashMaterialEnum(speed));
        StartCoroutine(FlashMaterialEnum(speed));
    }

    IEnumerator FlashMaterialEnum(float speed)
    {
        SetMaterialColor(speed / 2f);
        yield return new WaitForSeconds(speed / 2f);
        ResetMaterialColor(speed / 2f);
    }

    public void SetMaterialColor(float speed)
    {
        foreach (Material material in materials)
        {
            material.DOColor(colorTarget, speed);
        }
    }

    public void SetMaterialEmission(float speed)
    {
        foreach (Material material in materials)
        {
            StartCoroutine(LerpMaterialEmission(material, colorTarget, speed));
        }
    }

    public void ResetMaterialColor(float speed)
    {
        for (int i = 0; i < materials.Count; i++)
        {
            materials[i].DOColor(defaultMaterialColor[i], speed);
        }
    }

    public void ResetImageColor(float speed)
    {
        if (image != null)
        {
            image.DOColor(startColor, speed);
        }
    }

    public void Grow(float speed)
    {
        transform.DOScale(growTarget, speed);
        if (disableCollisionOnGrow)
        {
            foreach (Collider collider in colliders)
            {
                collider.enabled = false;
            }
        }
    }

    public void Shrink(float speed)
    {
        transform.DOScale(shrinkTarget, speed);
        if (disableCollisionOnGrow)
        {
            foreach (Collider collider in colliders)
            {
                collider.enabled = false;
            }
        }
    }

    public void Shake(float duration)
    {
        transform.DOShakePosition(duration, shakeStrength);
    }

    private void OnDisable()
    {
        ResetAlphaImmediate();
    }

    private void OnDestroy()
    {
        ResetAlphaImmediate();
    }

    public void ResetAlphaImmediate()
    {
        foreach (Material material in materials)
        {
            if (material.HasProperty("_Color"))
            {
                Color tempColor = material.color;
                tempColor.a = 1f;
                material.color = tempColor;
            }
        }
    }

    public void FadeOut(float speed)
    {
        foreach (Material material in materials)
        {
            material.DOFade(0f, speed);
        }

        if (destroyAfterFade)
        {
            if (LeanPool.Links.ContainsKey(gameObject))
            {
                LeanPool.Despawn(rootObject, speed + 0.1f);
            }
            else
            {
                Destroy(rootObject, speed + 0.1f);
            }
        }
    }

    IEnumerator LerpMaterialEmission(Material materialToFade, Color targetColor, float speed)
    {
        Color currentColor = materialToFade.GetColor("_EmissionColor");
        float elapsedTime = 0f;

        while (elapsedTime < speed)
        {
            elapsedTime += Time.deltaTime;
            materialToFade.SetColor("_EmissionColor", Color.Lerp(currentColor, targetColor, elapsedTime / speed));
            yield return new WaitForEndOfFrame();
        }

        materialToFade.SetColor("_EmissionColor", targetColor);
    }
}
