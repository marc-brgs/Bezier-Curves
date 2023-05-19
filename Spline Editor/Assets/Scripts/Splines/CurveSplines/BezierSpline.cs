using UnityEngine;

public class BezierSpline : Spline
{
	public override Vector3 GetPointAt(float t)
	{
		return transform.TransformPoint(GetPointAt(P, t));
	}

	public override Vector3 GetVelocityAt(float t)
	{
		return transform.TransformPoint(GetVelocityAt(P, t)) - transform.position;
	}

	public static Vector3 GetPointAt(Vector3[] v, float t)
	{
		return Mathf.Pow(1f - t, 3f) * v[0] +
			3f * t * Mathf.Pow(1f - t, 2f) * v[1] +
			3f * t * t * (1f - t) * v[2] +
			t * t * t * v[3];
	}

	public static Vector3 GetVelocityAt(Vector3[] v, float t)
	{
		return (-3f * t * t + 6f * t - 3f) * v[0] +
			(9f * t * t - 12f * t + 3f) * v[1] +
			(-9f * t * t + 6f * t) * v[2] +
			(3f * t * t) * v[3];
	}

	public override void Reset()
	{
		base.Reset();
		P = new Vector3[4];
		P[0] = new Vector3(-10, 0, 0);
		P[1] = new Vector3(-10, 10, 0);
		P[2] = new Vector3(10, 10, 0);
		P[3] = new Vector3(10, 0, 0);
	}
}
