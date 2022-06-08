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

    [HideInInspector] public List<FighterInfo> fighterInfos = new List<FighterInfo>();

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
                LeaderboardItem scoreItem = Instantiate(leaderboardItem, leaderboardGO.transform).GetComponent<LeaderboardItem>();
                scoreItem.SetPreview(fighterInfos[i].playerID);
                ScoreManagerProxy.singleton.BuildPreview(fighterInfos[i].playerID);
                scoreItem.GetComponent<QuickAnimations>().Squish(.5f);
                scoreItem.SetName($"Player {fighterInfos[i].playerID + 1}");
                scoreItem.SetRank(fighterInfos.Count() - fighterInfos[i].ranking);
                OnSpawnEvent.Invoke();
                yield return new WaitForSeconds(spawnDelay);
            }
        }
    }
}
