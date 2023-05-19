using UnityEditor;

[CustomEditor(typeof(HermiteSpline))]
public class HermiteSplineEditor : SplineEditor
{
	protected override void DrawLineBetweenPoints()
	{
		Handles.DrawLine(m_Points[0], m_Points[1]);
		Handles.DrawLine(m_Points[3], m_Points[2]);
	}
}
