using UnityEngine;

public class BezierSurfaceSpline : SurfaceSpline
{
	public override Vector3 GetPointAt(float t, float s)
	{
		Vector3[] points = new Vector3[4];

		for (int i = 0; i < 4; ++i)
		{
			Vector3[] curvePoints = new Vector3[4];
			for (int j = 0; j < 4; ++j)
				curvePoints[j] = P[i * 4 + j];
			points[i] = BezierSpline.GetPointAt(curvePoints, s);
		}

		return transform.TransformPoint(BezierSpline.GetPointAt(points, t));
	}

	public override Vector3 GetVelocityAt(float t, float s)
	{
		Vector3[] points = new Vector3[4];

		for (int i = 0; i < 4; ++i)
		{
			Vector3[] curvePoints = new Vector3[4];
			for (int j = 0; j < 4; ++j)
				curvePoints[j] = P[i * 4 + j];
			points[i] = BezierSpline.GetVelocityAt(curvePoints, s);
		}

		return transform.TransformPoint(BezierSpline.GetVelocityAt(points, t)) - transform.position;
	}
}
