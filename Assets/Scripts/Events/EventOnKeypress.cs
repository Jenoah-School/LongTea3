using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class EventOnKeypress : MonoBehaviour
{
    [SerializeField] private List<InputActionReference> keys = new List<InputActionReference>();
    [SerializeField] private UnityEvent OnKeyPress;
    private PlayerInput playerInput;
    private PlayerControls playerControls;

    private void Start()
    {
        playerControls = new PlayerControls();
        playerInput = FindObjectOfType<PlayerInput>();
    }

    // Update is called once per frame
    void Update()
    {
        foreach(InputActionReference key in keys)
        {
            if(playerControls.Regular.PrimaryWeapon.WasPerformedThisFrame())
            {
                Debug.Log("YEEEAA");
                OnKeyPress.Invoke();
            }
        }
    }
}
