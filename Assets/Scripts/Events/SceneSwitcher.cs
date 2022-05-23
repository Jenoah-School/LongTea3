using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    [SerializeField] private Animator fadeAnimator;
    [SerializeField] private float fadeDuration = 1.25f;
    [SerializeField] private UnityEvent OnSwitchScene;

    private bool isTransitioning = false;

    public void SwitchScene(int buildIndex)
    {
        if (!fadeAnimator.gameObject.activeSelf) fadeAnimator.gameObject.SetActive(true);
        if (!fadeAnimator.enabled) fadeAnimator.enabled = true;
        if (!isTransitioning)
        {
            isTransitioning = true;
            StartCoroutine(SwitchWithFade(buildIndex));
            OnSwitchScene.Invoke();
        }
    }

    private IEnumerator SwitchWithFade(int buildIndex)
    {
        fadeAnimator.StopPlayback();
        fadeAnimator.Play("FadeIn");
        yield return new WaitForSecondsRealtime(fadeDuration);
        Time.timeScale = 1;
        SceneManager.LoadSceneAsync(buildIndex, LoadSceneMode.Single);
    }

    private IEnumerator SwitchWithFade(string sceneName)
    {
        fadeAnimator.StopPlayback();
        fadeAnimator.Play("FadeIn");
        yield return new WaitForSecondsRealtime(fadeDuration);
        Time.timeScale = 1;
        SceneManager.LoadSceneAsync(SceneManager.GetSceneByName(sceneName).buildIndex, LoadSceneMode.Single);
    }

    public void InstantSwitch(int buildIndex)
    {
        Time.timeScale = 1;
        SceneManager.LoadSceneAsync(buildIndex, LoadSceneMode.Single);
    }

    public void InstantSwitch(string sceneName)
    {
        Time.timeScale = 1;
        SceneManager.LoadSceneAsync(SceneManager.GetSceneByName(sceneName).buildIndex, LoadSceneMode.Single);
    }

    public void ReloadScene()
    {
        if (!fadeAnimator.enabled) fadeAnimator.enabled = true;
        if (!isTransitioning)
        {
            isTransitioning = true;
            StartCoroutine(SwitchWithFade(SceneManager.GetActiveScene().buildIndex));
            OnSwitchScene.Invoke();
        }
    }

    public void InstantReloadScene()
    {
        InstantSwitch(SceneManager.GetActiveScene().buildIndex);
    }    
}
