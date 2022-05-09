using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Healthbar : MonoBehaviour
{
    [SerializeField] private Image healthbarFill = null;
    [SerializeField] private float healthSmoothing = .1f;

    [SerializeField] private List<FighterPart> playerFighterparts = new List<FighterPart>();
    [SerializeField] private float maxHealth = 0;

    public void SetColor(Color healthbarColor)
    {
        healthbarFill.color = healthbarColor;
    }

    public void SetFighterParts(List<FighterPart> fighterParts)
    {
        foreach (FighterPart fighterPart in fighterParts)
        {
            playerFighterparts.Add(fighterPart);
            maxHealth += fighterPart.healthPoints;
        }
    }

    public void RecalculateHealth()
    {
        float currentHealth = 0f;
        foreach (FighterPart fighterPart in playerFighterparts)
        {
            currentHealth += fighterPart.healthPoints;
        }

        SetFill(1f / maxHealth * currentHealth);
    }

    public void SetFill(float fillAmount)
    {
        healthbarFill.DOFillAmount(fillAmount, healthSmoothing);
    }
}
