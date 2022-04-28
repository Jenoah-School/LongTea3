using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Users;

public class PlayerManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform playerParent = null;
    [SerializeField] private List<Healthbar> healthbars = new List<Healthbar>();
    [SerializeField] private List<PlayerInput> players = new List<PlayerInput>();

    [Header("Player settings")]
    [SerializeField] private GameObject playerPrefab = null;
    [SerializeField] private List<Color> playerColors = new List<Color>();

    [Header("Events")]
    [SerializeField] private UnityEvent OnPlayerJoin;

    private void Start()
    {
        ++InputUser.listenForUnpairedDeviceActivity;

        InputUser.onUnpairedDeviceUsed +=
    (control, eventPtr) =>
    {
        // Ignore anything but button presses.
        if (!(control is ButtonControl))
            return;

        SpawnNewPlayer();
    };
    }

    public void SpawnNewPlayer()
    {
        Debug.Log("Attempting to spawn new player");
        PlayerInput spawnedPlayerInput = PlayerInput.Instantiate(playerPrefab);
        players.Add(spawnedPlayerInput);
        int playerIndex = players.IndexOf(spawnedPlayerInput);
        
        GameObject spawnedPlayer = spawnedPlayerInput.gameObject;
        spawnedPlayer.name = $"Fighter {spawnedPlayerInput.playerIndex}";
        spawnedPlayer.transform.SetParent(playerParent);

        //Color the player ring
        Transform ringObject = spawnedPlayer.transform.Find("Ring");
        if (ringObject && ringObject.TryGetComponent(out SpriteRenderer ringRenderer))
        {
            ringRenderer.color = playerColors[spawnedPlayerInput.playerIndex % playerColors.Count];
        }

        //Couple healthbar
        if (healthbars.Count >= playerIndex)
        {
            Healthbar healthbar = healthbars[playerIndex];
            healthbar.gameObject.SetActive(true);
            healthbar.SetColor(playerColors[spawnedPlayerInput.playerIndex % playerColors.Count]);
            healthbar.SetFill(1f);
        }

        OnPlayerJoin.Invoke();
        Debug.Log($"Player with ID {spawnedPlayerInput.playerIndex} succesfully spawned");
    }
}
