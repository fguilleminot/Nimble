using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Settings : MonoBehaviour {


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

}
