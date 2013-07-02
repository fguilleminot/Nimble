using UnityEngine;
using System.Collections;

public class InputManager : GameManager
{

    GameEnum.INPUTSTATE state = GameEnum.INPUTSTATE.NONE;
    GameEnum.EVALUATEINPUT typeOfInput = GameEnum.EVALUATEINPUT.NONE;
    GameEnum.MOVE direction = GameEnum.MOVE.NONE;

    public Vector3 oldMousePosition;
    public Vector3 newMousePosition;
    public bool LeftClickPressed = false;

    //Settings settings;

#if UNITY_EDITOR
    LogFile InputLog = new LogFile();
    string log = "";
#endif
    // Use this for initialization
    void Start()
    {
        //settings = GetComponent<Settings>();

#if UNITY_EDITOR
        InputLog.SetName("InputManager");
        if (InputLog.GetLength() > 0)
        {
            InputLog.ClearFile();
        }
#endif
    }

    // Update is called once per frame
    void Update()
    {
        Manage();
#if UNITY_EDITOR
        UpdateLog();
#endif
    }

    void Manage()
    {
        switch (state)
        {
            case GameEnum.INPUTSTATE.NONE:
                state = GameEnum.INPUTSTATE.EVALUATE;
                break;
            case GameEnum.INPUTSTATE.EVALUATE:
                MouseDetectionMove();
                if (typeOfInput == GameEnum.EVALUATEINPUT.NONE)
                    KeyBoardDetectionMove();

                if (direction != GameEnum.MOVE.NONE)
                    state = GameEnum.INPUTSTATE.CHECK;
                break;
            case GameEnum.INPUTSTATE.CHECK:
                if (CheckDirectionMove())
                    state = GameEnum.INPUTSTATE.MOVE;
                break;
            case GameEnum.INPUTSTATE.MOVE:

                Leader.GetComponent<UnitManager>().newDirection = direction;

                direction = GameEnum.MOVE.NONE;
                typeOfInput = GameEnum.EVALUATEINPUT.NONE;
                state = GameEnum.INPUTSTATE.NONE;
                break;
            default: break;
        }

    }

    bool CheckDirectionMove()
    {
        GameEnum.MOVE current = Leader.GetComponent<UnitManager>().currentDirection;

        switch (current)
        {
            case GameEnum.MOVE.UP:
                return !(direction == GameEnum.MOVE.DOWN);
            case GameEnum.MOVE.DOWN:
                return !(direction == GameEnum.MOVE.UP);
            case GameEnum.MOVE.RIGHT:
                return !(direction == GameEnum.MOVE.LEFT);
            case GameEnum.MOVE.LEFT:
                return !(direction == GameEnum.MOVE.RIGHT);
            case GameEnum.MOVE.NONE:
                return true;
            default:
                break;
        }
        return false;
    }

    void KeyBoardDetectionMove()
    {
        if (direction != GameEnum.MOVE.NONE)
            return;

        for (uint u = 0; u < GameManager.settings.input.Length; ++u)
        {
            if (Input.GetKey(GameManager.settings.input[u].Up))
            {
                direction = GameEnum.MOVE.UP;
            }
            else if (Input.GetKey(GameManager.settings.input[u].Down))
            {
                direction = GameEnum.MOVE.DOWN;
            }

            if (Input.GetKey(GameManager.settings.input[u].Left))
            {
                direction = GameEnum.MOVE.LEFT;
            }

            else if (Input.GetKey(GameManager.settings.input[u].Right))
            {
                direction = GameEnum.MOVE.RIGHT;
            }
        }

        if (direction != GameEnum.MOVE.NONE)
        {
            typeOfInput = GameEnum.EVALUATEINPUT.KEYBOARD;
#if UNITY_EDITOR
            log += "KeyBoard pressed\nGo the direction : " + direction + "\n";
#endif
        }
    }

    void MouseDetectionMove()
    {
        RaycastHit hit;
        //Left click pressed
        if (Input.GetMouseButtonDown(0))
        {

#if UNITY_EDITOR
            log += "Mouse Click pressed\n";
#endif

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100.0f))
            {
                if (hit.transform.tag == "Map")
                {
                    oldMousePosition = hit.point;
#if UNITY_EDITOR
                    log += "Collision with the Map detected at the old position : " + oldMousePosition + "";
#endif
                }
            }
            LeftClickPressed = true;

            typeOfInput = GameEnum.EVALUATEINPUT.MOUSE;
        }
        if (Input.GetMouseButtonUp(0) && LeftClickPressed)
        {
            //need to now the position when the click is released
#if UNITY_EDITOR
            log += "Mouse Click released\n";
#endif

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100.0f))
            {
                if (hit.transform.tag == "Map")
                {
                    newMousePosition = hit.point;
#if UNITY_EDITOR
                    log += "Collision with the Map detected at the new position : " + newMousePosition + "\n";
#endif
                }
                else
                    newMousePosition = oldMousePosition;
            }

            LeftClickPressed = false;
        }

        //Evaluate the direction between the position of the click and the position of the release
        if (LeftClickPressed == false && typeOfInput == GameEnum.EVALUATEINPUT.MOUSE)
        {
            float xDiff = Mathf.Abs(newMousePosition.x - oldMousePosition.x);
            float zDiff = Mathf.Abs(newMousePosition.z - oldMousePosition.z);

            //Movement on X-Axis
            if (xDiff >= zDiff)
            {
                if (newMousePosition.x >= oldMousePosition.x)
                    direction = GameEnum.MOVE.RIGHT;
                else
                    direction = GameEnum.MOVE.LEFT;
            }
            else
            {
                if (newMousePosition.z >= oldMousePosition.z)
                    direction = GameEnum.MOVE.UP;
                else
                    direction = GameEnum.MOVE.DOWN;
            }

#if UNITY_EDITOR
            log += "Go the direction : " + direction + "\n";
#endif
        }
    }

#if UNITY_EDITOR
    void UpdateLog()
    {

        if (log.Length > 0)
        {
            InputLog.WriteLine(log);
            log = "";
        }
    }
#endif

}
