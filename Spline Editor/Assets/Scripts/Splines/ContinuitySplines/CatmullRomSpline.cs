using UnityEngine;

public class CatmullRomSpline : ContinuitySpline
{
	public override Vector3 GetPointAt(float t)
	{
		return transform.TransformPoint(GetPointAt(P, t, IndexCurve));
	}

	public override Vector3 GetVelocityAt(float t)
	{
		return transform.TransformPoint(GetVelocityAt(P, t, IndexCurve)) - transform.position;
	}

	public static Vector3 GetPointAt(Vector3[] v, float t, int i)
	{
		return 1f / 2f * ((-t * t * t + 2f * t * t - t) * v[v.ClampIndex(i - 3)] +
			(3f * t * t * t - 5f * t * t + 2f) * v[v.ClampIndex(i - 2)] +
			(-3f * t * t * t + 4f * t * t + t) * v[v.ClampIndex(i - 1)] +
			(t * t * t - t * t) * v[v.ClampIndex(i)]);
	}

	public static Vector3 GetVelocityAt(Vector3[] v, float t, int i)
	{
		return 1f / 2f * ((-3f * t * t + 4f * t - 1f) * v[v.ClampIndex(i - 3)] +
			(9f * t * t - 10f * t) * v[v.ClampIndex(i - 2)] +
			(-9f * t * t + 8f * t + 1f) * v[v.ClampIndex(i - 1)] +
			(3f * t * t - 2f * t) * v[v.ClampIndex(i)]);
	}
}
