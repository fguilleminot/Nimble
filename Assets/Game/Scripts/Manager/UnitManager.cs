using UnityEngine;
using System.Collections;

public class UnitManager : MonoBehaviour
{

#if UNITY_EDITOR
    LogFile UnitLog = new LogFile();
    string log = "";
#endif

    public Settings.Unit unit;
   
    //per default the groupe go down
    public GameEnum.MOVE currentDirection;
    public GameEnum.MOVE newDirection;

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
    }

    // Update is called once per frame
    void Update()
    {

        if (Leader)
        {
            if (currentDirection != newDirection)
            {
                Rotate();
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
                switch(newDirection)
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
