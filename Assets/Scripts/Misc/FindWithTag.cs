using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindWithTag : MonoBehaviour
{
    [SerializeField] private string searchTag = "Fighter";

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] tagged = GameObject.FindGameObjectsWithTag(searchTag);
        foreach(GameObject tag in tagged)
        {
            Debug.Log($"Found {tag.name}", tag);
        }
    }
}
