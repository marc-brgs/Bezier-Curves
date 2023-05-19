using UnityEngine;

public class FollowJonctionSpline : MonoBehaviour
{
	public JonctionSpline Spline;
	public float Speed;

	public float m_T;

	public void Update()
	{
		m_T += Time.deltaTime * Speed;
		if (m_T > 1f)
			m_T -= 1f;

		transform.position = Spline.GetPointAt(m_T);
		transform.LookAt(Spline.GetPointAt(m_T) + Spline.GetDirectionAt(m_T));
	}
}
