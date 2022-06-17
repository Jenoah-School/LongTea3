using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using Lean.Pool;
using DG.Tweening;
using TMPro;
using UnityEngine.Events;

public class Fighter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private PlayerMovement playerMovement;

    public FighterBody body;
    public List<FighterWeapon> fighterWeapons = new List<FighterWeapon>();
    public FighterPower powerup;


    [Header("Fighter info")]
    public int fighterID = 0;
    public Color fighterColor;
    [SerializeField, HideInInspector] float healthPoints;
    [SerializeField, HideInInspector] private float startHealth;
    [SerializeField] protected TextMeshPro damageText;

    public bool canDamage;
    [Header("Particles")]
    [SerializeField] private ParticleSystem healthSmoke;
    [SerializeField] private Gradient smokeColor;

    private ParticleSystem.MainModule healthSmokeMainModule;
    private ParticleSystem.EmissionModule healthSmokeEmissionModule;
    private float healthSmokeMaxEmission = 20f;

    [Header("Health")]
    public bool isDead = false;
    public OnTakeDamage onAttack;
    public OnTakeDamage onTakeDamage;
    public delegate void OnTakeDamage();
    public delegate void OnAttack();
    [SerializeField] private int fallDamageTreshHold;
    [SerializeField] private float fallDamageMultiplier;
    [Space(20)]
    [SerializeField] private UnityEvent OnDamageEvent;
    [SerializeField] private UnityEvent OnDeathEvent;

    

    private List<FighterPart> fighterParts = new List<FighterPart>();

    private float lastFallDmgTime;
    private float lastPowerUpTime;

    private TextMeshPro stackDamageTextObject;

    #region Assembly

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
        canDamage = true;

        foreach (FighterWheels wheelsPart in bodyObject.GetComponentsInChildren<FighterWheels>())
        {
            fighterParts.Add(wheelsPart);
        }

        startHealth = bodyObject.fighterHealth;
        healthPoints = startHealth;

        PostAssemblyStart();
    }

    public void PostAssemblyStart()
    {
        GetPartReferences();
        IgnoreCollisionOnItself();
        SetFighterPartReferences();
        SetCenterOfMass();
        SetReferences();
    }

    private void SetReferences()
    {
        if (playerMovement)
        {
            if (body.GetGroundedCheckOriginTransform()) playerMovement.SetGroundedTransform(body.GetGroundedCheckOriginTransform());
            playerMovement.SetMaxMoveSpeed(body.GetMoveSpeed());
            playerMovement.SetAccelerationSpeed(body.GetAccelerationSpeed());
            playerMovement.SetDrag(body.GetBrakeDrag(), body.GetDriftDrag(), body.GetAirDrag(), body.GetAirVerticalDrag());
        }
        if (body)
        {
            if(fighterID != 0 && PlayerManager.singleton.antennaColors.Count > fighterID - 1) body.SetAntennaColor(PlayerManager.singleton.antennaColors[fighterID - 1]);
        }
        if (healthSmoke)
        {
            healthSmokeMainModule = healthSmoke.main;
            healthSmokeEmissionModule = healthSmoke.emission;
            if(healthSmokeEmissionModule.rateOverTime.constant != 0) healthSmokeMaxEmission = healthSmokeEmissionModule.rateOverTime.constant;
            healthSmokeEmissionModule.rateOverTime = 0f;
            healthSmoke.Play();

        }
    }

    private void SetCenterOfMass()
    {
        if (rb != null && body.GetCenterOfMass() != null)
        {
            rb.centerOfMass = body.GetCenterOfMass().localPosition;
        }
    }

    #endregion

    #region Getters

    private void GetPartReferences()
    {
        fighterParts.Clear();
        fighterWeapons.Clear();
        List<FighterPart> fighterPartRefences = GetComponentsInChildren<FighterPart>().ToList();
        foreach (FighterPart fighterPart in fighterPartRefences)
        {
            if (fighterPart is FighterWeapon)
            {
                fighterWeapons.Add(fighterPart as FighterWeapon);
            }
        }
        fighterParts.AddRange(fighterPartRefences);
    }

    private void SetFighterPartReferences()
    {
        foreach (FighterPart part in fighterParts)
        {
            part.SetReferences(this, rb);
        }
    }

    public FighterBody GetBody()
    {
        return body;
    }

    public FighterWeapon GetFighterWeapon(int index)
    {
        return fighterWeapons[index];
    }

    public FighterPower GetFighterPower()
    {
        return powerup;
    }

    public Rigidbody GetRigidBody()
    {
        return rb;
    }

    public PlayerMovement GetPlayerMovement()
    {
        return playerMovement;
    }

    public float GetStartHealth()
    {
        return startHealth;
    }

    public float GetCurrentHealth()
    {
        return healthPoints;
    }

    #endregion

    #region ActivateStuff

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
        if (Time.time >= lastPowerUpTime)
        {
            lastPowerUpTime = Time.time + powerup.cooldown;
            powerup.Activate();
        }
    }

    #endregion

    #region DamageAndDeath

    private void OnCollisionEnter(Collision collision)
    {
        FallDamage(collision);
    }

    private void FallDamage(Collision collision)
    {
        //Debug.Log("FALL DAMAGE");
        Vector3 direction = collision.transform.position - transform.position;
        //Debug.Log(direction);
        if (Mathf.Abs(collision.relativeVelocity.y) > 10 && Time.time > lastFallDmgTime && !playerMovement.IsGrounded())
        {
            lastFallDmgTime = Time.time + 1;
            float fallDamage = Mathf.Abs(Mathf.Round(collision.relativeVelocity.magnitude * fallDamageMultiplier));
            TakeDamage(fallDamage, this);
        }
    }

    public void TakeDamage(float damage, Fighter origin = null, bool showDamage = true, bool doStack = false)
    {
        if (isDead || !canDamage) return;

        damage = (float)System.Math.Round(damage, 2);
        healthPoints -= damage;
        if (origin = null) origin = this;

        if (showDamage) DamageIndication(damage, origin, doStack);
        if (healthPoints > startHealth) healthPoints = startHealth;
        if (healthSmoke)
        {
            healthSmokeMainModule.startColor = smokeColor.Evaluate(1f - (1f / startHealth * healthPoints));
            healthSmokeEmissionModule.rateOverTime = Mathf.Lerp(healthSmokeMaxEmission, 0f, (1f / startHealth * healthPoints));
        }

        CheckDeath();
        onTakeDamage();
        OnDamageEvent.Invoke();
    }

    public void DamageIndication(float damage, Fighter origin, bool doStack = false)
    {
        TextMeshPro damageTextClone = null;

        if (!doStack)
        {
            damageTextClone = LeanPool.Spawn(damageText, transform.position, transform.rotation);
            damageTextClone.text = damage.ToString();
            damageTextClone.transform.DOMoveY(transform.position.y + damageTextClone.transform.position.y + Random.Range(1.5f, 2.5f), Random.Range(2.5f, 3.5f));
        }
        else
        {
            if (!stackDamageTextObject)
            {
                stackDamageTextObject = LeanPool.Spawn(damageText, transform.position, transform.rotation);
                stackDamageTextObject.text = "0";
                stackDamageTextObject.transform.DOMoveY(transform.position.y + stackDamageTextObject.transform.position.y + Random.Range(1.5f, 2.5f), Random.Range(2.5f, 3.5f));
            }
            if (stackDamageTextObject)
            {
                stackDamageTextObject.transform.position = new Vector3(transform.position.x, stackDamageTextObject.transform.position.y, transform.position.z);
                stackDamageTextObject.text = (float.Parse(stackDamageTextObject.text) + damage).ToString();
                stackDamageTextObject.alpha = 1;
                damageTextClone = stackDamageTextObject;
                damageTextClone.GetComponent<Fade3DText>().StopFadeOut();
            }
        }

        damageTextClone.color = new Color(fighterColor.r, fighterColor.g, fighterColor.b, damageTextClone.color.a);
        damageTextClone.GetComponent<Fade3DText>().StartFadeOut();

        if (damageTextClone.alpha < 0.0001f)
        {
            damageTextClone.alpha = 1;
            damageTextClone.color = fighterColor;
        }
    }

    public void CheckDeath()
    {
        if (isDead) return;
        if (healthPoints <= 0)
        {
            OnDeath();
        }
    }

    private void OnDeath()
    {
        isDead = true;
        onTakeDamage();
        OnDeathEvent.Invoke();
        PlayerManager.singleton.TogglePlayer(this, false);
        PlayerManager.singleton.CheckDeathRate();
        PlayerManager.singleton.IncreaseRankingForAlive();
        Debug.Log($"<color='red'>Player {gameObject.name} died</color>");
    }

    #endregion

    #region Misc

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

    private void OnDrawGizmosSelected()
    {
        if (rb != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(rb.position + rb.centerOfMass, 0.2f);
        }
    }

    #endregion
}
