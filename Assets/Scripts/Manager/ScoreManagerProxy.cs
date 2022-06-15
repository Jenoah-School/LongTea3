using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManagerProxy : MonoBehaviour
{
    [SerializeField] private List<Fighter> previewGameObjects = new List<Fighter>();
    [SerializeField] private List<Material> previewMaterials = new List<Material>();

    public static ScoreManagerProxy singleton;

    private void Awake()
    {
        singleton = this;
    }

    public void ShowScores()
    {
        if (ScoreManager.singleton != null)
        {
            ScoreManager.singleton.ShowScores();
        }
        else
        {
            Debug.LogError("Score manager doesn't exist. Cannot fetch scores");
        }
    }

    public void DestroyScoreManager()
    {
        if (ScoreManager.singleton) Destroy(ScoreManager.singleton.gameObject);
    }

    public Material GetPreviewMaterial(int playerID)
    {
        if (previewMaterials[playerID] != null) return previewMaterials[playerID];
        return null;
    }

    public void BuildPreview(int playerID)
    {
            foreach (Transform child in previewGameObjects[playerID].transform)
            {
                Destroy(child.gameObject);
            }

        List<FighterWeapon> fighterWeapons = new List<FighterWeapon>();
        fighterWeapons.Add(FighterCreator.singleton.fighterWeapons[ScoreManager.singleton.fighterInfos[playerID].rangedWeaponID]);
        previewGameObjects[playerID].AssembleFighterParts(FighterCreator.singleton.fighterBodies[ScoreManager.singleton.fighterInfos[playerID].bodyID], fighterWeapons, FighterCreator.singleton.fighterPowerups[ScoreManager.singleton.fighterInfos[playerID].powerupID]);
    }
}
