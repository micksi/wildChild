using UnityEngine;
using System.Collections;
using System;

public static class FocusProvider {

	public enum Source { Mouse, ScreenCentre, Gaze };
	public static Source source = Source.ScreenCentre;

	private static Camera _cam = null;
	private static Camera cam
	{
		get { if(!_cam) _cam = Camera.main; return _cam; }
	}

	// Distance
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


	// Object screen position
	private static Vector2 GetObjectScreenPosition(Vector3 worldPosition)
	{
		Camera cam = Camera.main;
		Vector3 screenPos = cam.WorldToScreenPoint(worldPosition);
		return new Vector2(screenPos.x, screenPos.y);
	}

	// Transform a position from coordinates in pixels to normalized
	// screen coordinates.
	private static Vector2 NormalizeScreenPosition(Vector2 pixelPosition)
	{
		Camera cam = Camera.main;
		pixelPosition.x /= cam.pixelWidth;
		pixelPosition.y /= cam.pixelHeight;
		return pixelPosition;
	}


	// Focus position
	public static Vector2 GetFocusPosition()
	{
		switch(source)
		{
			case Source.Mouse:
				return (Vector2)Input.mousePosition;
			case Source.ScreenCentre:
				return new Vector2(cam.pixelWidth / 2, cam.pixelHeight / 2);
			case Source.Gaze:
				throw new NotImplementedException("Gaze focus point functionality not implemented yet!");
			default:
				return new Vector2(-1, -1);
		}
	}

	public static Vector3 GetFocusDirection()
	{
		Vector2 focusPoint = GetFocusPosition();
		return Camera.main.ScreenPointToRay(new Vector3(focusPoint.x, focusPoint.y, 0)).direction;
	}

	public static Vector2 GetNormalizedFocusPosition()
	{
		Vector2 pixelFocus = GetFocusPosition();
		return NormalizeScreenPosition(pixelFocus);
	}



	public static float GetFocusRadiusPixels(float userDistanceCentimetres, float focusAngleDegrees)
	{
		// Radius on screen in pixels = tan(angle of view) * distance to user * DPI of screen
		float radiusCM = Mathf.Tan(focusAngleDegrees * Mathf.Deg2Rad) * userDistanceCentimetres;
		float radiusInches = radiusCM * 0.393701f;
		float dpi = Screen.dpi;
		if(dpi < 1f) // Screen.dpi probably couldn't be determined
		{
			dpi = 110.267f; // Thorbjørns computer's DPI
		}	
		float radiusPixels = radiusInches * dpi;

		return radiusPixels;
	}
}
