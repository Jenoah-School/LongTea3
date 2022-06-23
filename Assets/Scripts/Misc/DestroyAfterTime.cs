using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;

public class DestroyAfterTime : MonoBehaviour
{
    public float lifeTime = 5f;

    [Header("Animating")]
    [SerializeField] private bool scaleDownBeforeDestroy = true;
    [SerializeField] private float scalingTime = 0.3f;
    [SerializeField] private bool TryPoolDespawn = true;

    private void OnEnable()
    {
        RestartTimer();
    }

    public void RestartTimer()
    {
        StopAllCoroutines();
        StartCoroutine(DestroyTime());
    }

    IEnumerator DestroyTime()
    {
        float waitTime = scaleDownBeforeDestroy ? lifeTime - scalingTime : lifeTime;
        yield return new WaitForSeconds(waitTime);

        if (scaleDownBeforeDestroy)
        {
            float elapsedTime = 0f;
            Vector3 startScale = transform.localScale;
            while(elapsedTime < waitTime)
            {
                elapsedTime += Time.deltaTime;
                transform.localScale = startScale * Mathf.SmoothStep(1f, 0f, elapsedTime / waitTime);
                yield return new WaitForEndOfFrame();
            }
        }

        if (TryPoolDespawn)
        {
            if (LeanPool.Links.ContainsKey(gameObject))
            {
                LeanPool.Despawn(gameObject);
                yield break;
            }
        }

        Destroy(gameObject);
    }
}
