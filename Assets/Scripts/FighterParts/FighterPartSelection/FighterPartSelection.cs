using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;

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

    [Header("Body references")]
    [SerializeField] private Image hpBar = null;
    [SerializeField] private Image attackBar = null;
    [SerializeField] private Image defenseBar = null;
    [SerializeField] private Image speedBar = null;
    [SerializeField] private TextMeshProUGUI weightString = null;

    [Header("Weapon references")]
    [SerializeField] private Image weaponSecondaryPreviewImage = null;
    [SerializeField] private TextMeshProUGUI weaponNameField = null;
    [SerializeField] private Image damageBar = null;
    [SerializeField] private Image rangeBar = null;

    [Header("Powerup references")]
    [SerializeField] private Image powerupSecondaryPreviewImage = null;
    [SerializeField] private TextMeshProUGUI powerupNameField = null;
    [SerializeField] private TextMeshProUGUI powerupDescriptionField = null;

    [Header("Events")]
    [SerializeField] private UnityEvent OnChangePart = new UnityEvent();

    private float maxAvailableHP = 0;
    private float maxAvailableAttack = 0;
    private float maxAvailableDefense = 0;
    private float maxAvailableSpeed = 0;

    private float maxWeaponDamage = 0;
    private float maxWeaponRange = 0;

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

        hpBar.DOFillAmount(1f / maxAvailableHP * fighterBody.hp, 0.1f);
        attackBar.DOFillAmount(1f / maxAvailableAttack * fighterBody.attack, 0.1f);
        defenseBar.DOFillAmount(1f / maxAvailableDefense * fighterBody.defense, 0.1f);
        speedBar.DOFillAmount(1f / maxAvailableSpeed * fighterBody.speed, 0.1f);
        weightString.text = $"Weight: {fighterBody.weight}";

        OnChangePart.Invoke();
    }

    public void SelectNextBody()
    {
        ChangeBody((currentBodyID + 1) % fighterBodies.Count);
    }

    public void SelectPreviousBody()
    {
        ChangeBody(currentBodyID - 1 >= 0 ? currentBodyID - 1 : fighterBodies.Count - 1);
    }

    #endregion
    #region Weapon

    public void ChangeWeapon(int index)
    {
        currentWeaponIndex = index;
        FighterWeaponInformation fighterWeapon = fighterWeapons[currentWeaponIndex];
        currentWeaponID = fighterWeapon.partID;
        if (weaponSecondaryPreviewImage) weaponSecondaryPreviewImage.sprite = fighterWeapon.previewImage;
        weaponNameField.text = fighterWeapon.weaponName;

        damageBar.DOFillAmount(1f / maxWeaponDamage * fighterWeapon.damage, 0.1f);
        rangeBar.DOFillAmount(1f / maxWeaponRange * fighterWeapon.range, 0.1f);

        OnChangePart.Invoke();
    }

    public void SelectNextWeapon()
    {
        ChangeWeapon((currentWeaponID + 1) % fighterWeapons.Count);
    }

    public void SelectPreviousWeapon()
    {
        ChangeWeapon(currentWeaponID - 1 >= 0 ? currentWeaponID - 1 : fighterWeapons.Count - 1);
    }

    #endregion
    #region Powerup

    public void ChangePowerup(int index)
    {
        currentPowerupIndex = index;
        FighterPowerupInformation fighterPowerup = fighterPowerups[currentPowerupIndex];
        currentPowerupID = fighterPowerup.partID;
        if (powerupSecondaryPreviewImage) powerupSecondaryPreviewImage.sprite = fighterPowerup.previewImage;

        powerupNameField.text = fighterPowerup.powerupName;
        powerupDescriptionField.text = fighterPowerup.powerupDescription;

        OnChangePart.Invoke();
    }

    public void SelectNextPowerup()
    {
        ChangePowerup((currentPowerupID + 1) % fighterPowerups.Count);
    }

    public void SelectPreviousPowerup()
    {
        ChangePowerup(currentPowerupID - 1 >= 0 ? currentPowerupID - 1 : fighterPowerups.Count - 1);
    }

    #endregion
}
