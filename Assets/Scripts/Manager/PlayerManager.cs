using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Users;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform playerParent = null;
    [SerializeField] private List<Healthbar> healthbars = new List<Healthbar>();
    [SerializeField] private List<PlayerJoinView> playerJoinViews = new List<PlayerJoinView>();
    [SerializeField] private List<PlayerInput> players = new List<PlayerInput>();

    [Header("Player settings")]
    [SerializeField] private GameObject controllerPrefab = null;
    [SerializeField] private GameObject fighterPrefab = null;
    [SerializeField] private List<Color> playerColors = new List<Color>();

    [Header("Events")]
    [SerializeField] private bool spawnFighter = false;
    [SerializeField] private bool spawnFighterController = false;
    [SerializeField] private int spawnFighterSceneIndex = -1;
    [SerializeField] private UnityEvent OnPlayerJoin;
    [SerializeField] private UnityEvent OnSceneSwitch;

    public static PlayerManager singleton;

    private void Start()
    {
        DontDestroyOnLoad(this);

        if (singleton == null)
        {
            singleton = this;
            SceneManager.sceneLoaded += delegate { StopListeningForInput(); };
            SceneManager.sceneLoaded += OnSceneChange;
            SceneManager.sceneLoaded += delegate { OnSceneSwitch.Invoke(); };
        }
        else
        {
            Destroy(gameObject);
            return;
        }


        Debug.Log("Listening for player input");

        ++InputUser.listenForUnpairedDeviceActivity;


        InputUser.onUnpairedDeviceUsed +=
    (control, eventPtr) =>
    {
        Debug.Log("Unpaired device found");
        // Ignore anything but button presses.
        if (!(control is ButtonControl))
            return;

        SpawnNewPlayer();
    };
    }

    public void SpawnNewPlayer()
    {
        Debug.Log("Attempting to spawn new player");
        PlayerInput spawnedPlayerInput = PlayerInput.Instantiate(controllerPrefab);
        spawnedPlayerInput.transform.SetParent(transform);
        players.Add(spawnedPlayerInput);
        int playerID = players.IndexOf(spawnedPlayerInput);

        if (spawnFighter) SpawnFighter(playerID);
        if (spawnFighterController) SpawnController(playerID);

        OnPlayerJoin.Invoke();
        Debug.Log($"Player with ID {playerID} succesfully spawned");
    }

    public void StopListeningForInput()
    {
        --InputUser.listenForUnpairedDeviceActivity;
    }

    public void SpawnController(int playerID)
    {
        PlayerJoinView playerJoinView = playerJoinViews[playerID];
        playerJoinView.backgroundImage.color = playerColors[playerID];
        playerJoinView.joinText.SetText($"Player {playerID}");
    }

    public void OnSceneChange(Scene scene, LoadSceneMode mode)
    {
        healthbars.AddRange(FindObjectsOfType<Healthbar>(true).OrderBy(m => m.transform.GetSiblingIndex()).ToArray());
        SpawnAllFighters();
    }

    public void SpawnAllFighters()
    {
        for (int i = 0; i < players.Count; i++)
        {
            SpawnFighter(i);
        }
    }

    public void SpawnFighter(int playerID)
    {
        if (playerID > players.Count) return;

        PlayerInput spawnedPlayerInput = players[playerID];

        string controlScheme = spawnedPlayerInput.currentControlScheme;
        InputDevice[] playerInputDevices = spawnedPlayerInput.devices.ToArray();
        spawnedPlayerInput.DeactivateInput();

        PlayerInput fighterInput = PlayerInput.Instantiate(fighterPrefab, -1, controlScheme, -1, playerInputDevices[0]);
        fighterInput.SwitchCurrentControlScheme(controlScheme, playerInputDevices[0]);

        GameObject fighterObject = fighterInput.gameObject;


        //Destroy(spawnedPlayerInput.gameObject);
        //players[playerID] = fighterInput;

        //Debug.Log($"{playerInputDevices[0]}");
        //fighterInput.SwitchCurrentControlScheme(playerInputDevices);
        fighterObject.name = $"Fighter {playerID}";
        fighterObject.transform.SetParent(playerParent);

        //Color the player ring
        Transform ringObject = fighterObject.transform.Find("Ring");
        if (ringObject && ringObject.TryGetComponent(out SpriteRenderer ringRenderer))
        {
            ringRenderer.color = playerColors[playerID % playerColors.Count];
        }

        //Couple healthbar
        if (healthbars.Count > playerID)
        {
            Healthbar healthbar = healthbars[playerID];
            healthbar.gameObject.SetActive(true);
            healthbar.SetColor(playerColors[playerID % playerColors.Count]);
            healthbar.SetFill(1f);
            healthbar.RecalculateHealth();
        }
    }
}
