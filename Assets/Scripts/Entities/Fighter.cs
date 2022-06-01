using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using Lean.Pool;
using DG.Tweening;
using TMPro;

public class Fighter : MonoBehaviour
{
    [Header("Vehicle")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private PlayerMovement playerMovement;

    [SerializeField] private FighterBody body;    
    private List<FighterWeapon> fighterWeapons = new List<FighterWeapon>();
    [SerializeField] private FighterPower powerup;

    [SerializeField, Range(10, 100)] private int healthThreshold;
    [SerializeField] private int fallDamageTreshHold;
    [SerializeField] private float fallDamageMultiplier;

    [SerializeField] protected GameObject damageText;

    private List<FighterPart> fighterParts = new List<FighterPart>();

    private float startTotalHealth;
    public bool isDead = false;

    public delegate void OnTakeDamage();
    public OnTakeDamage onTakeDamage;

    public delegate void OnAttack();
    public OnTakeDamage onAttack;

    public Color fighterColor;

    private float lastFallDmgTime;

    private float lastPowerUpTime;

    public void AssembleFighterParts(FighterBody body, List<FighterWeapon> weapons, FighterPower powerup)
    {
        FighterBody bodyObject = Instantiate(body, transform);
        bodyObject.transform.localPosition = new Vector3(0, 0, 0);
        this.body = bodyObject;
        fighterParts.Add(bodyObject);

        for (int i = 0; i < weapons.Count; i++)
        {
            FighterWeapon weapon = Instantiate(weapons[i], transform);
            weapon.transform.localPosition = bodyObject.GetWeaponLocation(weapon.weaponLocation).localPosition;
            weapon.transform.localEulerAngles = bodyObject.GetWeaponLocation(weapon.weaponLocation).localEulerAngles;
            weapon.weaponOrder = (FighterWeapon.WeaponOrder)i;
            fighterWeapons.Add(weapon);

            if (weapons[i].isPair)
            {
                FighterWeapon weapon2 = Instantiate(weapons[i], transform);
                weapon2.transform.localPosition = bodyObject.GetWeaponRightSideLocation().localPosition;
                weapon2.transform.localEulerAngles = bodyObject.GetWeaponRightSideLocation().localEulerAngles;
                weapon2.weaponOrder = (FighterWeapon.WeaponOrder)i;
                fighterWeapons.Add(weapon2);
            }
        }

        FighterPower power = Instantiate(powerup, transform);
        power.SetFighterRoot(this);
        this.powerup = power;

        foreach (FighterWheels wheelsPart in bodyObject.GetComponentsInChildren<FighterWheels>())
        {
            fighterParts.Add(wheelsPart);
        }

        PostAssemblyStart();
    }

    public void PostAssemblyStart()
    {
        GetPartReferences();
        IgnoreCollisionOnItself();
        SetFighterPartReferences();
        SetCenterOfMass();
        startTotalHealth = GetTotalPartHealth();
    }

    private void GetPartReferences()
    {
        List<FighterPart> fighterPartRefences = GetComponentsInChildren<FighterPart>().ToList();
        foreach(FighterPart fighterPart in fighterPartRefences)
        {
            if(fighterPart is FighterWeapon)
            {
                fighterWeapons.Add(fighterPart as FighterWeapon);
            }
        }
        fighterParts.AddRange(fighterPartRefences);
    }

    public float GetStartHealth()
    {
        return startTotalHealth;
    }

    public float GetHealthThreshold()
    {
        return healthThreshold;
    }

    private void SetCenterOfMass()
    {
        if (rb != null && body.GetCenterOfMass() != null)
        {
            rb.centerOfMass = body.GetCenterOfMass().localPosition;
        }
    }

    private void IgnoreCollisionOnItself()
    {
        foreach (Collider col1 in GetComponentsInChildren<Collider>())
        {
            foreach (Collider col2 in GetComponentsInChildren<Collider>())
            {
                Physics.IgnoreCollision(col1, col2);
            }
        }
    }

    public void IgnoreCollisionWithObject(GameObject gameObject)
    {
        foreach (Collider fighterCol in GetComponentsInChildren<Collider>())
        {
            foreach (Collider gameObjectCol in gameObject.GetComponentsInChildren<Collider>())
            {
                Physics.IgnoreCollision(fighterCol, gameObjectCol);
            }
        }
    }

    private void SetFighterPartReferences()
    {
        foreach (FighterPart part in fighterParts)
        {
            part.SetReferences(this, rb);
        }
    }

    public float GetTotalPartHealth()
    {
        if (isDead) return 0;
        float totalHealth = 0;
        foreach (FighterPart part in fighterParts)
        {
            totalHealth += part.healthPoints;
        }
        return totalHealth;
    }

    public void CheckDeath()
    {
        if (isDead) return;
        if(GetTotalPartHealth() < startTotalHealth / 100f * (float)healthThreshold)
        {
            OnDeath();
        }
    }

    private void OnDeath()
    {
        isDead = true;
        onTakeDamage();
        PlayerManager.singleton.TogglePlayer(this, false);
        PlayerManager.singleton.CheckDeathRate();
        PlayerManager.singleton.IncreaseRankingForAlive();
        Debug.Log($"<color='red'>Player {gameObject.name} died</color>");
    }

    public void ExecutePrimary(InputAction.CallbackContext context)
    {
        if (isDead || !enabled) return;
        foreach (FighterWeapon weapon in fighterWeapons)
        {
            if (weapon.weaponOrder == FighterWeapon.WeaponOrder.PRIMARY)
            {
                weapon.ActivateWeapon(context);
            }
        }
    }

    public void ExecuteSecondary(InputAction.CallbackContext context)
    {
        if (isDead || !enabled) return;
        foreach (FighterWeapon weapon in fighterWeapons)
        {
            if (weapon.weaponOrder == FighterWeapon.WeaponOrder.SECONDARY)
            {
                weapon.ActivateWeapon(context);
            }
        }
    }

    public void ExecutePowerUp(InputAction.CallbackContext context)
    {
        if(Time.time >= lastPowerUpTime)
        {
            lastPowerUpTime = lastPowerUpTime + Time.time;
            powerup.Activate();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        FallDamage(collision.relativeVelocity);
    }

    private void FallDamage(Vector3 hitForce)
    {
        if(Mathf.Abs(hitForce.y) > 10 && Time.time > lastFallDmgTime)
        {
            lastFallDmgTime = Time.time + 1;
            float fallDamage =  Mathf.Abs(Mathf.Round(hitForce.y * fallDamageMultiplier));

            foreach (FighterPart part in fighterParts)
            {
                part.TakeDamage(fallDamage / fighterParts.Count, part.transform.position, false);
            }
            //Debug.Log(GetTotalPartHealth());
            DamageIndication(fallDamage, transform.position, this);
        }
    }

    public void DamageIndication(float damage, Vector3 hitPos, Fighter origin, bool doStack = false)
    {
        GameObject damageTextObject = LeanPool.Spawn(damageText, hitPos, transform.rotation);
        TextMeshPro damageTextObjectText = damageTextObject.GetComponent<TextMeshPro>();

        damageTextObjectText.alpha = 1;
        damageTextObjectText.text = damage.ToString();
        damageTextObjectText.color = fighterColor;

        damageTextObject.transform.DOMoveY(transform.position.y + damageTextObject.transform.position.y + Random.Range(1.5f, 2.5f), Random.Range(2.5f, 3.5f));

        LeanPool.Despawn(damageTextObject, 3);

        onTakeDamage();
    }

    private void OnDrawGizmosSelected()
    {
        if (rb != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(rb.position + rb.centerOfMass, 0.2f);
        }
    }
}
