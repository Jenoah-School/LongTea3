using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterCreator : MonoBehaviour
{
    [SerializeField] private List<FighterBody> fighterBodies = new List<FighterBody>();
    [SerializeField] private List<FighterWheels> fighterWheels = new List<FighterWheels>();
    [SerializeField] private List<FighterWeapon> fighterWeapons = new List<FighterWeapon>();

    private void Start()
    {
        CreateNewFighter();
    }

    void CreateNewFighter()
    {
        GameObject fighterObject = new GameObject("fighter");
        Fighter fighter = fighterObject.AddComponent<Fighter>();
        fighter.AssembleFighter(fighterBodies[0], fighterWheels[0], fighterWeapons[0]);
        fighterObject.AddComponent<Rigidbody>();
    }
}
