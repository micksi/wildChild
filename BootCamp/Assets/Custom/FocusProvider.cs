using UnityEngine;
using System.Collections;
using System;

public static class FocusProvider {

	private const float CmToInch = 0.393701f;

	public enum Source { Mouse, ScreenCentre, Gaze };
	public static Source source = Source.ScreenCentre;

	private static Camera _cam = null;
	private static Camera cam
	{
		get { if(_cam == null) _cam = Camera.main; return _cam; }
	}

	private static GazeWrap _gazeScript = null;
	private static GazeWrap gazeScript
	{
		get { if(_gazeScript == null) _gazeScript = cam.GetComponent(typeof(GazeWrap)) as GazeWrap; return _gazeScript; }
	}

	private static float _dpi = 0;
	private static float dpi
	{
		get { 	if(_dpi < 1)
				{
					_dpi = Screen.dpi; 
					if(_dpi < 1f) // Screen.dpi couldn't be determined
					{
						Debug.LogWarning("Couldn't determine display DPI, falling back to 110.267.");
						_dpi = 110.267f; // Thorbjørn's computer's DPI
					}	
				}

				return _dpi; 
			}
	}


	// Pixel distance between focus point and object point on screen
	public static float GetFocusDistance(Vector3 fromWorldPosition)
	{
		return GetFocusObjectDifference(fromWorldPosition).magnitude;
	}

	public static float GetNormalizedFocusDistance(Vector3 fromWorldPosition)
	{
		return GetNormalizedFocusObjectDifference(fromWorldPosition).magnitude;
	}

	public static float GetNormalizedFocusDistanceSquared(Vector3 fromWorldPosition)
	{
		return GetNormalizedFocusObjectDifference(fromWorldPosition).sqrMagnitude;
	}

	// Object - focus difference vectors
	private static Vector2 GetFocusObjectDifference(Vector3 worldPosition)
	{
		return GetFocusPosition() - GetObjectScreenPosition(worldPosition);
	}

	private static Vector2 GetNormalizedFocusObjectDifference(Vector3 worldPosition)
	{
		Vector2 pixelVector = GetFocusObjectDifference(worldPosition);
		return NormalizeScreenPosition(pixelVector);
	}

	// Returns world position's screen position, in pixels
	private static Vector2 GetObjectScreenPosition(Vector3 worldPosition)
	{
		Vector3 screenPos = cam.WorldToScreenPoint(worldPosition);
		return new Vector2(screenPos.x, screenPos.y);
	}

	// Transform a position from coordinates in pixels to normalized
	// screen coordinates [0;1].
	private static Vector2 NormalizeScreenPosition(Vector2 pixelPosition)
	{
		pixelPosition.x /= cam.pixelWidth;
		pixelPosition.y /= cam.pixelHeight;
		return pixelPosition;
	}

	// Returns focus position, in pixels.
	public static Vector2 GetFocusPosition()
	{	
		switch(source)
		{
			case Source.Mouse:
				return (Vector2)Input.mousePosition;
			case Source.ScreenCentre:
				return new Vector2(cam.pixelWidth / 2, cam.pixelHeight / 2);
			case Source.Gaze:
				return gazeScript.GetGazeScreenPosition();
			default:
				return new Vector2(-1, -1);
		}
	}

	public static Vector3 GetFocusDirection()
	{
		Vector2 focusPoint = GetFocusPosition();
		return cam.ScreenPointToRay(new Vector3(focusPoint.x, focusPoint.y, 0)).direction;
	}

	// Get focus position where x, y = [0;1]
	public static Vector2 GetNormalizedFocusPosition()
	{
		Vector2 pixelFocus = GetFocusPosition();
		return NormalizeScreenPosition(pixelFocus);
	}

	public static float GetFocusRadiusPixels(float userDistanceCentimetres, float focusAngleDegrees)
	{
		// Radius on screen in pixels is equal to
		// tan(angle of view) * distance to user * DPI of screen
		float radiusCentimetres = Mathf.Tan(focusAngleDegrees * Mathf.Deg2Rad) * userDistanceCentimetres;
		float radiusInches = radiusCentimetres * CmToInch; 	
		float radiusPixels = radiusInches * dpi;

		return radiusPixels;
	}
}
