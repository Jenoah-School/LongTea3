using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Countdown : MonoBehaviour
{
    [SerializeField] private float startingTimeInSeconds = 5;
    [SerializeField] private TMPro.TextMeshProUGUI textLabel;
    [SerializeField] private UnityEvent OnChangeSecond = null;
    [SerializeField] private UnityEvent OnCountdownFinish = null;

    private float currentTimeLeft = 0;
    private int previousSecond = 0;


    public void StartCountdown()
    {
        currentTimeLeft = startingTimeInSeconds;
        previousSecond = Mathf.RoundToInt(currentTimeLeft);
        StartCoroutine(Count());
    }

    public void StopCountdown()
    {
        StopAllCoroutines();
    }

    IEnumerator Count()
    {
        while (currentTimeLeft > 0)
        {
            currentTimeLeft -= Time.deltaTime;
            textLabel.text = Mathf.RoundToInt(currentTimeLeft).ToString();
            if (Mathf.RoundToInt(currentTimeLeft) != previousSecond)
            {
                previousSecond = Mathf.RoundToInt(currentTimeLeft);
                OnChangeSecond.Invoke();
            }
            yield return new WaitForEndOfFrame();
        }
        OnCountdownFinish.Invoke();
    }
}
