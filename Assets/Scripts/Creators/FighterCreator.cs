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
    [SerializeField] private bool spawnOnInitialisation = true;
    public static FighterCreator singleton;

    private void Start()
    {
        DontDestroyOnLoad(this);
        Fighter fighter;
        if(spawnOnInitialisation) fighter = CreateNewFighter(0,0,1,0);
        if(singleton == null)
        {
            singleton = this;
        }
        else
        {
            Destroy(singleton);
            return;
        }
    }

    public Fighter CreateNewFighter(int bodyIndex, int wheelIndex, int weaponIndex)
    {
        GameObject fighterObject = Instantiate(emptyFighter);
        Fighter fighter = fighterObject.GetComponent<Fighter>();
        fighter.AssembleFighterParts(fighterBodies[bodyIndex], fighterWheels[wheelIndex], fighterWeapons[weaponIndex]);
        return fighter;
    }

    public Fighter CreateNewFighter(int bodyIndex, int wheelIndex, int weapon1Index, int weapon2Index)
    {
        GameObject fighterObject = Instantiate(emptyFighter);
        Fighter fighter = fighterObject.GetComponent<Fighter>();
        fighter.AssembleFighterParts(fighterBodies[bodyIndex], fighterWheels[wheelIndex], fighterWeapons[weapon1Index], fighterWeapons[weapon2Index]);
        return fighter;
    }
}
