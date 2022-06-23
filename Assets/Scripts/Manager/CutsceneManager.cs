using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Events;
using System.Linq;

public class CutsceneManager : MonoBehaviour
{
    [Header("Cameras")]
    [SerializeField] private CinemachineBrain cinemachineBrain;
    [SerializeField] private List<CinemachineVirtualCamera> camerasToExclude = new List<CinemachineVirtualCamera>();

    [SerializeField] private List<CinemachineVirtualCamera> camerasToCycle = new List<CinemachineVirtualCamera>();

    [Header("Transition settings")]
    [SerializeField] private float transitionTime = 3f;
    [SerializeField] private UnityEvent preCycleEvent;
    [SerializeField] private UnityEvent afterCycleEvent;
    [SerializeField] private int lowPriority = 5;
    [SerializeField] private int highPriority = 15;

    public static CutsceneManager singleton = null;

    private void Awake()
    {
        singleton = this;
    }

    public void PopulateCameras()
    {
        camerasToCycle = FindObjectsOfType<CinemachineVirtualCamera>().ToList();
        foreach (CinemachineVirtualCamera cameraToExclude in camerasToExclude)
        {
            camerasToCycle.Remove(cameraToExclude);
        }

        foreach (CinemachineVirtualCamera cameraToCycle in camerasToCycle)
        {
            if (cameraToCycle == null) camerasToCycle.Remove(cameraToCycle);
        }
    }

    public void CycleCams()
    {
        StopAllCoroutines();
        StartCoroutine(CycleCamsEnum());
    }

    IEnumerator CycleCamsEnum()
    {
        preCycleEvent.Invoke();
        for (int i = 0; i < camerasToCycle.Count; i++)
        {
            if (camerasToCycle[i] == null) continue;
            foreach (CinemachineVirtualCamera cycleCamera in camerasToCycle)
            {
                cycleCamera.Priority = lowPriority;
            }

            camerasToCycle[i].Priority = highPriority;
            yield return new WaitForSeconds(transitionTime);
        }

        foreach (CinemachineVirtualCamera cycleCamera in camerasToCycle)
        {
            cycleCamera.Priority = lowPriority;
        }

        afterCycleEvent.Invoke();
    }
}
