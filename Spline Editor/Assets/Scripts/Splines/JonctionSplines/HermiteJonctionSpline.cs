using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HermiteJonctionSpline : JonctionSpline
{
	public override Vector3 GetPointAt(float t)
	{
		int i = GetIndexJonction(ref t);

		return transform.TransformPoint(HermiteSpline.GetPointAt(new Vector3[4]
		{ P[i], P[i == 0 ? i + 1 : i - 1], P[i + 2], P[i + 3] }, t));
	}

	public override Vector3 GetVelocityAt(float t)
	{
		int i = GetIndexJonction(ref t);

		return transform.TransformPoint(HermiteSpline.GetVelocityAt(new Vector3[4]
		{ P[i], P[i == 0 ? i + 1 : i - 1], P[i + 2], P[i + 3] }, t));
	}
}
