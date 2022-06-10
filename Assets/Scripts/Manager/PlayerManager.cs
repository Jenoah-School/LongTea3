using System;
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
    [SerializeField] private List<Fighter> fighters = new List<Fighter>();

    [Header("Player settings")]
    [SerializeField] private GameObject controllerPrefab = null;
    [SerializeField] private List<GameObject> spawnPoints = new List<GameObject>();
    [SerializeField] private FighterInfo defaultFighterBuild = new FighterInfo();
    [SerializeField] private bool showPlayerRing = true;

    [Header("Events")]
    [SerializeField] private bool spawnFighter = false;
    [SerializeField] private bool spawnFighterController = false;
    [SerializeField] private int spawnFighterSceneIndex = -1;
    [SerializeField] private UnityEvent OnPlayerJoin;
    [SerializeField] private UnityEvent OnSceneSwitch;

    [Header("Colors")]
    [SerializeField] private Color playerListInactiveColor = new Color(200f, 200f, 200f, 1f);
    [SerializeField] private Color playerListActiveColor = Color.white;
    [SerializeField] private List<Color> playerRingColors = new List<Color>();
    [SerializeField] private bool setHealthbarToPlayerColor = true;

    [Header("Ready")]
    [SerializeField] private bool allPlayersReady = false;
    [SerializeField] private UnityEvent onAllPlayersReady = null;
    [SerializeField] private UnityEvent onPlayersUnready = null;

    public static PlayerManager singleton;
    [SerializeField] private List<FighterInfo> fighterInfos = new List<FighterInfo>();

    Action<InputControl, UnityEngine.InputSystem.LowLevel.InputEventPtr> playerJoinEvent;


    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        if (singleton == null)
        {
            singleton = this;
            SceneManager.sceneLoaded += OnSceneChange;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        Debug.Log("Listening for player input");
        ++InputUser.listenForUnpairedDeviceActivity;

        playerJoinEvent = (control, eventPtr) =>
        {
            if (!(control is ButtonControl))
                return;

            PlayerManager.singleton.SpawnNewPlayer();
        };

        if (InputUser.listenForUnpairedDeviceActivity > 0)
        {
            InputUser.onUnpairedDeviceUsed += playerJoinEvent;
        }

        if (playerJoinViews.Count > 0)
        {
            for (int i = 0; i < playerJoinViews.Count; i++)
            {
                playerJoinViews[i].backgroundImage.color = playerListInactiveColor;
            }
        }
    }

    #region Spawning

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
        Debug.Log($"Player {playerID + 1} succesfully spawned", spawnedPlayerInput.gameObject);
    }

    public void SpawnController(int playerID)
    {
        PlayerJoinView playerJoinView = playerJoinViews[playerID];
        if (playerJoinView.TryGetComponent(out ControllerHaptics controllerHaptics))
        {
            controllerHaptics.LinkPlayerInput(players[playerID]);
        }

        MultiplayerEventSystem playerUISystem = players[playerID].GetComponent<MultiplayerEventSystem>();
        if (fighterInfos.Where(x => x.playerID == playerID).Count() <= 0)
        {
            FighterInfo fighterInfo = new FighterInfo();
            fighterInfo.playerID = playerID;
            fighterInfo.bodyID = 0;
            fighterInfo.powerupID = 0;
            fighterInfo.weaponID = 0;
            fighterInfo.ranking = 0;
            fighterInfo.fighterColor = playerRingColors[playerID % playerRingColors.Count];
            fighterInfos.Add(fighterInfo);
        }

        playerJoinView.isPlayer = true;
        playerJoinView.onReadyChange += CheckAllPlayersReady;

        playerJoinView.backgroundImage.color = playerListActiveColor;
        playerJoinView.characterSelectPanel.SetActive(true);
        playerJoinView.OnJoinEvent.Invoke();

        if (playerJoinView.playerRoot != null) playerUISystem.playerRoot = playerJoinView.playerRoot;
        if (playerJoinView.firstSelected != null)
        {
            playerUISystem.firstSelectedGameObject = playerJoinView.firstSelected;
            playerUISystem.SetSelectedGameObject(playerJoinView.firstSelected);
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
        if (fighterInfos.Count > playerID)
        {
            fighterGameObject = FighterCreator.singleton.CreateNewFighter(fighterInfos[playerID].bodyID, fighterInfos[playerID].weaponID, fighterInfos[playerID].powerupID, 0).gameObject;
        }
        else
        {
            Debug.Log("No info found about fighter");
            fighterGameObject = FighterCreator.singleton.CreateNewFighter(defaultFighterBuild.bodyID, defaultFighterBuild.weaponID, defaultFighterBuild.powerupID, 0).gameObject;
        }

        PlayerInput fighterInput = PlayerInput.Instantiate(fighterGameObject, -1, controlScheme, -1, playerInputDevices[0]);
        GameObject fighterObject = fighterInput.gameObject;
        Fighter fighter = fighterInput.GetComponent<Fighter>();
        fighters.Add(fighter);
        if (spawnPoints.Count > 0)
        {
            fighterObject.transform.SetPositionAndRotation(spawnPoints[playerID % spawnPoints.Count].transform.position, spawnPoints[playerID % spawnPoints.Count].transform.rotation);
        }
        Destroy(fighterGameObject);
        fighterInput.SwitchCurrentControlScheme(controlScheme, playerInputDevices[0]);
        fighter.PostAssemblyStart();

        fighterObject.name = $"Fighter {playerID}";
        fighterObject.transform.SetParent(fighterParent);

        //Color the player ring

        Transform ringObject = fighterObject.transform.Find("Ring");
        if (showPlayerRing)
        {
            ringObject.gameObject.SetActive(showPlayerRing);
            if (ringObject.TryGetComponent(out SpriteRenderer ringRenderer))
            {
                ringRenderer.color = playerRingColors[playerID % playerRingColors.Count];
            }
        }

        //Couple healthbar
        if (healthbars.Count > playerID)
        {
            Healthbar healthbar = healthbars[playerID];
            healthbar.SetFighter(fighter);
            healthbar.gameObject.SetActive(true);
            if (setHealthbarToPlayerColor) healthbar.SetColor(playerRingColors[playerID % playerRingColors.Count]);
            healthbar.SetFill(1f);
            healthbar.RecalculateHealth();
            fighter.onTakeDamage += healthbar.RecalculateHealth;
        }
    }

    #endregion

    #region Input

    public void StopListeningForInput()
    {
        if (InputUser.listenForUnpairedDeviceActivity > 0) --InputUser.listenForUnpairedDeviceActivity;
    }

    public void UnbindAllInput()
    {
        foreach (InputUser inputUser in InputUser.all.ToList())
        {
            inputUser.UnpairDevicesAndRemoveUser();
        }

    }

    #endregion

    #region Gameplay
    public void SafePlayerInfo()
    {
        for (int i = 0; i < fighterInfos.Count; i++)
        {
            FighterInfo fighterInfo = fighterInfos[i];
            fighterInfo.bodyID = fighterPartSelections[i].currentBodyID;
            fighterInfo.weaponID = fighterPartSelections[i].currentWeaponID;
            fighterInfo.powerupID = fighterPartSelections[i].currentPowerupID;

            fighterInfos[i] = fighterInfo;
        }
    }

    public void TogglePlayer(Fighter fighter, bool toggleState = false)
    {
        fighter.GetComponent<PlayerMovement>().enabled = toggleState;
        fighter.enabled = toggleState;
    }

    public void SetMoveStates(bool newMoveStates)
    {
        foreach (Fighter fighter in fighters)
        {
            TogglePlayer(fighter, newMoveStates);
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

    public void CheckDeathRate()
    {
        int fightersAlive = 0;
        foreach (Fighter fighter in fighters)
        {
            if (!fighter.isDead) fightersAlive++;
        }

        if (fightersAlive <= 1)
        {
            RoundManager.singleton.EndRound();
        }
    }

    public void IncreaseRankingForAlive()
    {
        for (int i = 0; i < fighters.Count; i++)
        {
            if (!fighters[i].isDead)
            {

                FighterInfo fighterInfo = fighterInfos[i];
                fighterInfo.ranking++;
                fighterInfos[i] = fighterInfo;
            }
        }
    }

    #endregion

    #region Misc

    public void OnSceneChange(Scene scene, LoadSceneMode mode)
    {
        StopListeningForInput();

        if (spawnFighterSceneIndex != -1 && scene.buildIndex == spawnFighterSceneIndex)
        {
            healthbars.AddRange(FindObjectsOfType<Healthbar>(true).OrderBy(m => m.transform.GetSiblingIndex()).ToArray());
            spawnPoints.AddRange(GameObject.FindGameObjectsWithTag("Spawnpoint").OrderBy(m => m.transform.GetSiblingIndex()).ToArray());
            Invoke(nameof(SpawnAllFighters), 0.1f);
        }

        OnSceneSwitch.Invoke();

    }

    public List<FighterInfo> GetFighterInfo()
    {
        return fighterInfos;
    }

    public void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneChange;
        if (playerJoinEvent != null) InputUser.onUnpairedDeviceUsed -= playerJoinEvent;
    }

    #endregion
}

[System.Serializable]
public struct FighterInfo
{
    public int playerID;
    public int bodyID;
    public int weaponID;
    public int powerupID;
    public int ranking;
    public Color fighterColor;
}
