using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameDev{

    public static System.Random RandomGenerator = new System.Random();

    public class EnnemiesParty : IEnumerable
    {
        List<GameObject> Ennemies = new List<GameObject>();

        public void AddEnnemy(GameObject target)
        {
            if (Contains(target))
                return;
            else
                Ennemies.Add(target);
        }

        public bool Contains(GameObject target)
        {
            return Ennemies.Contains(target);
        }

        public void RemoveEnnemy(GameObject target)
        {
            if (!Contains(target))
                return;
            else
                Ennemies.Remove(target);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (GameObject Ennemy in Ennemies)
            {
                yield return Ennemy;
            }
        }
    }

}
