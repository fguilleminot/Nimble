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

	public bool HumanControlled = false;
	public bool Leader;

	private GameDev.ChangeDirection ChangeDir = new GameDev.ChangeDirection();

	public int NbChangeDirectionDone = 0;
	private float timeBetweenAttack = -1f;
	private float timeBeforeTakeDamage = -1f;

	// Use this for initialization
	void Start()
	{
#if UNITY_EDITOR
		UnitLog.SetName(this.gameObject.transform.parent.name + "_" + this.gameObject.name);
		if (UnitLog.GetLength() > 0)
		{
			UnitLog.ClearFile();
		}
		log += "Leader : " + Leader + "\n";
		log += "Type : " + unit.Type + "\n";
		log += "Class : " + unit.Class + "\n";
		log += "Attack : " + unit.Attack + "\t";
		log += "AttackValue : " + unit.AttackValue + "\n";
		log += "Range :" + unit.Range + "\t";
		log += "RangeValue :" + unit.RangeValue + "\n";
		log += "Fov :" + unit.Fov + "\t";
		log += "FovAngleValue :" + unit.FovAngleValue + "\n";
		log += "AttackSpeed :" + unit.AttackSpeed + "\t";
		log += "AttackSpeedValue :" + unit.AttackSpeedValue + "\n";
		log += "Movement :" + unit.Movement + "\t";
		log += "MovementValue :" + unit.MovementValue + "\n";
		log += "Cooldown :" + unit.Cooldown + "\n";
		log += "Health :" + unit.Health + "\n";
#endif
		currentHealth = unit.Health;
	}

	// Update is called once per frame
	void Update()
	{
		Dead();

		if (!HumanControlled)
		{
			IAManage();
		}

		if (currentDirection != newDirection)
		{
			if (Leader)
			{
				ChangeDir.Position = this.transform.position;
				ChangeDir.NewDir = newDirection;
				if (!HumanControlled)
					GameManager.Instance.GetEnnemiesParty(this.gameObject).ChangeDir.Add(ChangeDir);
			}
			Rotate();
		}

		if (currentDirection != GameEnum.MOVE.NONE && CanMove())
			Move();

		Attack();

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
		if (Leader)
			log += "Rotate the leader of the pack : \n" + currentDirection + " to " + newDirection + "\n";
#endif

		currentDirection = newDirection;
	}

	void Attack()
	{
		if (CanAttack())
		{
			GameObject target;
			Debug.Log("je peux attaquer");
			if (InRange(out target))
			{
#if UNITY_EDITOR
				log += "Deal damage to : " + target;
#endif
				Vector3 direction;
				Debug.Log("j'ai qq'un à portée");
				if (InFov(target, out direction))
				{
					DealDamage(target);
				}
			}
		}
	}

	void Move()
	{
		Vector3 pos = this.gameObject.transform.position;

		pos += this.gameObject.transform.forward * this.unit.MovementValue * Time.deltaTime;
		this.gameObject.transform.position = pos;

	}

	void IAManage()
	{
		switch (IAState)
		{
			case GameEnum.IASTATE.EVALUATE:
				if (!HumanControlled && Leader)
				{
					EvaluateNewDirection();
				}

				if (!Leader)
				{
					FollowLeader();
				}
				IAState = GameEnum.IASTATE.CHECK;
				break;
			case GameEnum.IASTATE.CHECK:
				IAState = GameEnum.IASTATE.MOVE;
				break;
			case GameEnum.IASTATE.MOVE:
				//if (currentDirection != newDirection)
				//{
				//	if (Leader)
				//		positionLeaderBeforeRotate = this.transform.position;
				//	Rotate();
				//}
				IAState = GameEnum.IASTATE.NONE;
				break;

			case GameEnum.IASTATE.NONE:
				IAState = GameEnum.IASTATE.EVALUATE;
				break;
			default:
				break;
		}
	}

	void FollowLeader()
	{
		//Get My Team
		GameDev.EnnemiesParty EP = GameManager.Instance.GetEnnemiesParty(this.gameObject);

		if (EP.ChangeDir.Count <= NbChangeDirectionDone)
			return;
#if UNITY_EDITOR
		//Get My Lead
		GameObject MyLead = EP.GetLeaderOfPack();
#endif

		GameDev.ChangeDirection CD = EP.ChangeDir[NbChangeDirectionDone];

#if UNITY_EDITOR
		log += "Leader : " + MyLead.name + " ChangeDirection : " + CD.Position + " -> " + CD.NewDir;
		log += "\n : MyPos " + this.transform.position + "\n";
#endif
		if (Others.CompareVectors(this.transform.position, CD.Position, 1f))
		{
			Debug.Log("TURN from " + newDirection + " to " + CD.NewDir);
			this.transform.position = CD.Position;
			newDirection = CD.NewDir;
			NbChangeDirectionDone++;
#if UNITY_EDITOR
			log += "I Change Direction " + newDirection;
#endif
		}


		//UnitManager UM = GameManager.Instance.GetEnnemiesParty(this.gameObject).GetLeaderOfPack().GetComponent<UnitManager>();
		//if (this.currentDirection != UM.currentDirection)
		//{
		//	log += "My Lead Change his direction";
		//	if (this.transform.position == UM.positionLeaderBeforeRotate)
		//	{
		//		newDirection = UM.currentDirection;
		//	}
		//}
	}

	bool CanMove()
	{
		switch (unit.Movement)
		{
			case GameEnum.TYPEOFMOVEMENT.NONE:
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
						newDirection = randDirection(5);
						break;
					case GameEnum.MOVE.UP:
					case GameEnum.MOVE.DOWN:
					case GameEnum.MOVE.LEFT:
					case GameEnum.MOVE.RIGHT:
						do
						{
						newDirection = randDirection(4);
						}while(!CheckNewDirection());
						break;
					default: break;
				}
			}
			else
			{
				if (GameDev.RandomGenerator.Next(100) >= 99)
				{
#if UNITY_EDITOR
					log += "Change Direction cause i want";
#endif
					switch (currentDirection)
					{
						case GameEnum.MOVE.NONE:
							newDirection = randDirection(5);
							break;
						case GameEnum.MOVE.UP:
						case GameEnum.MOVE.DOWN:
						case GameEnum.MOVE.LEFT:
						case GameEnum.MOVE.RIGHT:
							newDirection = randDirection(4);
							break;
						default: break;
					}
				}
			}
		}
	}

	GameEnum.MOVE randDirection(int maxRand)
	{
		if (maxRand > 5)
			maxRand = 5;
		int newDir;

		do
		{
			newDir = GameDev.RandomGenerator.Next(1, maxRand);
		} while (newDir == (int)currentDirection && !CheckDirectionMove());

		//Debug.Log("new direction for " + this.gameObject.name + " "+(int) currentDirection + " to " + newDir);
		switch (newDir)
		{
			case 1: return GameEnum.MOVE.UP;
			case 2: return GameEnum.MOVE.DOWN;
			case 3: return GameEnum.MOVE.LEFT;
			case 4: return GameEnum.MOVE.RIGHT;
			default: return GameEnum.MOVE.NONE;
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
		log += "Variation of life, new life : " + this.currentHealth+"\n";
#endif
	}

	void DealDamage(GameObject target = null)
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
					log += "try to attack a target null\n";
				break;
			case GameEnum.TYPEOFATTACK.INSTANTKILL:
				if (target)
					target.GetComponent<UnitManager>().TakeDamage(target.GetComponent<UnitManager>().currentHealth, DelayDamage());
				else
					log += "try to instakill a target null\n";
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
							UM.TakeDamage(-(UM.unit.Health - UM.currentHealth));
						}
					}
				}
				break;
			default: break;
		}
	}

	bool CanAttack()
	{
		if (timeBetweenAttack == -1f)
			return true;
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
				if (this.unit.Type == GameEnum.TYPEOFUNIT.ENNEMIES)
				{
					foreach (var g in GameManager.Instance.Heroes)
					{
						if (Vector3.SqrMagnitude(g.transform.position - this.gameObject.transform.position) <=
							(unit.RangeValue * GameManager.settings.multiplierRange) * (unit.RangeValue * GameManager.settings.multiplierRange))
						{
							target = g;
							return true;
						}
					}
				}
				else
				{
					foreach (var EP in GameManager.Instance.EnnemiesParty)
					{
						foreach (GameObject g in EP)
						{
							if (Vector3.SqrMagnitude(g.transform.position - this.gameObject.transform.position) <=
								(unit.RangeValue * GameManager.settings.multiplierRange) * (unit.RangeValue * GameManager.settings.multiplierRange))
							{
								target = g;
								return true;
							}
						}
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

		if (target == null)
		{
			return true;
		}
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
				return (Vector3.Angle(target.transform.position - this.transform.position, this.transform.forward) <= unit.FovAngleValue / 2);
			case GameEnum.TYPEOFFOV.ALLGROUP:
				return true;
			default: break;
		}

		return false;
	}

	public bool CheckNewDirection()
	{
		switch (newDirection)
		{
			case GameEnum.MOVE.UP:
				return !(Physics.Raycast(this.transform.position, Vector3.forward, 1f, GameManager.settings.WallLayer));
			case GameEnum.MOVE.DOWN:
				return !(Physics.Raycast(this.transform.position, Vector3.back, 1f, GameManager.settings.WallLayer));
			case GameEnum.MOVE.RIGHT:
				return !(Physics.Raycast(this.transform.position, Vector3.right, 1f, GameManager.settings.WallLayer));
			case GameEnum.MOVE.LEFT:
				return !(Physics.Raycast(this.transform.position, Vector3.left, 1f, GameManager.settings.WallLayer));
			case GameEnum.MOVE.NONE:
				return true;
			default:
				break;
		}
		return false;
	}

	public bool CheckDirectionMove()
	{
		switch (currentDirection)
		{
			case GameEnum.MOVE.UP:
				return !(newDirection == GameEnum.MOVE.DOWN);
			case GameEnum.MOVE.DOWN:
				return !(newDirection == GameEnum.MOVE.UP);
			case GameEnum.MOVE.RIGHT:
				return !(newDirection == GameEnum.MOVE.LEFT);
			case GameEnum.MOVE.LEFT:
				return !(newDirection == GameEnum.MOVE.RIGHT);
			case GameEnum.MOVE.NONE:
				return true;
			default:
				break;
		}
		return false;
	}

	public bool CheckDirectionMove(int dir)
	{
		switch (currentDirection)
		{
			case GameEnum.MOVE.UP:
				return !(dir == (int)GameEnum.MOVE.DOWN);
			case GameEnum.MOVE.DOWN:
				return !(dir == (int)GameEnum.MOVE.UP);
			case GameEnum.MOVE.RIGHT:
				return !(dir == (int)GameEnum.MOVE.LEFT);
			case GameEnum.MOVE.LEFT:
				return !(dir == (int)GameEnum.MOVE.RIGHT);
			case GameEnum.MOVE.NONE:
				return true;
			default:
				break;
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
