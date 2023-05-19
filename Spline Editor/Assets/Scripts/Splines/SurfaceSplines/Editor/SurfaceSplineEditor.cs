using UnityEditor;

[CustomEditor(typeof(SurfaceSpline), true)]
public class SurfaceSplineEditor : SplineEditor
{
	protected override void OnSceneGUI()
	{
		base.OnSceneGUI();
		m_PrecisionCurve = 0.1f;
	}

	protected override void DrawCurve()
	{
		for (float t = 0; t < 1f + m_PrecisionCurve; t += m_PrecisionCurve)
		{
			for (float s = 0; s < 1f + m_PrecisionCurve; s += m_PrecisionCurve)
			{
				if (t < 1f)
					Handles.DrawLine(m_Spline.GetPointAt(t, s), m_Spline.GetPointAt(t + m_PrecisionCurve, s), m_CurveLineThickness);
				if (s < 1f)
					Handles.DrawLine(m_Spline.GetPointAt(t, s), m_Spline.GetPointAt(t, s + m_PrecisionCurve), m_CurveLineThickness);
			}
		}
	}

	protected override void DrawDirections()
	{
		for (float t = 0; t < 1f + m_PrecisionCurve; t += m_PrecisionCurve)
		{
			for (float s = 0; s < 1f + m_PrecisionCurve; s += m_PrecisionCurve)
			{
				if (t < 1f)
					Handles.DrawLine(m_Spline.GetPointAt(t, s), m_Spline.GetPointAt(t, s) + m_Spline.GetDirectionAt(t + m_PrecisionCurve, s) * m_LineScaleDirection);
				if (s < 1f)
					Handles.DrawLine(m_Spline.GetPointAt(t, s), m_Spline.GetPointAt(t, s) + m_Spline.GetDirectionAt(t, s + m_PrecisionCurve) * m_LineScaleDirection);
			}
		}
	}

	protected override void DrawLineBetweenPoints()
	{
		for (int i = 0; i < 13; i += 4)
		{
			Handles.DrawLine(m_Points[i], m_Points[i + 1]);
			Handles.DrawLine(m_Points[i + 1], m_Points[i + 2]);
			Handles.DrawLine(m_Points[i + 2], m_Points[i + 3]);

			if (i < 12)
			{
				for (int j = 0; j < 4; ++j)
					Handles.DrawLine(m_Points[i + j], m_Points[i + j + 4]);
			}
		}
	}
}
