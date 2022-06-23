using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ShockWave : FighterPower
{
    [SerializeField] GameObject shockwaveRing;     
    [SerializeField] float ringDamage;
    [SerializeField] float minRingSpeed;
    [SerializeField] float ringForceMultiplier;
    [SerializeField] float maxRingSize;

    List<Fighter> hitFighters = new List<Fighter>();

    float scaleSpeed;
    Material shockwaveMaterial;
    bool startedFade;

    Vector3 startRingTransform;

    private void Start()
    {
        shockwaveRing.SetActive(false);
        transform.parent = null;
        shockwaveMaterial = shockwaveRing.transform.GetChild(0).GetComponent<Renderer>().material;       
        startRingTransform = transform.localScale;
        maxRingSize = maxRingSize / 2;
        scaleSpeed = maxRingSize / 2;
    }

    public override void Activate()
    {
        Debug.Log(fighterRoot.onUsePowerup);
        StartCoroutine(FireRing());
        OnTrigger.Invoke();
        fighterRoot.onUsePowerup();
    }

    IEnumerator FireRing()
    {
        shockwaveRing.SetActive(true);
        transform.position = fighterRoot.transform.position;
        hitFighters.Add(fighterRoot);
        scaleSpeed = maxRingSize / 2;
        transform.localScale = startRingTransform;
        shockwaveMaterial.color = new Color(shockwaveMaterial.color.r, shockwaveMaterial.color.g, shockwaveMaterial.color.b, 1);
        while (transform.localScale.x < maxRingSize)
        {
            if (scaleSpeed > minRingSpeed) { scaleSpeed -= 0.1f; }
            else { if (!startedFade) startedFade = true; shockwaveMaterial.DOFade(0, 1); }

            transform.localScale += new Vector3(1,0,1) * (scaleSpeed / 100);
            yield return new WaitForEndOfFrame();
        }
        hitFighters.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.GetComponentInParent<Fighter>())
        {
            Fighter hitFighter = other.transform.GetComponentInParent<Fighter>();
            if (!hitFighters.Contains(hitFighter))
            {
                Debug.Log("HIT" + hitFighter.name);
                hitFighters.Add(hitFighter);
                hitFighter.TakeDamage(ringDamage, fighterRoot);
                hitFighter.GetComponent<Rigidbody>().AddForce((( hitFighter.transform.position - transform.position).normalized + fighterRoot.transform.up * 2) * ((350) * (3 * 20)) * Mathf.Abs(Physics.gravity.y / 10));
            }
        }
    }
}
