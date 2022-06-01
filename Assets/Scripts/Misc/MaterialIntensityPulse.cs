using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialIntensityPulse : MonoBehaviour
{
    private Material material;
    [SerializeField] private float intensityMargin;
    [SerializeField] private float pulseSpeed;

    // Start is called before the first frame update
    void Start()
    {
        material = GetComponent<Renderer>().sharedMaterial;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        material.SetColor("_EmissionColor", material.color * (Mathf.Sin(pulseSpeed * Time.time) + 1 ) / intensityMargin);
    }
}
