using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Fighter Powerup info", menuName = "Fighter/Fighter Powerup Info", order = 3)]
public class FighterPowerupInformation : FighterPartInformation
{
    public string powerupName = "Powerup name";
    public Sprite powerHUDIcon = null;
}
