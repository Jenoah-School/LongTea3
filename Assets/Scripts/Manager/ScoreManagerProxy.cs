using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManagerProxy : MonoBehaviour
{
    public void ShowScores()
    {
        if(ScoreManager.singleton != null)
        {
            ScoreManager.singleton.ShowScores();
        }
        else
        {
            Debug.LogError("Score manager doesn't exist. Cannot fetch scores");
        }
    }
}
