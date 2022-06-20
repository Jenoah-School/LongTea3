using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Fighter Body info", menuName = "Fighter/Fighter Body Info", order = 1)]
public class FighterBodyInformation : FighterPartInformation
{
    public string bodyName = "Body name";
    public float hp = 0;
    public float attack = 0;
    public float defense = 0;
    public float speed = 0;
    public float weight = 0;
}
