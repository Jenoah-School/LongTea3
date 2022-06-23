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
    [SerializeField] private List<FighterWeaponInformation> fighterRangedWeapons = new List<FighterWeaponInformation>();
    [SerializeField] private List<FighterWeaponInformation> fighterMeleeWeapons = new List<FighterWeaponInformation>();
    [SerializeField] private List<FighterPowerupInformation> fighterPowerups = new List<FighterPowerupInformation>();
    [Space(10)]

    public int currentBodyID = 0;
    public int currentRangedWeaponID = 0;
    public int currentMeleeWeaponID = 0;
    public int currentPowerupID = 0;

    private int currentSelectedRangedWeaponIndex = 0;
    private int currentSelectedMeleeWeaponIndex = 0;

    private int currentBodyIndex;
    private int currentMeleeWeaponIndex;
    private int currentRangedWeaponIndex;
    private int currentPowerupIndex;

    private FighterWeapon.WeaponLocations rangedWeaponLocation = FighterWeapon.WeaponLocations.TOP;
    private FighterWeapon.WeaponLocations meleeWeaponLocation = FighterWeapon.WeaponLocations.SIDE;

    [Header("General references")]
    [SerializeField] private Fighter fighterReference;

    [Header("Body references")]
    [SerializeField] private TextMeshProUGUI fighterField = null;
    [SerializeField] private Image hpBar = null;
    [SerializeField] private Image attackBar = null;
    [SerializeField] private Image defenseBar = null;
    [SerializeField] private Image speedBar = null;
    [SerializeField] private TextMeshProUGUI bodyDescription = null;

    [Header("Ranged weapon references")]
    [SerializeField] private TextMeshProUGUI rangedWeaponNameField = null;
    [SerializeField] private Image rangedDamageBar = null;
    [SerializeField] private Image rangedRangeBar = null;
    [SerializeField] private TextMeshProUGUI rangedWeaponDescription = null;

    [Header("Melee weapon references")]
    [SerializeField] private TextMeshProUGUI meleeWeaponNameField = null;
    [SerializeField] private Image meleeDamageBar = null;
    [SerializeField] private Image meleeRangeBar = null;
    [SerializeField] private TextMeshProUGUI meleeWeaponDescription = null;

    [Header("Powerup references")]
    [SerializeField] private Image powerupSecondaryPreviewImage = null;
    [SerializeField] private TextMeshProUGUI powerupNameField = null;
    [SerializeField] private TextMeshProUGUI powerupDescriptionField = null;
    [SerializeField] private string cooldownFieldPrefix = "Cooldown:";
    [SerializeField] private TextMeshProUGUI powerupCooldownField = null;
    [SerializeField] private string cooldownFieldEnd = "s";

    [Header("Part flashing")]
    [SerializeField] private float flashSpeed = 0.2f;
    [SerializeField] private Gradient flashGradient = new Gradient();

    [Header("Events")]
    [SerializeField] private UnityEvent OnChangePart = new UnityEvent();
    [SerializeField] private UnityEvent OnWeaponLocationNotAvailable = new UnityEvent();
    [SerializeField] private UnityEvent OnWeaponLocationAvailable = new UnityEvent();

    private float maxAvailableHP = 0;
    private float maxAvailableAttack = 0;
    private float maxAvailableDefense = 0;
    private float maxAvailableSpeed = 0;

    private float maxRangedWeaponDamage = 0;
    private float maxRangedWeaponRange = 0;

    private float maxMeleeWeaponDamage = 0;
    private float maxMeleeWeaponRange = 0;

    private bool bodyChanged = false;
    private bool rangedWeaponChanged = false;
    private bool meleeWeaponChanged = false;
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

        foreach (FighterWeaponInformation weaponInfo in fighterRangedWeapons)
        {
            maxRangedWeaponDamage = weaponInfo.damage > maxRangedWeaponDamage ? weaponInfo.damage : maxRangedWeaponDamage;
            maxRangedWeaponRange = weaponInfo.range > maxRangedWeaponRange ? weaponInfo.range : maxRangedWeaponRange;
        }

        foreach (FighterWeaponInformation weaponInfo in fighterMeleeWeapons)
        {
            maxMeleeWeaponDamage = weaponInfo.damage > maxMeleeWeaponDamage ? weaponInfo.damage : maxMeleeWeaponDamage;
            maxMeleeWeaponRange = weaponInfo.range > maxMeleeWeaponRange ? weaponInfo.range : maxMeleeWeaponRange;
        }

        ChangeBody(0);
        ChangeRangedWeapon(0);
        ChangeMeleeWeapon(0);
        ChangePowerup(0);
    }

    #region Body

    public void ChangeBody(int index)
    {
        currentBodyIndex = index;
        FighterBodyInformation fighterBody = fighterBodies[currentBodyIndex];
        currentBodyID = fighterBody.partID;

        if (fighterField != null) fighterField.SetText($"{fighterBody.bodyName}");
        if (hpBar != null) hpBar.DOFillAmount(1f / maxAvailableHP * fighterBody.hp, 0.1f);
        if (attackBar != null) attackBar.DOFillAmount(1f / maxAvailableAttack * fighterBody.attack, 0.1f);
        if (defenseBar != null) defenseBar.DOFillAmount(1f / maxAvailableDefense * fighterBody.defense, 0.1f);
        if (speedBar != null) speedBar.DOFillAmount(1f / maxAvailableSpeed * fighterBody.speed, 0.1f);
        if (bodyDescription != null) bodyDescription.SetText($"{fighterBody.partDescription}");

        bodyChanged = true;

        OnChangePart.Invoke();

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

    public void ChangeRangedWeapon(int index, bool tryChangeOther = false)
    {
        if (currentRangedWeaponIndex == index) return;
        currentSelectedRangedWeaponIndex = index;
        FighterWeaponInformation fighterWeapon = fighterRangedWeapons[currentSelectedRangedWeaponIndex];
        FighterWeapon.WeaponLocations currentWeaponLocation = FighterCreator.singleton.fighterWeapons[fighterWeapon.partID].weaponLocation;

        if (meleeWeaponLocation == currentWeaponLocation)
        {
            //Not available
            OnWeaponLocationNotAvailable.Invoke();
        }
        else
        {
            currentRangedWeaponID = fighterWeapon.partID;
            rangedWeaponLocation = currentWeaponLocation;
            currentRangedWeaponIndex = index;

            OnChangePart.Invoke();
            OnWeaponLocationAvailable.Invoke();
            rangedWeaponChanged = true;
        }
        if (rangedWeaponNameField != null) rangedWeaponNameField.text = fighterWeapon.weaponName;
        if (rangedWeaponDescription != null) rangedWeaponDescription.SetText($"{fighterWeapon.partDescription}");

        rangedDamageBar.DOFillAmount(1f / maxRangedWeaponDamage * fighterWeapon.damage, 0.1f);
        rangedRangeBar.DOFillAmount(1f / maxRangedWeaponRange * fighterWeapon.range, 0.1f);

        if (tryChangeOther) ChangeMeleeWeapon(currentSelectedMeleeWeaponIndex);

    }

    public void ChangeMeleeWeapon(int index, bool tryChangeOther = false)
    {
        if (currentMeleeWeaponIndex == index) return;
        currentSelectedMeleeWeaponIndex = index;
        FighterWeaponInformation fighterWeapon = fighterMeleeWeapons[currentSelectedMeleeWeaponIndex];
        FighterWeapon.WeaponLocations currentWeaponLocation = FighterCreator.singleton.fighterWeapons[fighterWeapon.partID].weaponLocation;
        if (rangedWeaponLocation == currentWeaponLocation)
        {
            //Not available
            OnWeaponLocationNotAvailable.Invoke();
        }
        else
        {
            currentMeleeWeaponID = fighterWeapon.partID;
            meleeWeaponLocation = currentWeaponLocation;
            currentMeleeWeaponIndex = index;

            OnChangePart.Invoke();
            meleeWeaponChanged = true;
            OnWeaponLocationAvailable.Invoke();
        }
        
        if (meleeWeaponNameField != null) meleeWeaponNameField.text = fighterWeapon.weaponName;
        if (meleeWeaponDescription != null) meleeWeaponDescription.SetText($"{fighterWeapon.partDescription}");

        meleeDamageBar.DOFillAmount(1f / maxRangedWeaponDamage * fighterWeapon.damage, 0.1f);
        meleeRangeBar.DOFillAmount(1f / maxRangedWeaponRange * fighterWeapon.range, 0.1f);

        if (tryChangeOther) ChangeRangedWeapon(currentSelectedRangedWeaponIndex);
    }


    public void SelectNextRangedWeapon()
    {
        ChangeRangedWeapon((currentSelectedRangedWeaponIndex + 1) % fighterRangedWeapons.Count, true);
    }

    public void SelectNextMeleeWeapon()
    {
        ChangeMeleeWeapon((currentSelectedMeleeWeaponIndex + 1) % fighterMeleeWeapons.Count, true);
    }

    public void SelectPreviousRangedWeapon()
    {
        ChangeRangedWeapon(currentSelectedRangedWeaponIndex - 1 >= 0 ? currentSelectedRangedWeaponIndex - 1 : fighterRangedWeapons.Count - 1, true);
    }

    public void SelectPreviousMeleeWeapon()
    {
        ChangeMeleeWeapon(currentSelectedMeleeWeaponIndex - 1 >= 0 ? currentSelectedMeleeWeaponIndex - 1 : fighterMeleeWeapons.Count - 1, true);
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
        if (powerupCooldownField != null) powerupCooldownField.SetText($"{cooldownFieldPrefix}{fighterPowerup.cooldownTime}{cooldownFieldEnd}");

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

    public void FlashChangedParts()
    {
        List<FighterPart> changedFighterParts = new List<FighterPart>();

        int weaponIndexOffset = 0;

        if (bodyChanged && fighterReference.body != null) changedFighterParts.Add(fighterReference.body);
        if (rangedWeaponChanged && fighterReference.fighterWeapons.Count > 0 && fighterReference.fighterWeapons[0] != null)
        {
            changedFighterParts.Add(fighterReference.fighterWeapons[0]);
            if (fighterReference.fighterWeapons[0].isPair)
            {
                changedFighterParts.Add(fighterReference.fighterWeapons[1]);
                weaponIndexOffset++;
            }
        }
        if (meleeWeaponChanged && fighterReference.fighterWeapons.Count > 1 + weaponIndexOffset && fighterReference.fighterWeapons[1 + weaponIndexOffset] != null)
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
        rangedWeaponChanged = false;
        meleeWeaponChanged = false;
        powerupChanged = false;

    }
}
