using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Fade3DText : MonoBehaviour
{
    [SerializeField] private Color targetColor = Color.white;
    [Header("Start fade")]
    [SerializeField] private bool startFadeOnSpawn = false;
    [SerializeField] private bool destroyOnFinish = false;
    [SerializeField] private float startFadeDelay = 2f;
    [SerializeField] private float startFadeDuration = 1f;

    [Header("Pulsing")]
    [SerializeField] private bool pulseTextAlpha = false;
    [SerializeField] private float minPulseAlpha = 30f;
    [SerializeField] private float pulseSpeed = 5f;

    private TMPro.TextMeshPro textObject;
    private Color defaultColor = Color.white;

    // Start is called before the first frame update
    void Start()
    {
        textObject = GetComponent<TMPro.TextMeshPro>();
        if (textObject == null) enabled = false;
        defaultColor = textObject.color;

        if (startFadeOnSpawn)
        {
            StartCoroutine(FadeDelayed(startFadeDelay, startFadeDuration));
        }
    }

    private void Update()
    {
        if (pulseTextAlpha)
        {
            PulseAlpha();
        }
    }

    public IEnumerator FadeDelayed(float fadeDelay, float fadeDuration)
    {
        yield return new WaitForSeconds(fadeDelay);
        FadeOut(fadeDuration);
    }

    public void FadeIn(float fadeSpeed)
    {
        StopAllCoroutines();
        StartCoroutine(FadeColor(1f, fadeSpeed));
    }

    public void FadeToTarget(float fadeSpeed)
    {
        textObject.DOColor(targetColor, fadeSpeed).SetUpdate(true);
    }
    
    public void FadeToDefault(float fadeSpeed)
    {
        textObject.DOColor(defaultColor, fadeSpeed).SetUpdate(true);
    }

    public void FadeOut(float fadeSpeed)
    {
        StopAllCoroutines();
        StartCoroutine(FadeColor(0f, fadeSpeed));
    }

    public void StartFadeOut()
    {
        StartCoroutine(FadeDelayed(startFadeDelay, startFadeDuration));
    }

    public void StopFadeOut()
    {
        StopAllCoroutines();
    }

    public void PulseAlpha()
    {
        float targetAlpha = 1f / 255f * (minPulseAlpha + (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f * (255f - minPulseAlpha));
        textObject.alpha = targetAlpha;
    }
    
    IEnumerator FadeColor(float targetAlpha, float fadeSpeed)
    {
        float currentAlpha = textObject.alpha;
        float elapsedTime = 0f;

        while(elapsedTime < fadeSpeed)
        {
            elapsedTime += Time.unscaledDeltaTime;
            textObject.alpha = Mathf.SmoothStep(currentAlpha, targetAlpha, elapsedTime / fadeSpeed);
            yield return new WaitForEndOfFrame();
        }

        if (destroyOnFinish) Destroy(this.gameObject);

        textObject.alpha = targetAlpha;
    }
}
