using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreventSplashOnFirstBoot : MonoBehaviour
{
    [SerializeField] private float timerThreshold = 5f;
    [SerializeField] private Animator splashScreenAnimator = null;
    [SerializeField] private string animationName = "FadeOut";
    [SerializeField] private bool enableOtherwise = false;
    // Start is called before the first frame update
    void Start()
    {
       if(Time.unscaledTime <= timerThreshold)
        {
            splashScreenAnimator.Play(animationName, 0, 1f);
        }
        else if(enableOtherwise)
        {
            splashScreenAnimator.gameObject.SetActive(true);
        }
    }
}
