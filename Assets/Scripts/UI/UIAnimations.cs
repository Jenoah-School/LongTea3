using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(AudioSource))]
public class UIAnimations : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, ISelectHandler, IDeselectHandler, ISubmitHandler
{
    public bool isInteractable = true;

    [Header("Animation")]
    [SerializeField] private float animationTime = .15f;
    [SerializeField, Range(0f, 4f)] private float widthMultiplier = 1.5f;
    [SerializeField, Range(0f, 4f)] private float heightMultiplier = 1f;
    [SerializeField] private bool moveFromCenter = true;

    [Header("Movement")]
    [SerializeField] private Vector2 targetPosition = Vector2.zero;

    [Header("Audio")]
    [SerializeField] private AudioClip hoverSound = null;
    [SerializeField] private AudioClip clickDownSound = null;
    [SerializeField] private AudioClip clickUpSound = null;
    [SerializeField] private AudioClip fullClickSound = null;

    [Header("Actions")]
    public UnityEvent OnSuccesfulClick;
    public UnityEvent OnHoverEnter;
    public UnityEvent OnHoverExit;

    private CanvasGroup canvasGroup;
    private Vector2 startSize = new Vector2();
    private Vector2 startPosition = new Vector2();
    private RectTransform rectTransform = null;
    private AudioSource audioSource = null;

    private bool isSelected = false;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        startSize = rectTransform.sizeDelta;
        startPosition = rectTransform.anchoredPosition;
        audioSource = GetComponent<AudioSource>();
        if(!TryGetComponent(out canvasGroup))
        {
            canvasGroup = transform.GetComponentInParent<CanvasGroup>();
        }
    }

    #region Sizing

    public void GrowToSize()
    {
        Vector2 targetSize = startSize;
        targetSize.x /= widthMultiplier;
        targetSize.y /= heightMultiplier;

        Vector2 targetPosition = startPosition;
        targetPosition.x += (targetSize.x - startSize.x) / 2f;
        targetPosition.y += (targetSize.y - startSize.y) / 2f;

        StopAllCoroutines();
        StartCoroutine(ChangeSize(targetSize, targetPosition));
    }

    public void ReturnToStartSize()
    {
        StopAllCoroutines();
        StartCoroutine(ChangeSize(startSize, startPosition));
    }

    private IEnumerator ChangeSize(Vector2 targetSize, Vector2 targetPosition)
    {
        float elapsedTime = 0f;
        Vector2 currentSize = rectTransform.sizeDelta;
        Vector2 currentPosition = rectTransform.anchoredPosition;

        float normalizedTime = 0f;

        while (elapsedTime < animationTime)
        {
            elapsedTime += Time.unscaledDeltaTime;
            normalizedTime = Mathf.SmoothStep(0f, 1f, elapsedTime / animationTime);
            rectTransform.sizeDelta = Vector2.Lerp(currentSize, targetSize, normalizedTime);
            if (!moveFromCenter)
            {
                rectTransform.anchoredPosition = Vector2.Lerp(currentPosition, targetPosition, normalizedTime);
            }
            yield return new WaitForEndOfFrame();
        }
    }

    #endregion

    #region Pointer Events

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (canvasGroup != null && !canvasGroup.interactable || !isInteractable) return;
        GrowToSize();
        PlayClickHoverSound();
        OnHoverEnter.Invoke();
        isSelected = true;
    }

    public void SetInteractable(bool newState)
    {
        isInteractable = newState;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (canvasGroup != null && !canvasGroup.interactable || !isInteractable) return;
        ReturnToStartSize();
        OnHoverExit.Invoke();
        isSelected = false;
    }

    //Do the same as PointerEnter, but when using arrows/controller instead of mouse
    public void OnSelect(BaseEventData eventData)
    {
        if (canvasGroup != null && !canvasGroup.interactable || !isInteractable) return;
        GrowToSize();
        PlayClickHoverSound();
        OnHoverEnter.Invoke();
        isSelected = true;
    }

    //Do the same as PointerExit, but when using arrows/controller instead of mouse
    public void OnDeselect(BaseEventData eventData)
    {
        if (canvasGroup != null && !canvasGroup.interactable || !isInteractable) return;
        ReturnToStartSize();
        OnHoverExit.Invoke();
        isSelected = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (canvasGroup != null && !canvasGroup.interactable || !isInteractable) return;
        PlayClickUpSound();
        if (isSelected) OnSuccesfulClick.Invoke();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (canvasGroup != null && !canvasGroup.interactable || !isInteractable) return;
        PlayClickDownSound();
    }

    public void OnSubmit(BaseEventData eventData)
    {
        if (canvasGroup != null && !canvasGroup.interactable || !isInteractable) return;
        OnSuccesfulClick.Invoke();
    }

    #endregion

    #region Audio

    public void PlayClickHoverSound()
    {
        if (hoverSound != null)
        {
            audioSource.PlayOneShot(hoverSound);
        }
    }

    public void PlayClickDownSound()
    {
        if (clickDownSound != null)
        {
            audioSource.PlayOneShot(clickDownSound);
        }
    }

    public void PlayClickUpSound()
    {
        if (clickUpSound != null)
        {
            audioSource.PlayOneShot(clickUpSound);
        }
    }

    public void PlayClickFullClickound()
    {
        if (fullClickSound != null)
        {
            audioSource.PlayOneShot(fullClickSound);
        }
    }

    #endregion

    #region Movement

    public void MoveToPosition()
    {
        rectTransform.DOAnchorPos(targetPosition, animationTime);
    }

    public void MoveToStartPosition()
    {
        rectTransform.DOAnchorPos(startPosition, animationTime);
    }

    public void MoveToStartPositionImmediately()
    {
        if (rectTransform != null)
            rectTransform.anchoredPosition = startPosition;
    }

    public void MoveToTargetPositionImmediately()
    {
        if (rectTransform != null)
            rectTransform.anchoredPosition = targetPosition;
    }
    private IEnumerator MoveToPosition_IEnum(Vector2 targetPosition)
    {
        float elapsedTime = 0f;
        Vector2 currentPosition = rectTransform.anchoredPosition;

        while (elapsedTime < animationTime)
        {
            elapsedTime += Time.unscaledDeltaTime;
            rectTransform.anchoredPosition = Vector2.Lerp(currentPosition, targetPosition, Mathf.SmoothStep(0f, 1f, elapsedTime / animationTime));
            yield return new WaitForEndOfFrame();
        }
    }

    #endregion
}
