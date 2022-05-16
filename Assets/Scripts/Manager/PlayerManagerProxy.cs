using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManagerProxy : MonoBehaviour
{
    public void SetMoveStates(bool newMoveState)
    {
        if (PlayerManager.singleton != null)
        {
            PlayerManager.singleton.SetMoveStates(newMoveState);
        }
    }
}
