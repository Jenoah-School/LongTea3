using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;

public class OilBarrel : Projectile
{
    [SerializeField] GameObject oilBarrelModel;
    [SerializeField] Oil oilPattern;
    [SerializeField] UnityEvent OnHitFloor;

    float damage;
    float duration;
    float maxSize;
    Fighter fighterRoot;

    bool canDamage = true;

    public void SetVariables(float damage, float duration, float oilmaxSize, Fighter root)
    {
        this.damage = damage;
        this.duration = duration;
        fighterRoot = root;
        maxSize = oilmaxSize / 10;
        fighterRoot.IgnoreCollisionWithObject(this.gameObject);

        StartCoroutine(FadeAway());
    }

    public override void OnHitFighter()
    {
        if(canDamage) GetHitFighter().TakeDamage(damage, fighterRoot);
        canDamage = false;
    }

    public override void OnHitObject()
    {
        GameObject hitObject = GetHitObject();
        if (hitObject.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            canDamage = false;
            GetComponent<Rigidbody>().isKinematic = true;
            oilBarrelModel.SetActive(false);
            oilPattern.gameObject.SetActive(true);
            oilPattern.transform.eulerAngles = new Vector3(90, 0, 0);
            oilPattern.transform.position = new Vector3(oilPattern.transform.position.x, (hitObject.transform.position.y + hitObject.transform.localScale.y / 2) + 0.001f, oilPattern.transform.position.z);
            oilPattern.transform.DOScale(Random.Range(maxSize / 2, maxSize), 2);
            OnHitFloor.Invoke();
        }       
    }

    IEnumerator FadeAway()
    {
        float fadeDuration = 2;
        yield return new WaitForSeconds(duration);
        oilPattern.DisableOil();
        oilPattern.GetComponent<SpriteRenderer>().DOFade(0, fadeDuration);
        yield return new WaitForSeconds(fadeDuration);
        Destroy(this.gameObject);
    }
}
