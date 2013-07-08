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
    public int EnnemiesNumberToWin = 10;
    public float TimeBetweenSpawn;

    public int EnnemiesTeamNumberMax = 3;

    public int multiplierRange = 5;

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

        public bool CheckStat()
        {
            if (this.Attack == GameEnum.TYPEOFATTACK.VALUE && this.AttackValue == 0)
                return false;
            if (this.AttackSpeed == GameEnum.TYPEOFATTACKSPEED.VALUE && this.AttackSpeedValue == 0)
                return false;
            if (this.Fov == GameEnum.TYPEOFFOV.ANGLE && this.FovAngleValue == 0)
                return false;
            if (this.Movement == GameEnum.TYPEOFMOVEMENT.VALUE && this.MovementValue == 0)
                return false;
            if (this.Range == GameEnum.TYPEOFRANGE.VALUE && this.RangeValue == 0)
                return false;
            if (Others.nearlyEqual(this.Cooldown, 0f, 0.01f))
                return false;
            if (this.Health == 0)
                return false;
            return true;
        }
    }

    public Unit[] Champion;
}