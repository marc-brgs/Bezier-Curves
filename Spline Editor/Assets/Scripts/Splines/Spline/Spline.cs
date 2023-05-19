using UnityEngine;

public abstract class Spline : MonoBehaviour
{
	public Vector3[] P;

	public virtual Vector3 GetPointAt(float t)
	{
		return Vector3.zero;
	}

	public virtual Vector3 GetPointAt(float t, float s)
	{
		return Vector3.zero;
	}

	public virtual Vector3 GetVelocityAt(float t)
	{
		return Vector3.zero;
	}

	public virtual Vector3 GetVelocityAt(float t, float s)
	{
		return Vector3.zero;
	}

	public virtual Vector3 GetDirectionAt(float t)
	{
		return GetVelocityAt(t).normalized;
	}

	public virtual Vector3 GetDirectionAt(float t, float s)
	{
		return GetVelocityAt(t, s).normalized;
	}

	public virtual void Reset()
	{
	}
}
