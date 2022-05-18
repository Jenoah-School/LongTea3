using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RoundManager : MonoBehaviour
{
    public static RoundManager singleton;
    [SerializeField] private UnityEvent OnEndRound;

    private void Awake()
    {
            singleton = this;
    }

    public void EndRound()
    {
        OnEndRound.Invoke();
    }
}
