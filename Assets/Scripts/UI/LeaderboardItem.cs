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

    public void SetRank(int newRank)
    {
        string suffix;
        switch (newRank)
        {
            case 1:
                suffix = "st";
                break;
            case 2:
                suffix = "nd";
                break;
            case 3:
                suffix = "rd";
                break;
            case 4:
                suffix = "th";
                break;
            default:
                suffix = "";
                break;
        }
        rankText.text = newRank + suffix;
    }
}
