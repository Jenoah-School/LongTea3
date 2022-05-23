using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Healthbar : MonoBehaviour
{
    [SerializeField] private Image healthbarFill = null;
    [SerializeField] private float healthSmoothing = .1f;

    private Fighter fighterReference;

    public void SetColor(Color healthbarColor)
    {
        healthbarFill.color = healthbarColor;
    }

    public void SetFighter(Fighter fighter)
    {
        fighterReference = fighter;
    }

    public void RecalculateHealth()
    {
        //This only works for half health threshold for some reason. I will look to it next week :/
        //TODO: Fix the threshold
        float fatalHealth = fighterReference.GetStartHealth() / 100f * fighterReference.GetHealthThreshold();
        float fillValue = (fighterReference.GetTotalPartHealth() - fatalHealth) / fatalHealth;

        SetFill(fillValue);
    }

    public void SetFill(float fillAmount)
    {
        healthbarFill.DOFillAmount(fillAmount, healthSmoothing);
    }
}
