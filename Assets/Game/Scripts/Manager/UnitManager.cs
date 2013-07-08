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

    // Use this for initialization
    void Start()
    {
#if UNITY_EDITOR
        UnitLog.SetName("UnitManager" + unit.Class);
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

        if (Leader)
        {
            if (currentDirection != newDirection)
            {
                Rotate();
            }
        }
        else
        {
            if (unit.Type == GameEnum.TYPEOFUNIT.HEROES)
            {
                if (currentDirection != GameManager.LeaderHeroes.GetComponent<UnitManager>().currentDirection)
                {
                    Rotate();
                }
            }
            else
            {
                //GameManager game = FindObjectOfType(typeof(GameManager)) as GameManager;
                //Debug.Log("gamemanager " + game.name);
                //foreach (var u in game.Ennemies)
                foreach (var l in GameManager.Instance.EnnemiesParty)
                {
                    foreach (GameObject g in l)
                    {
                        UnitManager UM = g.GetComponent<UnitManager>();
                        if (UM.Leader)
                        {
                            if (currentDirection != UM.currentDirection)
                            {
                                Rotate();
                            }
                        }
                    }
                }
            }
        }

        if (currentDirection != GameEnum.MOVE.NONE)
            Move();

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

                IAState = GameEnum.IASTATE.CHECK;
                break;
            case GameEnum.IASTATE.CHECK:

                IAState = GameEnum.IASTATE.MOVE;
                break;
            case GameEnum.IASTATE.MOVE:

                IAState = GameEnum.IASTATE.NONE;
                break;
            case GameEnum.IASTATE.NONE:

                IAState = GameEnum.IASTATE.EVALUATE;
                break;
            default: 
                break;
        }
    }

    void EvaluateNewDirection()
    {

    }

    void DealDamage( GameObject target )
    {
        switch (unit.Attack)
        {
            case GameEnum.TYPEOFATTACK.NONE:
                break;
            case GameEnum.TYPEOFATTACK.VALUE:
                target.GetComponent<UnitManager>().currentHealth -= unit.AttackValue;
                break;
            case GameEnum.TYPEOFATTACK.INSTANTKILL:
                target.GetComponent<UnitManager>().currentHealth -= 0;
                break;
                //Healer Case
            case GameEnum.TYPEOFATTACK.ALLTHELIFE:
                foreach (var l in GameManager.Instance.EnnemiesParty)
                {
                    if (l.Contains(this.gameObject))
                    {
                        foreach (GameObject g in l)
                        {
                            g.GetComponent<UnitManager>().currentHealth = unit.Health;
                        }
                    }
                }
                break;
            default: break;
        }   
    }

    bool InRange()
    {
        switch (unit.Range)
        {
            case GameEnum.TYPEOFRANGE.NONE:
                break;
            case GameEnum.TYPEOFRANGE.VALUE:
                foreach (var g in GameManager.Instance.Heroes)
                {
                    if (Vector3.SqrMagnitude(g.transform.position - this.gameObject.transform.position) <=
                        (unit.RangeValue * GameManager.settings.multiplierRange) * (unit.RangeValue * GameManager.settings.multiplierRange))
                        return true;
                }
                break;
            case GameEnum.TYPEOFRANGE.ALLGROUP:
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
