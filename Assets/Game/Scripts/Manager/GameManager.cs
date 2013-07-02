using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

    public List<GameObject> Heroes = new List<GameObject>();
    public List<GameObject> Ennemies = new List<GameObject>();
    public GameObject[] Prefab;
    public GameObject Spawn;

    public static Settings settings;
    public static GameObject Leader;

	// Use this for initialization
	void Start () {
	    settings = GetComponent<Settings>();
        InitGame(GameEnum.CLASS.KNIGHT);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void InitGame(GameEnum.CLASS Class = GameEnum.CLASS.NONE)
    {
        GameObject lead = Instantiate(Prefab[0], Spawn.transform.position, Quaternion.identity) as GameObject;
        UnitManager unitManager = lead.GetComponent<UnitManager>();
        unitManager.Leader = true;
        unitManager.unit.Class = Class;

        Leader = lead;
    }
}
