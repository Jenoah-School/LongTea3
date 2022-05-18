using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FighterCreator : MonoBehaviour
{
    public GameObject emptyFighter;

    public List<FighterBody> fighterBodies = new List<FighterBody>();
    public List<FighterWheels> fighterWheels = new List<FighterWheels>();
    public List<FighterWeapon> fighterWeapons = new List<FighterWeapon>();
    [SerializeField] private bool spawnOnInitialisation = true;
    [SerializeField] private bool spawnDummyOnInitialisation = false;
    public static FighterCreator singleton;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        Fighter fighter;
        Fighter fighterDummy;
        if(spawnOnInitialisation) fighter = CreateNewFighter(0,0,2);
        if (spawnDummyOnInitialisation)
        {
            fighterDummy = CreateNewFighter(0, 0, 2);
            fighterDummy.transform.position = new Vector3(0, 1, 6);
        }
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

    public Fighter CreateNewFighter(int bodyIndex, int weapon1Index, int weapon2Index)
    {
        GameObject fighterObject = Instantiate(emptyFighter);
        Fighter fighter = fighterObject.GetComponent<Fighter>();
        fighter.AssembleFighterParts(fighterBodies[bodyIndex], new List<FighterWeapon>() { fighterWeapons[weapon1Index], fighterWeapons[weapon2Index] });
        return fighter;
    }
}
