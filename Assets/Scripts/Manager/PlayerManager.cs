using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem.Users;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform fighterParent = null;
    [SerializeField] private List<Healthbar> healthbars = new List<Healthbar>();
    [SerializeField] private List<PlayerJoinView> playerJoinViews = new List<PlayerJoinView>();
    [SerializeField] private List<FighterPartSelection> fighterPartSelections = new List<FighterPartSelection>();
    [SerializeField] private List<PlayerInput> players = new List<PlayerInput>();

    [Header("Player settings")]
    [SerializeField] private GameObject controllerPrefab = null;
    [SerializeField] private List<Color> playerColors = new List<Color>();
    [SerializeField] private List<GameObject> spawnPoints = new List<GameObject>();

    [Header("Events")]
    [SerializeField] private bool spawnFighter = false;
    [SerializeField] private bool spawnFighterController = false;
    [SerializeField] private int spawnFighterSceneIndex = -1;
    [SerializeField] private UnityEvent OnPlayerJoin;
    [SerializeField] private UnityEvent OnSceneSwitch;

    [Header("Ready")]
    [SerializeField] private bool allPlayersReady = false;
    [SerializeField] private UnityEvent onAllPlayersReady = null;
    [SerializeField] private UnityEvent onPlayersUnready = null;

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

        Debug.Log("Listening for new player input");

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
        PlayerInput spawnedPlayerInput = PlayerInput.Instantiate(controllerPrefab);
        //If weapons don't work anymore, try to comment out the line below
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
        MultiplayerEventSystem playerUISystem = players[playerID].GetComponent<MultiplayerEventSystem>();

        playerJoinView.isPlayer = true;
        playerJoinView.onReadyChange += CheckAllPlayersReady;

        playerJoinView.backgroundImage.color = playerColors[playerID];
        playerJoinView.characterSelectPanel.SetActive(true);
        playerJoinView.OnJoinEvent.Invoke();

        if (playerJoinView.playerRoot != null) playerUISystem.playerRoot = playerJoinView.playerRoot;
        if (playerJoinView.firstSelected != null)
        {
            playerUISystem.firstSelectedGameObject = playerJoinView.firstSelected;
            playerUISystem.SetSelectedGameObject(playerJoinView.firstSelected);
        }

    }

    public void CheckAllPlayersReady()
    {
        foreach (PlayerJoinView playerJoinView in playerJoinViews)
        {
            if (playerJoinView.isPlayer && !playerJoinView.isReady)
            {
                if (allPlayersReady)
                {
                    allPlayersReady = false;
                    onPlayersUnready.Invoke();
                }
                return;
            }
        }

        if (!allPlayersReady)
        {
            allPlayersReady = true;
            onAllPlayersReady.Invoke();
        }
    }

    public void OnSceneChange(Scene scene, LoadSceneMode mode)
    {
        if (spawnFighterSceneIndex != -1 && scene.buildIndex == spawnFighterSceneIndex)
        {
            healthbars.AddRange(FindObjectsOfType<Healthbar>(true).OrderBy(m => m.transform.GetSiblingIndex()).ToArray());
            spawnPoints.AddRange(GameObject.FindGameObjectsWithTag("Spawnpoint").OrderBy(m => m.transform.GetSiblingIndex()).ToArray());
            SpawnAllFighters();
        }
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

        GameObject fighterGameObject;
        if(fighterPartSelections.Count > playerID && fighterPartSelections[playerID] != null)
        {
            fighterGameObject = FighterCreator.singleton.CreateNewFighter(fighterPartSelections[playerID].currentBodyIndex, 0, fighterPartSelections[playerID].currentWeaponIndex, fighterPartSelections[playerID].currentPowerupIndex).gameObject;
        }
        else
        {
            fighterGameObject = FighterCreator.singleton.CreateNewFighter(0, 0, 0, 1).gameObject;
        }
        
        PlayerInput fighterInput = PlayerInput.Instantiate(fighterGameObject, -1, controlScheme, -1, playerInputDevices[0]);
        GameObject fighterObject = fighterInput.gameObject;
        if (spawnPoints.Count > 0)
        {
            fighterObject.transform.SetPositionAndRotation(spawnPoints[playerID % spawnPoints.Count].transform.position, spawnPoints[playerID % spawnPoints.Count].transform.rotation);
        }
        Destroy(fighterGameObject);
        fighterInput.SwitchCurrentControlScheme(controlScheme, playerInputDevices[0]);
        fighterInput.GetComponent<Fighter>().PostAssemblyStart();

        fighterObject.name = $"Fighter {playerID}";
        fighterObject.transform.SetParent(fighterParent);

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
            healthbar.SetFighterParts(fighterObject.GetComponentsInChildren<FighterPart>().ToList());
            healthbar.gameObject.SetActive(true);
            healthbar.SetColor(playerColors[playerID % playerColors.Count]);
            healthbar.SetFill(1f);
            healthbar.RecalculateHealth();
        }
    }
}
