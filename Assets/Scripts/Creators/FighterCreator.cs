using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FighterCreator : MonoBehaviour
{
    [SerializeField] private GameObject emptyFighter;

    [SerializeField] private List<FighterBody> fighterBodies = new List<FighterBody>();
    [SerializeField] private List<FighterWheels> fighterWheels = new List<FighterWheels>();
    [SerializeField] private List<FighterWeapon> fighterWeapons = new List<FighterWeapon>();

    private void Start()
    {
        CreateNewFighter();
    }

    void CreateNewFighter()
    {
        GameObject fighterObject = Instantiate(emptyFighter);
        Fighter fighter = fighterObject.GetComponent<Fighter>();
        fighter.AssembleFighterParts(fighterBodies[0], fighterWheels[0], fighterWeapons[0]);
    }
}
