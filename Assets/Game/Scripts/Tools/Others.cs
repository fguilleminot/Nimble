using UnityEngine;
using System.Collections;

public class Others : MonoBehaviour
{

	public static bool nearlyEqual(float a, float b, float epsilon)
	{
		float absA = Mathf.Abs(a);
		float absB = Mathf.Abs(b);
		float diff = Mathf.Abs(a - b);

		if (a == b)
		{ // shortcut, handles infinities
			return true;
		}
		else if (a == 0 || b == 0 || diff < float.MinValue)
		{
			// a or b is zero or both are extremely close to it
			// relative error is less meaningful here
			return diff < (epsilon * float.MinValue);
		}
		else
		{ // use relative error
			return diff / (absA + absB) < epsilon;
		}
	}

	public static bool nearlyEqual(Vector3 a, Vector3 b, float epsilon, bool OnlyXZ = false)
	{
		if (OnlyXZ)
			return (nearlyEqual(a.x, b.x, epsilon) && nearlyEqual(a.z, b.z, epsilon));

		return (nearlyEqual(a.x, b.x, epsilon) && nearlyEqual(a.y, b.y, epsilon) && nearlyEqual(a.z, b.z, epsilon));
	}

	public static bool CompareVectors(Vector3 a, Vector3 b, float error)
	{
		//Check magnitude
		if (!Others.nearlyEqual(a.sqrMagnitude, b.sqrMagnitude, 0.005f))
			return false;

		float cosAngleError = Mathf.Cos(error * Mathf.Deg2Rad);

		float angle = Vector3.Dot(a.normalized, b.normalized);

		if (cosAngleError <= angle)
		{
			//If angle is greater, that means that the angle between the two vectors is less than the error allowed.
			return true;
		}

		return false;
	}
}
