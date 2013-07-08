using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

    public static Settings settings;
    public static GameObject LeaderHeroes;
    public static GameManager Instance;

    public List<GameObject> Heroes = new List<GameObject>();
    //public List<GameObject> Ennemies = new List<GameObject>();
    public List<GameDev.EnnemiesParty> EnnemiesParty = new List<GameDev.EnnemiesParty>();
    
    public GameObject[] Prefab;
	public GameObject Spawn;
    private float TimeSinceSpawn = 0f;
    public GameEnum.SPAWNSTATE SpawnState;

    public int EnnemiesKilled = 0;

	// Use this for initialization
	void Start () {
	    settings = GetComponent<Settings>();
        InitGame(GameEnum.CLASS.KNIGHT);
        Instance = this;
	}
	
	// Update is called once per frame
	void Update () {
        ManageSpawn();
	}

    void ManageSpawn()
    {
        switch (SpawnState)
        {
            case GameEnum.SPAWNSTATE.NONE:
             
                if (Others.nearlyEqual(TimeSinceSpawn, settings.TimeBetweenSpawn, 0.01f))
                {
                    SpawnState = GameEnum.SPAWNSTATE.SPAWN;
                }
                else
                {
                    TimeSinceSpawn += Time.deltaTime;
                }
                break;
            case GameEnum.SPAWNSTATE.SPAWN:

                GameDev.EnnemiesParty EP = new GameDev.EnnemiesParty();
                for (int i = 0; i < GameDev.RandomGenerator.Next(settings.EnnemiesTeamNumberMax); ++i)
                {
                    Vector3 newSpawnPosition = new Vector3((float)GameDev.RandomGenerator.NextDouble() * 20, 1f, (float)GameDev.RandomGenerator.NextDouble() * 20);
                    Spawn.transform.position = newSpawnPosition;

                    GameObject g = Instantiate(Prefab[5], Spawn.transform.position, Quaternion.identity) as GameObject;

                    double d = GameDev.RandomGenerator.NextDouble();
                    if (d <= 0.25)
                        g.transform.Rotate(Vector3.up, 90f);
                    else if (d > 0.25 && d <= 0.5)
                        g.transform.Rotate(Vector3.up, 180f);
                    else if (d > 0.5 && d <= 0.75)
                        g.transform.Rotate(Vector3.up, 270f);
                    else
                        g.transform.Rotate(Vector3.up, 0f);

                    EP.AddEnnemy(g);
                }

                EnnemiesParty.Add(EP);

                TimeSinceSpawn = 0f;
                SpawnState = GameEnum.SPAWNSTATE.NONE;
                break;
            default: break;
        }
    }

    void InitGame(GameEnum.CLASS Class = GameEnum.CLASS.NONE)
    {
        GameObject lead = Instantiate(Prefab[0], Spawn.transform.position, Quaternion.identity) as GameObject;
        UnitManager unitManager = lead.GetComponent<UnitManager>();
        unitManager.Leader = true;
        unitManager.unit.Class = Class;

        LeaderHeroes = lead;

        Heroes.Add(lead);
    }

}
