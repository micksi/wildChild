using UnityEngine;
using System.Collections;
using System;

public static class FocusProvider {

	public static bool useMouse = true;

	// Distance
	public static float GetFocusDistance(Vector3 fromWorldPosition)
	{
		return GetFocusObjectDifference(fromWorldPosition).magnitude;
	}

	public static float GetNormalizedFocusDistance(Vector3 fromWorldPosition)
	{
		return GetNormalizedFocusObjectDifference(fromWorldPosition).magnitude;
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
		Vector3 focus;
		if(useMouse)
			 focus = Input.mousePosition;
		else
			throw new NotImplementedException("Gaze focus point functionality not implemented yet!");
		
		return new Vector2(focus.x, focus.y);
	}

	public static Vector2 GetNormalizedFocusPosition()
	{
		Vector2 pixelFocus = GetFocusPosition();
		return NormalizeScreenPosition(pixelFocus);
	}
}
