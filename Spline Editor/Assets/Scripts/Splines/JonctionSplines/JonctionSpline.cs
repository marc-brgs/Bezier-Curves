using UnityEngine;

public class JonctionSpline : Spline
{
	protected int GetIndexJonction(ref float t)
	{
		int i = P.Length - 4;

		if (t != 1f)
		{
			t = ((P.Length - 1) / 3) * Mathf.Clamp01(t);
			int tmp = (int)t;
			t -= tmp;
			i = tmp * 3;
		}

		return i;
	}

	public void ForcePoint(int i)
	{
		int curveIndex = (i + 1) / 3;
		if (curveIndex == 0)
			return;

		int a, b, c;
		c = curveIndex * 3;

		if (i < c)
		{
			a = c - 1;
			b = c + 1;
			if (a < 0)
				a = P.Length - 2;
			if (b > P.Length)
				b = 1;
		}
		else
		{
			a = c + 1;
			b = c - 1;
			if (b < 0)
				b = P.Length - 2;
			if (a > P.Length)
				a = 1;
		}

		if (a < P.Length && b < P.Length && c < P.Length)
			P[b] = 2f * P[c] - P[a];
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
