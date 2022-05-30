using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Healthbar : MonoBehaviour
{
    [SerializeField] private Image healthbarFill = null;
    [SerializeField] private float healthSmoothing = .1f;

    private float healthbarMultiplier = 1f;

    private Fighter fighterReference;

    public void SetColor(Color healthbarColor)
    {
        healthbarFill.color = healthbarColor;
    }

    public void SetFighter(Fighter fighter)
    {
        fighterReference = fighter;

        float fatalHealth = fighterReference.GetStartHealth() / 100f * fighterReference.GetHealthThreshold();
        float fillValue = (fighterReference.GetTotalPartHealth() - fatalHealth) / fighterReference.GetStartHealth();
        healthbarMultiplier = 1f / fillValue;
    }

    public void RecalculateHealth()
    {
        float fatalHealth = fighterReference.GetStartHealth() / 100f * fighterReference.GetHealthThreshold();
        float fillValue = (fighterReference.GetTotalPartHealth() - fatalHealth) / fighterReference.GetStartHealth();

        SetFill(healthbarMultiplier * fillValue);
    }

    public void SetFill(float fillAmount)
    {
        healthbarFill.DOFillAmount(fillAmount, healthSmoothing);
    }
}
