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
    }

    public void RecalculateHealth()
    {
        SetFill(1f / fighterReference.GetStartHealth() * fighterReference.GetCurrentHealth());
    }

    public void SetFill(float fillAmount)
    {
        healthbarFill.DOFillAmount(fillAmount, healthSmoothing);
    }
}
