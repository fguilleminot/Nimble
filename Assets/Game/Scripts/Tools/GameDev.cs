using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameDev
{

	public static System.Random RandomGenerator = new System.Random();

    public class ChangeDirection
    {
        public Vector3          Position;
        public GameEnum.MOVE    NewDir;

		public ChangeDirection(Vector3 pos, GameEnum.MOVE move)
		{
			Position = pos;
			NewDir = move;
		}

		public ChangeDirection()
		{
			Position = Vector3.zero;
			NewDir = GameEnum.MOVE.NONE;
		}

		public bool IsSet()
		{
			return (Position != Vector3.zero && NewDir != GameEnum.MOVE.NONE);
		}
    }

	public class EnnemiesParty : IEnumerable
	{
		List<GameObject> Ennemies = new List<GameObject>();
		public List<ChangeDirection> ChangeDir = new List<ChangeDirection>();

		public void AddEnnemy(GameObject target)
		{
			if (Contains(target))
				return;
			else
				Ennemies.Add(target);

		}

		public int Count()
		{
			return Ennemies.Count;
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

        public GameObject GetLeaderOfPack()
        {
            GameObject res = null;

            foreach (GameObject g in Ennemies)
            {
                UnitManager UM = g.GetComponent<UnitManager>();
                if (UM.Leader)
                {
                    res = g;
                }
            }

            return res;
        }

	}

}
