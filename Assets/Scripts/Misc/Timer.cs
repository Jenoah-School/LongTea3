using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Timer : MonoBehaviour
{
    [SerializeField] private float startingTimeInSeconds = 61;
    [SerializeField] private string prefix = "Time left: ";
    [SerializeField] private TMPro.TextMeshProUGUI timeLabel = null;
    [SerializeField] private UnityEvent OnFinishTimer;

    private float currentTimeLeft = 0;
    private bool timerFinished = false;

    // Start is called before the first frame update
    void Start()
    {
        timeLabel = GetComponent<TMPro.TextMeshProUGUI>();
        currentTimeLeft = startingTimeInSeconds;
        UpdateTextLabel();
        InvokeRepeating("UpdateTextLabel", 0.25f, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        if(currentTimeLeft > 0)
        {
            currentTimeLeft -= Time.deltaTime;
        }else if(currentTimeLeft <= 0 && !timerFinished)
        {
            timerFinished = true;
            OnFinishTimer.Invoke();
        }
    }

    public void UpdateTextLabel()
    {
        float seconds = Mathf.Floor(currentTimeLeft % 60);
        float minutes = Mathf.Floor(currentTimeLeft / 60);
        timeLabel.text = prefix + minutes.ToString("00") + ":" + seconds.ToString("00");
    }
}
