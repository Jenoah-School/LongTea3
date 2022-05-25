using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class ScoreManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private string leaderboardParentName = "Leaderboard list";
    [SerializeField] private GameObject leaderboardItem = null;
    [SerializeField] private float spawnDelay = .5f;
    [SerializeField] private UnityEvent OnSpawnEvent;

    List<FighterInfo> fighterInfos = new List<FighterInfo>();

    public static ScoreManager singleton;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (singleton == null)
        {
            singleton = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void SafeScores()
    {
        if (PlayerManager.singleton != null)
        {
            fighterInfos = PlayerManager.singleton.GetFighterInfo();
        }
        else
        {
            Debug.LogError("PlayerManager not found. Can't fetch scores");
        }
    }

    public void ShowScores()
    {
        StartCoroutine(ShowScoresTimed());
    }

    public IEnumerator ShowScoresTimed()
    {
        fighterInfos = fighterInfos.OrderByDescending(player => player.ranking).ToList();
        GameObject leaderboardGO = GameObject.Find(leaderboardParentName);
        if (leaderboardGO != null)
        {
            for (int i = 0; i < fighterInfos.Count(); i++)
            {
                GameObject scoreItem = Instantiate(leaderboardItem, leaderboardGO.transform);
                scoreItem.GetComponent<QuickAnimations>().Squish(.5f);
                scoreItem.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = $"Player {fighterInfos[i].playerID + 1}";
                scoreItem.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = $"{fighterInfos[i].ranking}";
                OnSpawnEvent.Invoke();
                yield return new WaitForSeconds(spawnDelay);
            }
        }
    }
}
