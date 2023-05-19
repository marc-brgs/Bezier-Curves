using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(Spline), true)]
public class SplineEditor : Editor
{
	protected Spline m_Spline;
	protected Transform m_SplineTransform;
	protected Quaternion m_SplineRotation;
	protected List<Vector3> m_Points;

	protected int m_SelectedPoint = -1;

	protected const float m_PointSize = 0.1f;
	protected const float m_CurveLineThickness = 3f;
	protected const float m_CurveDirectionLineThickness = 1.5f;

	protected bool m_DrawDirections;
	protected float m_PrecisionCurve = 0.01f;
	protected float m_PrecisionDirection = 0.2f;

	protected float m_LineScaleDirection = 10f;

	public override void OnInspectorGUI()
	{
		m_Spline = target as Spline;

		DrawDefaultInspector();
		EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

		if (m_SelectedPoint > -1 && m_SelectedPoint < m_Spline.P.Length)
		{
			EditorGUI.BeginChangeCheck();
			Vector3 point = EditorGUILayout.Vector3Field("P" + m_SelectedPoint, m_Spline.P[m_SelectedPoint]);
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(m_Spline, "Move Point");
				EditorUtility.SetDirty(m_Spline);
				m_Spline.P[m_SelectedPoint] = point;
			}
		}

		EditorGUI.BeginChangeCheck();
		m_DrawDirections = EditorGUILayout.Toggle("Draw Directions", m_DrawDirections);
		if (EditorGUI.EndChangeCheck())
			EditorUtility.SetDirty(m_Spline);
	}

	protected virtual void OnSceneGUI()
	{
		m_Spline = target as Spline;
		m_SplineTransform = m_Spline.transform;
		m_SplineRotation = (Tools.pivotRotation == PivotRotation.Local) ? m_SplineTransform.rotation : Quaternion.identity;

		Handles.color = Color.yellow;
		m_Points = new List<Vector3>();
		for (int i = 0; i < m_Spline.P.Length; ++i)
			m_Points.Add(DrawPoint(i));

		Handles.color = Color.white;
		DrawCurve();

		if (m_DrawDirections)
		{
			Handles.color = Color.green;
			DrawDirections();
		}

		Handles.color = Color.grey;
		DrawLineBetweenPoints();
	}

	protected virtual void DrawCurve()
	{
		for (float t = 0f; t < 1f - m_PrecisionCurve; t += m_PrecisionCurve)
			Handles.DrawLine(m_Spline.GetPointAt(t), m_Spline.GetPointAt(t + m_PrecisionCurve), m_CurveLineThickness);
	}

	protected virtual void DrawDirections()
	{
		for (float t = 0f; t < 1f; t += m_PrecisionDirection)
			Handles.DrawLine(m_Spline.GetPointAt(t), m_Spline.GetPointAt(t) + m_Spline.GetDirectionAt(t) * m_LineScaleDirection);
	}

	protected virtual void DrawLineBetweenPoints()
	{
		for (int i = 1; i < m_Points.Count; ++i)
			Handles.DrawLine(m_Points[m_Points.ClampIndex(i - 1)], m_Points[i]);
	}

	protected virtual Vector3 DrawPoint(int i)
	{
		Vector3 point = m_SplineTransform.TransformPoint(m_Spline.P[i]);
		float size = HandleUtility.GetHandleSize(point) * m_PointSize;

		if (Handles.Button(point, m_SplineRotation, size, size * 1.5f, Handles.DotHandleCap))
		{
			m_SelectedPoint = i;
			Repaint();
		}
		if (m_SelectedPoint == i)
		{
			EditorGUI.BeginChangeCheck();
			point = Handles.DoPositionHandle(point, m_SplineRotation);
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(m_Spline, "DoPositionHandle in DrawPoint");
				EditorUtility.SetDirty(m_Spline);
				CallbackUpdatePoint(ref point);
			}
		}

		Handles.Label(point * 1.01f, "P" + i);

		return point;
	}

	protected virtual void CallbackUpdatePoint(ref Vector3 point)
	{
		m_Spline.P[m_SelectedPoint] = m_SplineTransform.InverseTransformPoint(point);
	}
}
