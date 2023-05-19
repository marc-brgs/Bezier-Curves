using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
	public static int ClampIndex(this Vector3[] v, int i)
	{
		if (i < 0)
		{
			i = v.Length + i;
			v.ClampIndex(i);
		}
		else if (i > v.Length)
		{
			i -= v.Length;
			v.ClampIndex(i);
		}

		return i;
	}

	public static int ClampIndex(this List<Vector3> v, int i)
	{
		if (i < 0)
		{
			i = v.Count + i;
			v.ClampIndex(i);
		}
		else if (i > v.Count)
		{
			i -= v.Count;
			v.ClampIndex(i);
		}

		return i;
	}
}
