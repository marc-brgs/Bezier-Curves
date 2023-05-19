using UnityEditor;

[CustomEditor(typeof(ContinuitySpline), true)]
public class ContinuitySplineEditor : SplineEditor
{
	protected bool m_DrawLoopCurve = true;

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		EditorGUI.BeginChangeCheck();
		m_DrawLoopCurve = EditorGUILayout.Toggle("Draw Loop Curve", m_DrawLoopCurve);
		if (EditorGUI.EndChangeCheck())
			EditorUtility.SetDirty(m_Spline);
	}

	protected override void DrawCurve()
	{
		for (int i = m_DrawLoopCurve ? 0 : 3; i < m_Spline.P.Length; ++i)
		{
			(m_Spline as ContinuitySpline).IndexCurve = i;
			base.DrawCurve();
		}
	}

	protected override void DrawDirections()
	{
		for (int i = 0; i < m_Spline.P.Length; ++i)
		{
			(m_Spline as ContinuitySpline).IndexCurve = i;
			base.DrawDirections();
		}
	}

	protected override void DrawLineBetweenPoints()
	{
		for (int i = m_DrawLoopCurve ? 0 : 1; i < m_Points.Count; ++i)
			Handles.DrawLine(m_Points[m_Points.ClampIndex(i - 1)], m_Points[i]);
	}
}
