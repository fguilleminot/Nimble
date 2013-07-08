using UnityEngine;
using System.Collections;

public class GameEnum {

    public enum INPUTSTATE
    {
        NONE,
        EVALUATE,
        CHECK,
        MOVE
    };

    public enum SPAWNSTATE
    {
        NONE,
        SPAWN
    }

    public enum IASTATE
    {
        NONE,
        EVALUATE,
        CHECK,
        MOVE
    }

    public enum EVALUATEINPUT
    {
        NONE,
        MOUSE,
        KEYBOARD
    };

    public enum MOVE
    {
        NONE,
        UP,
        DOWN,
        LEFT,
        RIGHT
    };

    public enum TYPEOFUNIT
    {
        NONE,
        HEROES,
        ENNEMIES
    };

    public enum TYPEOFATTACK
    {
        NONE,
        INSTANTKILL,
        VALUE,
        ALLTHELIFE,
    };

    public enum TYPEOFRANGE
    {
        NONE,
        VALUE,
        ALLGROUP
    };

    public enum TYPEOFFOV
    {
        NONE,
        ANGLE,
        INAROW,
        ALLGROUP
    };

    public enum TYPEOFATTACKSPEED
    {
        NONE,
        INSTANT,
        VALUE
    };

    public enum TYPEOFMOVEMENT
    {
        NONE,
        VALUE,
        STATIC
    };

    public enum CLASS
    {
		NONE = -1,
        KNIGHT,
        RANGER,
        LANCER,
        MAGE,
        GUNSLINGER,
        BOMBERMAN,
        BAT,
        SPIDER,
        RAFTER,
        DARKKNIGHT,
        DARKMAGE,
        HEALER
    };
}
