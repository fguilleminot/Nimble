using UnityEngine;
using System.Collections;

public class Settings : MonoBehaviour{

    [System.Serializable]
    public class Inputs
    {
        public KeyCode Up;
        public KeyCode Down;
        public KeyCode Left;
        public KeyCode Right;
        public KeyCode Menu;

    }

    public Inputs[] input;
    public int rotationAngle;
    public int HeroSpeed = 5;

    [System.Serializable]
    public class Unit
    {
        public GameEnum.TYPEOFUNIT Type;
        public GameEnum.CLASS Class;
        public GameEnum.TYPEOFATTACK Attack;
        public int AttackValue = 0;
        public GameEnum.TYPEOFATTACKSPEED AttackSpeed;
        public int AttackSpeedValue = 0;
        public GameEnum.TYPEOFFOV Fov;
        public int FovAngleValue = 0;
        public GameEnum.TYPEOFMOVEMENT Movement;
        public int MovementValue = 5;
        public GameEnum.TYPEOFRANGE Range;
        public int RangeValue = 0;
        public float Cooldown = 0;
        public int Health = 0;
    }

    public Unit[] Champion;
}