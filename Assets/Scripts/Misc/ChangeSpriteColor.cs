using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeSpriteColor : MonoBehaviour
{
    [SerializeField] private Image imageReference;

    public void SetColor(Color newColor)
    {
        imageReference.color = newColor;
    }

    public void ResetColor()
    {
        imageReference.color = Color.white;
    }
}
