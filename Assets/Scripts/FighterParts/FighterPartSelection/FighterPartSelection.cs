using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;
using System.Linq;

public class FighterPartSelection : MonoBehaviour
{
    [SerializeField] private List<FighterBodyInformation> fighterBodies = new List<FighterBodyInformation>();
    [SerializeField] private List<FighterWeaponInformation> fighterWeapons = new List<FighterWeaponInformation>();
    [SerializeField] private List<FighterPowerupInformation> fighterPowerups = new List<FighterPowerupInformation>();

    public int currentBodyID = 0;
    public int currentWeaponID = 0;
    public int currentPowerupID = 0;

    private int currentBodyIndex;
    private int currentWeaponIndex;
    private int currentPowerupIndex;

    [Header("General references")]
    [SerializeField] private Fighter fighterReference;

    [Header("Body references")]
    [SerializeField] private Image hpBar = null;
    [SerializeField] private Image attackBar = null;
    [SerializeField] private Image defenseBar = null;
    [SerializeField] private Image speedBar = null;
    [SerializeField] private TextMeshProUGUI bodyDescription = null;

    [Header("Weapon references")]
    [SerializeField] private Image weaponSecondaryPreviewImage = null;
    [SerializeField] private TextMeshProUGUI weaponNameField = null;
    [SerializeField] private Image damageBar = null;
    [SerializeField] private Image rangeBar = null;
    [SerializeField] private TextMeshProUGUI weapon1Description = null;

    [Header("Powerup references")]
    [SerializeField] private Image powerupSecondaryPreviewImage = null;
    [SerializeField] private TextMeshProUGUI powerupNameField = null;
    [SerializeField] private TextMeshProUGUI powerupDescriptionField = null;

    [Header("Part flashing")]
    [SerializeField] private float flashSpeed = 0.2f;
    [SerializeField] private Gradient flashGradient = new Gradient();

    [Header("Events")]
    [SerializeField] private UnityEvent OnChangePart = new UnityEvent();

    private float maxAvailableHP = 0;
    private float maxAvailableAttack = 0;
    private float maxAvailableDefense = 0;
    private float maxAvailableSpeed = 0;

    private float maxWeaponDamage = 0;
    private float maxWeaponRange = 0;

    private bool bodyChanged = false;
    private bool weapon1Changed = false;
    private bool weapon2Changed = false;
    private bool powerupChanged = false;

    private void Start()
    {
        foreach (FighterBodyInformation bodyInfo in fighterBodies)
        {
            maxAvailableHP = bodyInfo.hp > maxAvailableHP ? bodyInfo.hp : maxAvailableHP;
            maxAvailableAttack = bodyInfo.attack > maxAvailableAttack ? bodyInfo.attack : maxAvailableAttack;
            maxAvailableDefense = bodyInfo.defense > maxAvailableDefense ? bodyInfo.defense : maxAvailableDefense;
            maxAvailableSpeed = bodyInfo.speed > maxAvailableSpeed ? bodyInfo.speed : maxAvailableSpeed;
        }

        foreach (FighterWeaponInformation weaponInfo in fighterWeapons)
        {
            maxWeaponDamage = weaponInfo.damage > maxWeaponDamage ? weaponInfo.damage : maxWeaponDamage;
            maxWeaponRange = weaponInfo.range > maxWeaponRange ? weaponInfo.range : maxWeaponRange;
        }

        ChangeBody(0);
        ChangeWeapon(0);
        ChangePowerup(0);
    }

    #region Body

    public void ChangeBody(int index)
    {
        currentBodyIndex = index;
        FighterBodyInformation fighterBody = fighterBodies[currentBodyIndex];
        currentBodyID = fighterBody.partID;

        if (hpBar != null) hpBar.DOFillAmount(1f / maxAvailableHP * fighterBody.hp, 0.1f);
        if (attackBar != null) attackBar.DOFillAmount(1f / maxAvailableAttack * fighterBody.attack, 0.1f);
        if (defenseBar != null) defenseBar.DOFillAmount(1f / maxAvailableDefense * fighterBody.defense, 0.1f);
        if (speedBar != null) speedBar.DOFillAmount(1f / maxAvailableSpeed * fighterBody.speed, 0.1f);
        if (bodyDescription != null) bodyDescription.SetText($"{fighterBody.partDescription}");

        bodyChanged = true;

        OnChangePart.Invoke();

    }

    public void FlashChangedParts()
    {
        List<FighterPart> changedFighterParts = new List<FighterPart>();

        int weaponIndexOffset = 0;

        if (bodyChanged && fighterReference.body != null) changedFighterParts.Add(fighterReference.body);
        if (weapon1Changed && fighterReference.fighterWeapons.Count > 0 && fighterReference.fighterWeapons[0] != null)
        {
            changedFighterParts.Add(fighterReference.fighterWeapons[0]);
            if (fighterReference.fighterWeapons[0].isPair)
            {
                changedFighterParts.Add(fighterReference.fighterWeapons[1]);
                weaponIndexOffset++;
            }
        }
        if (weapon2Changed && fighterReference.fighterWeapons.Count > 1 + weaponIndexOffset && fighterReference.fighterWeapons[1 + weaponIndexOffset] != null)
        {
            changedFighterParts.Add(fighterReference.fighterWeapons[1 + weaponIndexOffset]);
            if (fighterReference.fighterWeapons[1 + weaponIndexOffset].isPair)
            {
                changedFighterParts.Add(fighterReference.fighterWeapons[2 + weaponIndexOffset]);
            }
        }
        //if (powerupChanged && fighterReference.powerup != null) changedFighterParts.Add(fighterReference.powerup);

        foreach (FighterPart changedPart in changedFighterParts)
        {
            foreach (MeshRenderer meshRenderer in changedPart.GetComponentsInChildren<MeshRenderer>())
            {
                if (!meshRenderer.transform.CompareTag("ExcludeItem"))
                {
                    StartCoroutine(FlashPartEnum(meshRenderer.material));
                }
            }
        }

        bodyChanged = false;
        weapon1Changed = false;
        weapon2Changed = false;
        powerupChanged = false;

    }

    public void SelectNextBody()
    {
        ChangeBody((currentBodyIndex + 1) % fighterBodies.Count);
    }

    public void SelectPreviousBody()
    {
        ChangeBody(currentBodyIndex - 1 >= 0 ? currentBodyIndex - 1 : fighterBodies.Count - 1);
    }

    public IEnumerator FlashPartEnum(Material changeMaterial)
    {
        float elapsedTime = 0f;
        changeMaterial.EnableKeyword("_Emmision");
        changeMaterial.globalIlluminationFlags = MaterialGlobalIlluminationFlags.None;

        while (elapsedTime < flashSpeed)
        {
            elapsedTime += Time.deltaTime;
            changeMaterial.SetColor("_EmissionColor", flashGradient.Evaluate(elapsedTime / flashSpeed));
            yield return new WaitForEndOfFrame();
        }

        changeMaterial.SetColor("_EmissionColor", flashGradient.Evaluate(1));
    }

    #endregion
    #region Weapon

    public void ChangeWeapon(int index)
    {
        currentWeaponIndex = index;
        FighterWeaponInformation fighterWeapon = fighterWeapons[currentWeaponIndex];
        currentWeaponID = fighterWeapon.partID;
        if (weaponSecondaryPreviewImage) weaponSecondaryPreviewImage.sprite = fighterWeapon.previewImage;
        if (weaponNameField != null) weaponNameField.text = fighterWeapon.weaponName;
        if (weapon1Description != null) weapon1Description.SetText($"{fighterWeapon.partDescription}");

        damageBar.DOFillAmount(1f / maxWeaponDamage * fighterWeapon.damage, 0.1f);
        rangeBar.DOFillAmount(1f / maxWeaponRange * fighterWeapon.range, 0.1f);

        OnChangePart.Invoke();

        weapon1Changed = true;
    }

    public void SelectNextWeapon()
    {
        ChangeWeapon((currentWeaponIndex + 1) % fighterWeapons.Count);
    }

    public void SelectPreviousWeapon()
    {
        ChangeWeapon(currentWeaponIndex - 1 >= 0 ? currentWeaponIndex - 1 : fighterWeapons.Count - 1);
    }

    #endregion
    #region Powerup

    public void ChangePowerup(int index)
    {
        currentPowerupIndex = index;
        FighterPowerupInformation fighterPowerup = fighterPowerups[currentPowerupIndex];
        currentPowerupID = fighterPowerup.partID;
        if (powerupSecondaryPreviewImage) powerupSecondaryPreviewImage.sprite = fighterPowerup.previewImage;

        if (powerupNameField != null) powerupNameField.text = fighterPowerup.powerupName;
        if(powerupDescriptionField != null) powerupDescriptionField.SetText($"{fighterPowerup.partDescription}");

        OnChangePart.Invoke();

        powerupChanged = true;
    }

    public void SelectNextPowerup()
    {
        ChangePowerup((currentPowerupIndex + 1) % fighterPowerups.Count);
    }

    public void SelectPreviousPowerup()
    {
        ChangePowerup(currentPowerupIndex - 1 >= 0 ? currentPowerupIndex - 1 : fighterPowerups.Count - 1);
    }

    #endregion
}
