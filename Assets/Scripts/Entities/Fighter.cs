using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public class Fighter : MonoBehaviour
{
    [Header("Vehicle")]
    [SerializeField] private List<FighterWeapon> fighterWeapons = new List<FighterWeapon>();
    [SerializeField] private Rigidbody rb;
    [SerializeField] private FighterBody body;
    [SerializeField] private PlayerMovement playerMovement;

    [SerializeField, Range(10, 100)] private int healthTreshHold;
    [SerializeField] private float fallDamageMultiplier;

    private List<FighterPart> fighterParts = new List<FighterPart>();

    private float startTotalHealth;
    public bool isDead = false;

    public delegate void OnTakeDamage();
    public OnTakeDamage onTakeDamage;

    public delegate void OnAttack();
    public OnTakeDamage onAttack;

    public void AssembleFighterParts(FighterBody body, List<FighterWeapon> weapons)
    {
        FighterBody bodyObject = Instantiate(body, transform);
        bodyObject.transform.localPosition = new Vector3(0, 0, 0);
        this.body = bodyObject;
        fighterParts.Add(bodyObject);

        for(int i = 0; i < weapons.Count; i++)
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
        fighterParts.AddRange(GetComponentsInChildren<FighterPart>());
    }

    public float GetStartHealth()
    {
        return startTotalHealth;
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
        if(GetTotalPartHealth() < startTotalHealth / 100 * healthTreshHold)
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
        Debug.Log("He ded boy");
    }

    public void ExecutePrimary(InputAction.CallbackContext context)
    {
        if (isDead) return;
        if (context.action.WasPerformedThisFrame())
        {
            foreach(FighterWeapon weapon in fighterWeapons)
            {
                if(weapon.weaponOrder == FighterWeapon.WeaponOrder.PRIMARY)
                {
                    weapon.ActivateWeapon();
                }
            }
        }
    }

    public void ExecuteSecondary(InputAction.CallbackContext context)
    {
        if (isDead) return;
        if (context.action.WasPerformedThisFrame())
        {
            foreach (FighterWeapon weapon in fighterWeapons)
            {
                if (weapon.weaponOrder == FighterWeapon.WeaponOrder.SECONDARY)
                {
                    weapon.ActivateWeapon();
                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        FallDamage(collision.relativeVelocity, collision.contacts[0].thisCollider.gameObject);
    }

    private void FallDamage(Vector3 hitForce, GameObject col)
    {
        Debug.Log(hitForce.y);
        if(Mathf.Abs(hitForce.y) > 10)
        {
            if(col.gameObject.GetComponent<FighterPart>())
            {
                FighterPart part = col.gameObject.GetComponent<FighterPart>();
                part.TakeDamage(10 * fallDamageMultiplier, part.transform.position, Color.blue);
            }
            else if (col.gameObject.GetComponentInParent<FighterPart>())
            {
                FighterPart part = col.gameObject.GetComponentInParent<FighterPart>();
                part.TakeDamage(hitForce.y * fallDamageMultiplier, part.transform.position, Color.blue);
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
}
