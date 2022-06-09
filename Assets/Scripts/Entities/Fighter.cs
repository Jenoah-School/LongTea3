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

    [SerializeField] private int fallDamageTreshHold;
    [SerializeField] private float fallDamageMultiplier;

    [SerializeField] protected TextMeshPro damageText;

    [SerializeField, HideInInspector] float healthPoints;
    [SerializeField, HideInInspector] private float startHealth;

    public bool isDead = false;
    public OnTakeDamage onAttack;
    public delegate void OnTakeDamage();
    public OnTakeDamage onTakeDamage;
    public delegate void OnAttack();
    public Color fighterColor;

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
            playerMovement.SetMoveSpeed(body.GetMoveSpeed());
            playerMovement.SetAccelerationSpeed(body.GetAccelerationSpeed());
            playerMovement.SetDrag(body.GetBrakeDrag(), body.GetDriftDrag(), body.GetAirDrag(), body.GetAirVerticalDrag());
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

    public Rigidbody GetRigidBody()
    {
        return rb;
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
        if(Time.time >= lastPowerUpTime)
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
        Vector3 direction = collision.transform.position - transform.position;
        //Debug.Log(direction);
        if(Mathf.Abs(collision.relativeVelocity.y) > 10 && Time.time > lastFallDmgTime && !playerMovement.IsGrounded())
        {
            lastFallDmgTime = Time.time + 1;
            float fallDamage =  Mathf.Abs(Mathf.Round(collision.relativeVelocity.magnitude * fallDamageMultiplier));
            TakeDamage(fallDamage, this);
        }
    }
    
    public void TakeDamage(float damage, Fighter origin = null, bool showDamage = true, bool doStack = false)
    {
        if (isDead) return;

        damage = (float)System.Math.Round(damage, 2);
        healthPoints -= damage;
        if (origin = null) origin = this;

        if (showDamage) DamageIndication(damage, origin, doStack);

        CheckDeath();
        onTakeDamage();
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
            if(stackDamageTextObject)
            {
                stackDamageTextObject.transform.position = new Vector3(transform.position.x, stackDamageTextObject.transform.position.y, transform.position.z);
                stackDamageTextObject.text = (float.Parse(stackDamageTextObject.text) + damage).ToString();             
                stackDamageTextObject.alpha = 1;
                damageTextClone = stackDamageTextObject;
                damageTextClone.GetComponent<Fade3DText>().StopFadeOut();
            }
        }

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
