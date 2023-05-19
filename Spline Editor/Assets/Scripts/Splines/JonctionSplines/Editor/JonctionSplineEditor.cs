using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(JonctionSpline), true)]
public class JonctionSplineEditor : SplineEditor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		if (GUILayout.Button("Force all points"))
		{
			for (int i = 0; i < m_Spline.P.Length; ++i)
				(m_Spline as JonctionSpline).ForcePoint(i);
		}
	}

	protected override void CallbackUpdatePoint(ref Vector3 point)
	{
		base.CallbackUpdatePoint(ref point);
		(m_Spline as JonctionSpline).ForcePoint(m_SelectedPoint);
	}

	protected override void DrawLineBetweenPoints()
	{
		for (int i = 0; i < m_Points.Count; i += 3)
		{
			if (i - 1 >= 0)
				Handles.DrawLine(m_Points[i - 1], m_Points[i]);

			if (i + 1 < m_Points.Count)
				Handles.DrawLine(m_Points[i], m_Points[i + 1]);
		}
	}
}
