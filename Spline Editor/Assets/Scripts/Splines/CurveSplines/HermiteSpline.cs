using UnityEngine;

public class HermiteSpline : Spline
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
		return (2f * t * t * t - 3f * t * t + 1f) * v[0] +
			(-2f * t * t * t + 3f * t * t) * v[3] +
			(t * t * t - 2f * t * t + t) * (v[0] - v[1]) +
			(t * t * t - t * t) * (v[3] - v[2]);
	}

	public static Vector3 GetVelocityAt(Vector3[] v, float t)
	{
		return (6f * t * t - 6f * t) * v[0] +
			(-6f * t * t + 6f * t) * v[3] +
			(3f * t * t - 4f * t + 1f) * (v[0] - v[1]) +
			(3f * t * t - 2f * t) * (v[3] - v[2]);
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
