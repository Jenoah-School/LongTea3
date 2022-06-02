using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardItem : MonoBehaviour
{
    [SerializeField] private Image previewImage;
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI rankText;
    
    public void SetPreview(int playerID)
    {
        previewImage.material = ScoreManagerProxy.singleton.GetPreviewMaterial(playerID);
    }

    public void SetName(string newName)
    {
        playerNameText.text = newName;
    }

    public void SetRank(string newRank)
    {
        rankText.text = newRank;
    }
}
