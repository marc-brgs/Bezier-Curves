using UnityEngine;

public class ContinuitySpline : Spline
{
	[HideInInspector]
	public int IndexCurve;

	public override void Reset()
	{
		base.Reset();
		P = new Vector3[6];
		P[0] = new Vector3(0, -5, 0);
		P[1] = new Vector3(-10, -10, 0);
		P[2] = new Vector3(-20, 10, 0);
		P[3] = new Vector3(0, 15, 0);
		P[4] = new Vector3(15, 10, 0);
		P[5] = new Vector3(15, -15, 0);
	}
}
