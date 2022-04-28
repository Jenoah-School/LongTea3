using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class EventOnKeypress : MonoBehaviour
{
    [SerializeField] private List<InputActionReference> keys = new List<InputActionReference>();
    [SerializeField] private UnityEvent OnKeyPress;

    // Update is called once per frame
    void Update()
    {
        foreach(InputActionReference key in keys)
        {
            if (key.action.WasPerformedThisFrame())
            {
                OnKeyPress.Invoke();
            }
        }
    }
}
