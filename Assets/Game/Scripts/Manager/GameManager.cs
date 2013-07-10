using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class GameManager : MonoBehaviour
{

	public static Settings settings;
	public static GameObject LeaderHeroes;
	public static GameManager Instance;

#if UNITY_EDITOR
	LogFile GameManagerLog = new LogFile();
	string log = "";
#endif

	public List<GameObject> Heroes = new List<GameObject>();
	//public List<GameObject> Ennemies = new List<GameObject>();
	public List<GameDev.EnnemiesParty> EnnemiesParty = new List<GameDev.EnnemiesParty>();

	public GameObject[] Prefab;
	public GameObject Spawn;
	private float TimeSinceSpawn = 0f;
	public GameEnum.SPAWNSTATE SpawnState;

	public int EnnemiesKilled = 0;
	public int nbEnnemiesGroupSpawned = 1;

	void Awake()
	{
#if UNITY_EDITOR
		string[] files;
		if (Directory.Exists("Assets/Log/"))
		{
			files = Directory.GetFiles("Assets/Log/");

			foreach (var s in files)
			{
				try
				{
					File.Delete(s);
				}
				catch
				{
					Debug.LogError("FAIL DELETE LOG");
				}
			}
		}
#endif
	}

	// Use this for initialization
	void Start()
	{
		settings = GetComponent<Settings>();
		InitGame(GameEnum.CLASS.KNIGHT);
		Instance = this;

#if UNITY_EDITOR
		GameManagerLog.SetName(this.gameObject.name);
		if (GameManagerLog.GetLength() > 0)
		{
			GameManagerLog.ClearFile();
		}
#endif
	}

	// Update is called once per frame
	void Update()
	{

		switch (GameState())
		{
			case GameEnum.GAMESTATE.INGAME:
				ManageSpawn();
				break;
			case GameEnum.GAMESTATE.WIN:
				DrawReward(GameEnum.GAMESTATE.WIN);
				break;
			case GameEnum.GAMESTATE.LOOSE:
				DrawReward(GameEnum.GAMESTATE.LOOSE);
				break;
		}

#if UNITY_EDITOR
		UpdateLog();
#endif
	}

	void ManageSpawn()
	{
		switch (SpawnState)
		{
			case GameEnum.SPAWNSTATE.NONE:

				if (TimeSinceSpawn >= settings.TimeBetweenSpawn && nbEnnemiesGroupSpawned <= settings.EnnemiesMaxTeamOnMap)
				{
					SpawnState = GameEnum.SPAWNSTATE.SPAWN;
				}
				else
				{
					TimeSinceSpawn += Time.deltaTime;
				}
				break;
			case GameEnum.SPAWNSTATE.SPAWN:
#if UNITY_EDITOR
				log += "\nSpawn: \n";
#endif
				GoSpawn();

				break;
			default: break;
		}
	}

	void InitGame(GameEnum.CLASS Class = GameEnum.CLASS.NONE)
	{
		GameObject parent = new GameObject("Controlled");
		GameObject lead = Instantiate(Prefab[0], Spawn.transform.position, Quaternion.identity) as GameObject;
		lead.name = "PlayerLeader";
		UnitManager UM = lead.GetComponent<UnitManager>();

		UM.unit = CopyStatForChampion((int)Class);
		UM.Leader = true;
		UM.HumanControlled = true;

		LeaderHeroes = lead;
		lead.transform.parent = parent.transform;

#if UNITY_EDITOR
		log += "Nom : " + lead.transform.name + " Position : " + lead.transform.position;
#endif
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

	GameEnum.GAMESTATE GameState()
	{
		if (EnnemiesKilled >= settings.EnnemiesNumberToWin)
			return GameEnum.GAMESTATE.WIN;

		if (Physics.Raycast(LeaderHeroes.transform.position, LeaderHeroes.transform.forward, 0.5f, settings.WallLayer))
			return GameEnum.GAMESTATE.LOOSE;

		return GameEnum.GAMESTATE.INGAME;
	}

	void DrawReward(GameEnum.GAMESTATE reward)
	{

	}

	public GameDev.EnnemiesParty GetEnnemiesParty(GameObject child)
	{
		GameDev.EnnemiesParty EP = null;

		foreach (GameDev.EnnemiesParty ep in this.EnnemiesParty)
		{
			if (ep.Contains(child))
			{
				EP = ep;
			}
		}

		return EP;
	}

	void GoSpawn()
	{
		int MoveSpeed = 0;
		GameDev.EnnemiesParty EP = new GameDev.EnnemiesParty();
		Vector3 newSpawnPosition = Vector3.zero;
		GameEnum.MOVE orientation = GameEnum.MOVE.NONE;
		GameObject firstUnit;
		int maxEnnemiesInGroup;

		GameObject parent = new GameObject("EnnemiesTroup" + nbEnnemiesGroupSpawned);

		//Choose the class Before for
		int unitID = GameDev.RandomGenerator.Next(6, 12);

		if (unitID == (int)GameEnum.CLASS.RAFTER || (unitID == (int)GameEnum.CLASS.BAT && GameDev.RandomGenerator.NextDouble() < 0.9))
			maxEnnemiesInGroup = 0;
		else if (unitID == (int)GameEnum.CLASS.HEALER)
			maxEnnemiesInGroup = GameDev.RandomGenerator.Next(2, settings.EnnemiesTeamNumberMax);
		else
			maxEnnemiesInGroup = GameDev.RandomGenerator.Next(1, settings.EnnemiesTeamNumberMax);

		newSpawnPosition = new Vector3(GameDev.RandomGenerator.Next(-20, 20), 1f, GameDev.RandomGenerator.Next(-20, 20));
		orientation = RandomOrientation();

		firstUnit = InstantiateUnit(parent, 1, newSpawnPosition, orientation, unitID, true);
		MoveSpeed = firstUnit.GetComponent<UnitManager>().unit.MovementValue;
		EP.AddEnnemy(firstUnit);

		for (int i = 0; i < maxEnnemiesInGroup; ++i)
		{
			do
			{
				unitID = GameDev.RandomGenerator.Next(6, 12);
			} while (unitID == (int)GameEnum.CLASS.RAFTER || (unitID == (int)GameEnum.CLASS.BAT && GameDev.RandomGenerator.NextDouble() < 0.9));

			switch (orientation)
			{
				case GameEnum.MOVE.NONE: break;
				case GameEnum.MOVE.UP:
					newSpawnPosition.z -= 2f;
					break;
				case GameEnum.MOVE.DOWN:
					newSpawnPosition.z += 2f;
					break;
				case GameEnum.MOVE.LEFT:
					newSpawnPosition.x += 2f;
					break;
				case GameEnum.MOVE.RIGHT:
					newSpawnPosition.x -= 2f;
					break;
				default: break;
			}
			Spawn.transform.position = newSpawnPosition;
			EP.AddEnnemy(InstantiateUnit(parent, i + 2, newSpawnPosition, orientation, unitID));
		}

		foreach (GameObject g in EP)
		{
			UnitManager UM = g.GetComponent<UnitManager>();
			if (UM.unit.MovementValue < MoveSpeed)
				MoveSpeed = UM.unit.MovementValue;
		}

		foreach (GameObject g in EP)
		{
			g.GetComponent<UnitManager>().unit.MovementValue =	MoveSpeed;
		}

		EnnemiesParty.Add(EP);

		TimeSinceSpawn = 0f;
		SpawnState = GameEnum.SPAWNSTATE.NONE;
		nbEnnemiesGroupSpawned++;
	}

	GameEnum.MOVE RandomOrientation()
	{
		GameEnum.MOVE orientation;
		double d = GameDev.RandomGenerator.NextDouble();
		if (d <= 0.25)
		{
			orientation = GameEnum.MOVE.RIGHT;
		}
		else if (d > 0.25 && d <= 0.5)
		{
			orientation = GameEnum.MOVE.DOWN;
		}
		else if (d > 0.5 && d <= 0.75)
		{
			orientation = GameEnum.MOVE.LEFT;
		}
		else
		{
			orientation = GameEnum.MOVE.UP;
		}
		return orientation;
	}

	GameObject InstantiateUnit(GameObject parent, int groupID, Vector3 pos, GameEnum.MOVE orientation, int unitID, bool leader = false)
	{

		GameObject g = Instantiate(Prefab[5]) as GameObject;
		g.transform.parent = parent.transform;
		g.transform.position = pos;
		g.transform.rotation = Quaternion.identity;

		UnitManager UM = g.GetComponent<UnitManager>();

		UM.Leader = leader;
		UM.newDirection = orientation;
		UM.unit = CopyStatForChampion(unitID);
		g.transform.name = UM.unit.Class.ToString() + groupID.ToString();

#if UNITY_EDITOR
		log += "Nom : " + g.transform.name + " Position : " + g.transform.position + " Orientation " + orientation + "\n";
#endif
		return g;
	}

#if UNITY_EDITOR
	void UpdateLog()
	{

		if (log.Length > 0)
		{
			GameManagerLog.WriteLine(log);
			log = "";
		}
	}
#endif
}
