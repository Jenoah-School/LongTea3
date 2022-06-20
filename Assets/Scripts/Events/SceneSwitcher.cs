using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneSwitcher : MonoBehaviour
{
    [SerializeField] private Animator fadeAnimator;
    [SerializeField] private float fadeDuration = 1.25f;
    [SerializeField] private UnityEvent OnSwitchScene;
    [SerializeField] private Image backgroundImage = null;
    [SerializeField] private List<Sprite> backgroundImageSprites = new List<Sprite>();

    private bool isTransitioning = false;
    private int currentBackgroundImageIndex = 0;
    private float waitDuration = 0f;

    private void Awake()
    {
        currentBackgroundImageIndex = PlayerPrefs.GetInt("splashBackgroundImageIndex", 0);
        if (backgroundImageSprites.Count > currentBackgroundImageIndex && backgroundImage != null)
        {
            backgroundImage.sprite = backgroundImageSprites[currentBackgroundImageIndex];
        }
    }

    public void SetWaitDuration(float newWaitDuration)
    {
        waitDuration = newWaitDuration;
    }

    public void SetBackgroundImageSpriteIndex(int newIndex)
    {
        currentBackgroundImageIndex = newIndex;
        PlayerPrefs.SetInt("splashBackgroundImageIndex", currentBackgroundImageIndex);
        if (backgroundImageSprites.Count > currentBackgroundImageIndex && backgroundImage != null)
        {
            backgroundImage.sprite = backgroundImageSprites[currentBackgroundImageIndex];
        }
    }

    public void SwitchScene(int buildIndex)
    {
        if (!fadeAnimator.gameObject.activeSelf) fadeAnimator.gameObject.SetActive(true);
        if (!fadeAnimator.enabled) fadeAnimator.enabled = true;
        if (!isTransitioning)
        {
            isTransitioning = true;
            OnSwitchScene.Invoke();
            StartCoroutine(SwitchWithFade(buildIndex));
        }
    }

    private IEnumerator SwitchWithFade(int buildIndex)
    {
        fadeAnimator.StopPlayback();
        fadeAnimator.Play("FadeIn");
        yield return new WaitForSecondsRealtime(waitDuration + fadeDuration);
        Time.timeScale = 1;
        SceneManager.LoadSceneAsync(buildIndex, LoadSceneMode.Single);
    }

    private IEnumerator SwitchWithFade(string sceneName)
    {
        fadeAnimator.StopPlayback();
        fadeAnimator.Play("FadeIn");
        yield return new WaitForSecondsRealtime(waitDuration + fadeDuration);
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
