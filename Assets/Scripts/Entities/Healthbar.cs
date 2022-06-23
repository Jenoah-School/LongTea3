using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Healthbar : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private Image healthbarFill = null;
    [SerializeField] private float healthSmoothing = .1f;

    [Header("Powerup")]
    [SerializeField] private Image powerupFill = null;
    [SerializeField] private Image powerupIcon = null;
    [SerializeField] private float powerupDecreaseTime = 0.2f;
    [SerializeField, Range(0f, 1f)] private float powerupMaxFillAmount = 0.9f;

    private Fighter fighterReference;
    private FighterPower powerupReference;

    public void SetColor(Color healthbarColor)
    {
        healthbarFill.color = healthbarColor;
    }

    public void SetFighter(Fighter fighter)
    {
        fighterReference = fighter;
    }

    public void SetPowerup(FighterPower powerup)
    {
        powerupReference = powerup;
        powerupIcon.sprite = powerup.fighterPowerInformation.powerHUDIcon;
    }

    public void RecalculateHealth()
    {
        SetFill(1f / fighterReference.GetStartHealth() * fighterReference.GetCurrentHealth());
    }

    public void SetFill(float fillAmount)
    {
        healthbarFill.DOFillAmount(fillAmount, healthSmoothing);
    }

    public void UsePowerup()
    {
        StopAllCoroutines();
        StartCoroutine(UsePowerupEnum());
    }

    IEnumerator UsePowerupEnum()
    {
        powerupFill.DOFillAmount(0f, powerupDecreaseTime);
        yield return new WaitForSeconds(powerupDecreaseTime);
        float elapsedTime = 0f;
        float duration = powerupReference.cooldown - powerupDecreaseTime;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
            powerupFill.fillAmount = Mathf.Lerp(0f, powerupMaxFillAmount, elapsedTime / duration);
        }
        powerupFill.fillAmount = powerupMaxFillAmount;
    }
}
