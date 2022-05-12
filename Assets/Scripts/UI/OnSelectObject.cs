using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class OnSelectObject : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [SerializeField] private UnityEvent OnSelectEvent;
    [SerializeField] private UnityEvent OnDeselectEvent;

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
}
