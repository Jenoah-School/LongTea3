using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class FadeUI : MonoBehaviour
{
    private CanvasGroup canvasGroup = null;
    [SerializeField] private bool toggleInteractability = true;
    [SerializeField] private bool toggleRaycastBlocking = true;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) enabled = false;
    }

    public void FadeOut(float speed)
    {
        canvasGroup.DOFade(0f, speed).SetUpdate(true);
        if(toggleInteractability) canvasGroup.interactable = false;
        if (toggleRaycastBlocking) canvasGroup.blocksRaycasts = false;
    }

    public void FadeIn(float speed)
    {
        canvasGroup.DOFade(1f, speed).SetUpdate(true);
        if(toggleInteractability) canvasGroup.interactable = true;
        if (toggleRaycastBlocking) canvasGroup.blocksRaycasts = true;
    }
}
