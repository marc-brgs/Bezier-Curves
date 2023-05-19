using UnityEngine;

public class BezierJonctionSpline : JonctionSpline
{
	public override Vector3 GetPointAt(float t)
	{
		int i = GetIndexJonction(ref t);

		return transform.TransformPoint(BezierSpline.GetPointAt(new Vector3[4]
		{ P[i], P[i + 1], P[i + 2], P[i + 3] }, t));
	}

	public override Vector3 GetVelocityAt(float t)
	{
		int i = GetIndexJonction(ref t);

		return transform.TransformPoint(BezierSpline.GetVelocityAt(new Vector3[4]
		{ P[i], P[i + 1], P[i + 2], P[i + 3] }, t)) - transform.position;
	}
}
