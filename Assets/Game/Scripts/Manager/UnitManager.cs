using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitManager : MonoBehaviour
{

#if UNITY_EDITOR
	LogFile UnitLog = new LogFile();
	string log = "";
#endif

	public Settings.Unit unit;

	public int currentHealth;

	//per default the groupe go down
	public GameEnum.MOVE currentDirection;
	public GameEnum.MOVE newDirection;
	public GameEnum.IASTATE IAState;

	public bool Leader;

	private Vector3 positionLeaderBeforeRotate;

	private float timeBetweenAttack = -1f;
	private float timeBeforeTakeDamage = -1f;

	// Use this for initialization
	void Start()
	{
#if UNITY_EDITOR
		UnitLog.SetName(this.gameObject.transform.parent.name+"_" + this.gameObject.name);
		if (UnitLog.GetLength() > 0)
		{
			UnitLog.ClearFile();
		}
#endif
		currentHealth = unit.Health;
	}

	// Update is called once per frame
	void Update()
	{
		if (unit.Type == GameEnum.TYPEOFUNIT.ENNEMIES)
		{
			IAManage();
		}
		else
		{

			if (Leader)
			{
				if (currentDirection != newDirection)
				{
					Debug.Log(this.gameObject.name);
					Rotate();
				}
			}
			else
			{
				if (unit.Type == GameEnum.TYPEOFUNIT.HEROES)
				{
					if (currentDirection != GameManager.LeaderHeroes.GetComponent<UnitManager>().currentDirection
						&& this.transform.position == GameManager.LeaderHeroes.GetComponent<UnitManager>().positionLeaderBeforeRotate)
					{
						Rotate();
					}
				}
				else
				{
					
				}
			}
		}

		if (currentDirection != GameEnum.MOVE.NONE && CanMove())
			Move();

		if (timeBetweenAttack == 0f)
			timeBetweenAttack += Time.deltaTime;

		if (timeBeforeTakeDamage == 0f)
			timeBeforeTakeDamage += Time.deltaTime;

#if UNITY_EDITOR
		UpdateLog();
#endif
	}

	void Rotate()
	{

		switch (currentDirection)
		{
			case GameEnum.MOVE.UP:
				if (newDirection == GameEnum.MOVE.LEFT)
				{
					this.gameObject.transform.Rotate(Vector3.up, -GameManager.settings.rotationAngle);
				}
				else
				{
					this.gameObject.transform.Rotate(Vector3.up, GameManager.settings.rotationAngle);
				}
				break;
			case GameEnum.MOVE.DOWN:
				if (newDirection == GameEnum.MOVE.LEFT)
				{
					this.gameObject.transform.Rotate(Vector3.up, GameManager.settings.rotationAngle);
				}
				else
				{
					this.gameObject.transform.Rotate(Vector3.up, -GameManager.settings.rotationAngle);
				}
				break;
			case GameEnum.MOVE.LEFT:
				if (newDirection == GameEnum.MOVE.UP)
				{
					this.gameObject.transform.Rotate(Vector3.up, GameManager.settings.rotationAngle);
				}
				else
				{
					this.gameObject.transform.Rotate(Vector3.up, -GameManager.settings.rotationAngle);
				}
				break;
			case GameEnum.MOVE.RIGHT:
				if (newDirection == GameEnum.MOVE.UP)
				{
					this.gameObject.transform.Rotate(Vector3.up, -GameManager.settings.rotationAngle);
				}
				else
				{
					this.gameObject.transform.Rotate(Vector3.up, GameManager.settings.rotationAngle);
				}
				break;
			case GameEnum.MOVE.NONE:
				switch (newDirection)
				{
					case GameEnum.MOVE.DOWN:
						this.gameObject.transform.Rotate(Vector3.up, GameManager.settings.rotationAngle * 2);
						break;
					case GameEnum.MOVE.LEFT:
						this.gameObject.transform.Rotate(Vector3.up, -GameManager.settings.rotationAngle);
						break;
					case GameEnum.MOVE.RIGHT:
						this.gameObject.transform.Rotate(Vector3.up, GameManager.settings.rotationAngle);
						break;
					case GameEnum.MOVE.UP:
					default:
						break;
				}
				break;
			default: break;
		}

#if UNITY_EDITOR
		log += "Rotate the leader of the pack : \n" + currentDirection + " to " + newDirection + "\n";
#endif

		currentDirection = newDirection;
	}

	void Attack()
	{
		if (CanAttack())
		{
			GameObject target;

			if (InRange(out target))
			{
#if UNITY_EDITOR
				log += "Deal damage to : " + target;
#endif
				DealDamage(target);
			}
		}
	}

	void Move()
	{
		Vector3 pos = this.gameObject.transform.position;

		pos += this.gameObject.transform.forward * GameManager.settings.HeroSpeed * Time.deltaTime;
		this.gameObject.transform.position = pos;

	}

	void IAManage()
	{
		switch (IAState)
		{
			case GameEnum.IASTATE.EVALUATE:
				if (Leader)
				{
					EvaluateNewDirection();
				}
				else
				{
					{
						FollowLeader();
					}
				}
				IAState = GameEnum.IASTATE.CHECK;
				break;
			case GameEnum.IASTATE.CHECK:

				IAState = GameEnum.IASTATE.MOVE;
				break;
			case GameEnum.IASTATE.MOVE:
				if (Leader)
				{
					if (currentDirection != newDirection)
						Rotate();
				}
				IAState = GameEnum.IASTATE.NONE;
				break;

			case GameEnum.IASTATE.NONE:
				IAState = GameEnum.IASTATE.EVALUATE;
				break;
			default: 
				break;
		}
	}

	bool CanMove()
	{
		switch (unit.Movement)
		{
			case GameEnum.TYPEOFMOVEMENT.NONE :
				break;
			case GameEnum.TYPEOFMOVEMENT.STATIC:
				return false;
			case GameEnum.TYPEOFMOVEMENT.VALUE:
				if (unit.MovementValue > 0)
					return true;
				break;
			default: break;
		}
		return false;
	}

	void FollowLeader()
	{
		foreach (var l in GameManager.Instance.EnnemiesParty)
		{
			foreach (GameObject g in l)
			{
				UnitManager UM = g.GetComponent<UnitManager>();
				if (!UM.Leader)
				{
					if (currentDirection != UM.currentDirection
						&& this.transform.position == UM.positionLeaderBeforeRotate)
					{
						Rotate();
					}
				}
			}
		}
	}

	//Change Direction when approach wall
	void EvaluateNewDirection()
	{
		if (Leader)
		{
			//check if near wall
			Debug.DrawRay(this.transform.position, this.transform.forward, Color.yellow, 10f);
			if (Physics.Raycast(this.transform.position, this.transform.forward, 1f, GameManager.settings.WallLayer))
			{
#if UNITY_EDITOR
				log += "Change Direction too near wall";
#endif
				switch (currentDirection)
				{
					case GameEnum.MOVE.NONE:
						randDirection(5);
						break;
					case GameEnum.MOVE.UP:
					case GameEnum.MOVE.DOWN:
					case GameEnum.MOVE.LEFT:
					case GameEnum.MOVE.RIGHT:
						randDirection(4);
						break;
					default: break;
				}
			}
			else
			{
				if (GameDev.RandomGenerator.Next(100) >= 75)
				{
#if UNITY_EDITOR
					log += "Change Direction cause i want";
#endif
					switch (currentDirection)
					{
						case GameEnum.MOVE.NONE:
							randDirection(5);
							break;
						case GameEnum.MOVE.UP:
						case GameEnum.MOVE.DOWN:
						case GameEnum.MOVE.LEFT:
						case GameEnum.MOVE.RIGHT:
							randDirection(4);
							break;
						default: break;
					}
				}
			}
		}
		else
		{
			if (this.transform.position == positionLeaderBeforeRotate)
			{
				foreach (var l in GameManager.Instance.EnnemiesParty)
				{
					if (l.Contains(this.gameObject))
					{
						foreach (GameObject u in l)
						{
							if (u.GetComponent<UnitManager>().Leader)
							{
								newDirection = u.GetComponent<UnitManager>().currentDirection;
							}
						}
					}
				}
			}
		}
	}

	void randDirection(int maxRand)
	{
		if (maxRand > 5)
			maxRand = 5;
		int newDir;

		do
		{
			newDir = GameDev.RandomGenerator.Next(1, maxRand);
		} while (newDir == (int)currentDirection);

		//Debug.Log("new direction for " + this.gameObject.name + " "+(int) currentDirection + " to " + newDir);
		switch (newDir)
		{
			case 1: newDirection = GameEnum.MOVE.UP;
				break;
			case 2: newDirection = GameEnum.MOVE.DOWN;
				break;
			case 3: newDirection = GameEnum.MOVE.LEFT;
				break;
			case 4: newDirection = GameEnum.MOVE.RIGHT;
				break;
			default: newDirection = GameEnum.MOVE.NONE;
				break;
		}
	}

	void TakeDamage(int value, float timeDelay = 0f)
	{
		if (timeBeforeTakeDamage >= timeDelay)
		{
			currentHealth -= value;
			timeBeforeTakeDamage = -1f;
		}
#if UNITY_EDITOR
		log += "Variation of life, new life : " + this.currentHealth;
#endif
	}

	void DealDamage( GameObject target = null )
	{
		switch (unit.Attack)
		{
			case GameEnum.TYPEOFATTACK.NONE:
				break;
			case GameEnum.TYPEOFATTACK.VALUE:
				if (target)
					target.GetComponent<UnitManager>().TakeDamage(unit.AttackValue, DelayDamage());
					//target.GetComponent<UnitManager>().currentHealth -= unit.AttackValue;
				else
					log += "try to attack a target null";
				break;
			case GameEnum.TYPEOFATTACK.INSTANTKILL:
				if (target)
					target.GetComponent<UnitManager>().TakeDamage(target.GetComponent<UnitManager>().currentHealth, DelayDamage());
				else
					log += "try to instakill a target null";
				break;
				//Healer Case
			case GameEnum.TYPEOFATTACK.ALLTHELIFE:
				foreach (var l in GameManager.Instance.EnnemiesParty)
				{
					if (l.Contains(this.gameObject))
					{
						foreach (GameObject g in l)
						{
							UnitManager UM = g.GetComponent<UnitManager>();
							UM.TakeDamage(- (UM.unit.Health - UM.currentHealth));
						}
					}
				}
				break;
			default: break;
		}
	}

	bool CanAttack()
	{
		if (timeBetweenAttack >= unit.Cooldown)
		{
			timeBetweenAttack = -1f;
			return true;
		}
		else
			return false;
	}

	int DelayDamage()
	{
		switch (unit.AttackSpeed)
		{
			case GameEnum.TYPEOFATTACKSPEED.NONE:
			case GameEnum.TYPEOFATTACKSPEED.INSTANT:
				return 0;
			case GameEnum.TYPEOFATTACKSPEED.VALUE:
				return unit.AttackSpeedValue;
			default: break;
		}
		return 0;
	}

	void Dead()
	{
		if (currentHealth == 0)
		{
			GameManager.Instance.EnnemiesKilled++;
			Destroy(this);
		}
	}

	bool InRange(out GameObject target)
	{
		target = null;
		switch (unit.Range)
		{
			case GameEnum.TYPEOFRANGE.NONE:
				break;
			case GameEnum.TYPEOFRANGE.VALUE:
				foreach (var g in GameManager.Instance.Heroes)
				{
					if (Vector3.SqrMagnitude(g.transform.position - this.gameObject.transform.position) <=
						(unit.RangeValue * GameManager.settings.multiplierRange) * (unit.RangeValue * GameManager.settings.multiplierRange))
					{
						target = g;
						return true;
					}
				}
				break;
			case GameEnum.TYPEOFRANGE.ALLGROUP:
				return true;
			default: break;
		}
		return false;
	}

	bool InFov(GameObject target, out Vector3 direction)
	{
		direction = Vector3.zero;
		Vector3 dir = target.transform.position - this.transform.position;

		switch (unit.Fov)
		{
			case GameEnum.TYPEOFFOV.NONE:
				return false;
			case GameEnum.TYPEOFFOV.INAROW:
				
				float dotP = Vector3.Dot(dir, Vector3.up);
				if (Others.nearlyEqual(dotP, 1f, 0.1f))
				{
					direction = Vector3.up;
					return true;
				}

				dotP = Vector3.Dot(dir, Vector3.down);
				if (Others.nearlyEqual(dotP, 1f, 0.1f))
				{
					direction = Vector3.down;
					return true;
				}

				dotP = Vector3.Dot(dir, Vector3.right);
				if (Others.nearlyEqual(dotP, 1f, 0.1f))
				{
					direction = Vector3.right;
					return true;
				}

				dotP = Vector3.Dot(dir, Vector3.left);
				if (Others.nearlyEqual(dotP, 1f, 0.1f))
				{
					direction = Vector3.left;
					return true;
				}
				break;
			case GameEnum.TYPEOFFOV.ANGLE:
				return ( Vector3.Angle(target.transform.position - this.transform.position, this.transform.forward) <= unit.FovAngleValue/2 );
			case GameEnum.TYPEOFFOV.ALLGROUP:
				return true;
			default: break;
		}

		return false;
	}

#if UNITY_EDITOR
	void UpdateLog()
	{

		if (log.Length > 0)
		{
			UnitLog.WriteLine(log);
			log = "";
		}
	}
#endif
}
