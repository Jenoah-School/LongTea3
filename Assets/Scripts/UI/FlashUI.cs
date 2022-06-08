using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class FlashUI : MonoBehaviour
{
    [SerializeField] private float flashSpeed = .3f;
    [SerializeField] private Image flashImage = null;
    [SerializeField] private AnimationCurve alphaCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));

    private void OnValidate()
    {
        if (flashImage == null && GetComponent<Image>() != null) flashImage = GetComponent<Image>();
    }

    public void Flash()
    {
        StopCoroutine(FlashRoutine());
        StartCoroutine(FlashRoutine());
    }

    IEnumerator FlashRoutine()
    {
        float elapsedTime = 0f;
        Color alphaColor = flashImage.color;
        while (elapsedTime < flashSpeed)
        {
            elapsedTime += Time.deltaTime;
            alphaColor.a = alphaCurve.Evaluate(elapsedTime / flashSpeed);
            flashImage.color = alphaColor;
            yield return new WaitForEndOfFrame();
        }
        alphaColor.a = alphaCurve.Evaluate(1f);
        flashImage.color = alphaColor;
    }
}
