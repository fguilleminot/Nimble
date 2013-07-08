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
	public int nbEnnemiesGroupSpawned = 1;

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
			 
				if (TimeSinceSpawn >= settings.TimeBetweenSpawn && nbEnnemiesGroupSpawned <= 5)
				{
					SpawnState = GameEnum.SPAWNSTATE.SPAWN;
				}
				else
				{
					TimeSinceSpawn += Time.deltaTime;
				}
				break;
			case GameEnum.SPAWNSTATE.SPAWN:

				int MoveSpeed = 0;
				GameDev.EnnemiesParty EP = new GameDev.EnnemiesParty();
				Quaternion rotation = Quaternion.identity;
				Vector3 newSpawnPosition = Vector3.zero;
				GameEnum.MOVE orientation = GameEnum.MOVE.NONE;

				GameObject parent = new GameObject("EnnemiesTroup" + nbEnnemiesGroupSpawned);
				
				for (int i = 0; i < GameDev.RandomGenerator.Next(settings.EnnemiesTeamNumberMax) + 1; ++i)
				{
					if (i == 0)
					{
						newSpawnPosition = new Vector3(GameDev.RandomGenerator.Next(-20, 20), 1f, GameDev.RandomGenerator.Next(-20, 20));
					}
					else
					{
						switch (orientation)
						{
							case GameEnum.MOVE.NONE: break;
							case GameEnum.MOVE.UP:
								newSpawnPosition.z -= 1f;
								break;
							case GameEnum.MOVE.DOWN:
								newSpawnPosition.z += 1f;
								break;
							case GameEnum.MOVE.LEFT:
								newSpawnPosition.x -= 1f;
								break;
							case GameEnum.MOVE.RIGHT:
								newSpawnPosition.x += 1f;
								break;
							default: break;
						}
					}
					Spawn.transform.position = newSpawnPosition;

					GameObject g = Instantiate(Prefab[5], Spawn.transform.position, Quaternion.identity) as GameObject;
					g.transform.parent = parent.transform;

					UnitManager UM = g.GetComponent<UnitManager>();

					if (i == 0)
					{
						double d = GameDev.RandomGenerator.NextDouble();
						if (d <= 0.25)
						{
							g.transform.Rotate(Vector3.up, 90f);
							orientation = GameEnum.MOVE.RIGHT;
						}
						else if (d > 0.25 && d <= 0.5)
						{
							g.transform.Rotate(Vector3.up, 180f);
							orientation = GameEnum.MOVE.DOWN;
						}
						else if (d > 0.5 && d <= 0.75)
						{
							g.transform.Rotate(Vector3.up, 270f);
							orientation = GameEnum.MOVE.LEFT;
						}
						else
						{
							g.transform.Rotate(Vector3.up, 0f);
							orientation = GameEnum.MOVE.UP;
						}

						rotation = g.transform.rotation;
						
						UM.Leader = true;
						UM.newDirection = orientation;
						UM.unit = CopyStatForChampion(GameDev.RandomGenerator.Next(6,12));
						g.transform.name = UM.unit.Class.ToString() + (i + 1).ToString();
						MoveSpeed = UM.unit.MovementValue;
					}
					else
					{
						g.transform.rotation = rotation;
						UM.newDirection = orientation;
						UM.unit = CopyStatForChampion(GameDev.RandomGenerator.Next(6, 12));
						g.transform.name = UM.unit.Class.ToString() + (i + 1).ToString();

						//Debug.Log("Ms unit " + UM.unit.MovementValue + " Ms reference " + MoveSpeed);
						if (UM.unit.MovementValue < MoveSpeed)
							MoveSpeed = UM.unit.MovementValue;
					}

					EP.AddEnnemy(g);
				}

				foreach (GameObject g in EP)
				{
					g.GetComponent<UnitManager>().unit.MovementValue = MoveSpeed;
				}

				EnnemiesParty.Add(EP);

				TimeSinceSpawn = 0f;
				SpawnState = GameEnum.SPAWNSTATE.NONE;
				nbEnnemiesGroupSpawned++;

				break;
			default: break;
		}
	}

	void InitGame(GameEnum.CLASS Class = GameEnum.CLASS.NONE)
	{
		GameObject parent = new GameObject("Controlled");
		GameObject lead = Instantiate(Prefab[0], Spawn.transform.position, Quaternion.identity) as GameObject;
		UnitManager unitManager = lead.GetComponent<UnitManager>();
		unitManager.Leader = true;
		unitManager.unit.Class = Class;

		LeaderHeroes = lead;
		lead.transform.parent = parent.transform;

		Heroes.Add(lead);
	}

	GameEnum.CLASS GetClass(int ID)
	{
		switch (ID)
		{
			case -1: return GameEnum.CLASS.NONE;
			case 0: return GameEnum.CLASS.KNIGHT;
			case 1: return GameEnum.CLASS.RANGER;
			case 2: return GameEnum.CLASS.LANCER;
			case 3: return GameEnum.CLASS.MAGE;
			case 4: return GameEnum.CLASS.GUNSLINGER;
			case 5: return GameEnum.CLASS.BOMBERMAN;
			case 6: return GameEnum.CLASS.BAT;
			case 7: return GameEnum.CLASS.SPIDER;
			case 8: return GameEnum.CLASS.RAFTER;
			case 9: return GameEnum.CLASS.DARKKNIGHT;
			case 10: return GameEnum.CLASS.DARKMAGE;
			case 11: return GameEnum.CLASS.HEALER;
			default: return GameEnum.CLASS.NONE;
		}
	}

	Settings.Unit CopyStatForChampion(int ID)
	{
		Settings.Unit copy = GameManager.settings.Champion[ID];

		Settings.Unit dest = new Settings.Unit();
		dest.Type = copy.Type;
		dest.Class = copy.Class;
		dest.Attack = copy.Attack;
		dest.AttackValue = copy.AttackValue;
		dest.AttackSpeed = copy.AttackSpeed;
		dest.AttackSpeedValue = copy.AttackSpeedValue;
		dest.Fov = copy.Fov;
		dest.FovAngleValue = copy.FovAngleValue;
		dest.Movement = copy.Movement;
		dest.MovementValue = copy.MovementValue;
		dest.Range = copy.Range;
		dest.RangeValue = copy.RangeValue;
		dest.Cooldown = copy.Cooldown;
		dest.Health = copy.Health;

		return dest;
	}

	bool Win()
	{
		return (EnnemiesKilled >= settings.EnnemiesNumberToWin);
	}
}
