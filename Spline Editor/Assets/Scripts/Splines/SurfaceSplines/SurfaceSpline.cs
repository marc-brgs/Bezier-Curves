using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceSpline : Spline
{
	public override void Reset()
	{
		base.Reset();
		P = new Vector3[16];
		P[0] = new Vector3(0, 0, 0);
		P[1] = new Vector3(0, 3, 3);
		P[2] = new Vector3(0, 3, 6);
		P[3] = new Vector3(0, 0, 10);
		P[4] = new Vector3(5, 0, 0);
		P[5] = new Vector3(5, 5, 3);
		P[6] = new Vector3(5, 5, 6);
		P[7] = new Vector3(5, 0, 10);
		P[8] = new Vector3(10, 0, 0);
		P[9] = new Vector3(10, 5, 3);
		P[10] = new Vector3(10, 5, 6);
		P[11] = new Vector3(10, 0, 10);
		P[12] = new Vector3(15, 0, 0);
		P[13] = new Vector3(15, 3, 3);
		P[14] = new Vector3(15, 3, 6);
		P[15] = new Vector3(15, 0, 10);
	}
}
