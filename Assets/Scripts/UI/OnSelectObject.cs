using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class OnSelectObject : MonoBehaviour, ISelectHandler, IDeselectHandler, ICancelHandler
{
    [SerializeField] private UnityEvent OnSelectEvent;
    [SerializeField] private UnityEvent OnDeselectEvent;
    [SerializeField] private UnityEvent OnCancelEvent;

    [SerializeField] private GameObject selectOtherObject = null;

    public void OnSelect(BaseEventData eventData)
    {
        OnSelectEvent.Invoke();
        if (selectOtherObject) StartCoroutine(SelectOtherObject(eventData));
    }

    IEnumerator SelectOtherObject(BaseEventData eventData)
    {
        yield return new WaitForEndOfFrame();
        eventData.selectedObject = selectOtherObject;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        OnDeselectEvent.Invoke();
    }

    public void OnCancel(BaseEventData eventData)
    {
        OnCancelEvent.Invoke();
    }
}
