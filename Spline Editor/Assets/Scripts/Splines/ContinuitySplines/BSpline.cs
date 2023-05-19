using UnityEngine;

public class BSpline : ContinuitySpline
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
		return 1f / 6f * (Mathf.Pow(1f - t, 3f) * v[v.ClampIndex(i - 3)] +
			(3f * t * t * t - 6f * t * t + 4f) * v[v.ClampIndex(i - 2)] +
			(-3f * t * t * t + 3f * t * t + 3f * t + 1f) * v[v.ClampIndex(i - 1)] +
			t * t * t * v[i]);
	}

	public static Vector3 GetVelocityAt(Vector3[] v, float t, int i)
	{
		return 1f / 2f * (-v[v.ClampIndex(i - 3)] * Mathf.Pow(1f - t, 2f)) +
			t * (3f * v[v.ClampIndex(i - 2)] * t - 4f * v[v.ClampIndex(i - 2)] + v[v.ClampIndex(i)] * t) +
			v[v.ClampIndex(i - 1)] * (-3f * t * t + 2f * t + 1f);
	}
}
