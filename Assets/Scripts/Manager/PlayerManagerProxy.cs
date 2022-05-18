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

    public void DestroyPlayerManager()
    {
        Destroy(PlayerManager.singleton.gameObject);
    }

    public void StopListeningForInput()
    {
        PlayerManager.singleton.StopListeningForInput();
    }

    public void UnbindAllPlayers()
    {
        PlayerManager.singleton.UnbindAllInput();
    }
}
