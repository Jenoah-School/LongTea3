using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterWheels : FighterPart
{
    public WheelCollider wheelCollider = null;
    public Transform wheelMesh = null;
    public float maxSteeringAngle = 35f;
    public float speed = 35f;
}
