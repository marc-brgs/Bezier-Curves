using UnityEngine;

public class BSplineSurfaceSpline : SurfaceSpline
{
	public override Vector3 GetPointAt(float s, float t)
	{
		Vector3[] points = new Vector3[4];

		for (int i = 0; i < 4; ++i)
		{
			Vector3[] curvePoints = new Vector3[4];
			for (int j = 0; j < 4; ++j)
				curvePoints[j] = P[i * 4 + j];
			points[i] = BSpline.GetPointAt(curvePoints, s, 3);
		}

		return transform.TransformPoint(BSpline.GetPointAt(points, t, 3));
	}

	public override Vector3 GetVelocityAt(float t, float s)
	{
		Vector3[] points = new Vector3[4];

		for (int i = 0; i < 4; ++i)
		{
			Vector3[] curvePoints = new Vector3[4];
			for (int j = 0; j < 4; ++j)
				curvePoints[j] = P[i * 4 + j];
			points[i] = BSpline.GetVelocityAt(curvePoints, s, 3);
		}

		return transform.TransformPoint(BezierSpline.GetVelocityAt(points, t)) - transform.position;
	}
}
