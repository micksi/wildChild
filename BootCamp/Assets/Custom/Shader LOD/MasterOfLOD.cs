using UnityEngine;
using System.Collections.Generic;

public class MasterOfLOD : MonoBehaviour {

	// User's distance from the screen, in centimetres
	public float userDistanceCentimetres = 60f;
	[Range(0.0F, 50.0F)]

	// The user's physical FOV focus radius, in degrees
	public float userFocusRadiusAngleDegrees = 10f;

	// The shaders that we're switching between.
	// High is used when object is in focus area,
	// low otherwise.
	public Shader lowShader, highShader;

	// The source of our focus data. Default to Screen centre.
	public FocusProvider.Source focusSource = FocusProvider.Source.ScreenCentre;
	
	// When true, hold L down to use highShader, do not to use lowShader.
	// Logs some info to debug console, too.
	public bool debug = false; 

	// Displays focus area in game view. A bit heavy on performance.
	public bool showFocusArea = false; 

	// The list of GameObjects on which the shader depends on whether or not they are 
	// in focus.
	private GameObject[] affectedGameObjects;

	// Storing reference to main camera, rather than accessing it 1k times each frame,
	// improved FPS by around 2x
	private static Camera cam;

	// The focus radius, in radians, that will be used when 
	// determining what objects use what shader
	public float ingameFocusRadiusRadians
	{
		get;
		private set;
	}

	// Use this for initialization
	void Start () {
		// Initiate shaders
		Shader placeholder = Shader.Find("Custom/Bumped specular");
		
		if(lowShader == null)
			lowShader = Shader.Find("VertexLit");
		if(highShader == null)
			highShader = Shader.Find("Reflective/Bumped Specular");

		// Get ALL GameObjects
		Object[] objects = GameObject.FindObjectsOfType<GameObject>();

		// Create list for GameObjects that have the placeholder shader
		List<GameObject> gameObjectsWithPlaceholder = new List<GameObject>(objects.Length/2);

		// Get gameobjects with our placeholder shader
		foreach(Object o in objects)
		{
			if(o is GameObject)
			{
				GameObject go = (GameObject)o;
				if(go.renderer)
				{
					if(go.renderer.material.shader == placeholder)
					{
						gameObjectsWithPlaceholder.Add(go);
					}
				}
			}
		}

		affectedGameObjects = gameObjectsWithPlaceholder.ToArray();
		
		cam = Camera.main;
		ingameFocusRadiusRadians = GetIngameFocusRadiusRadians();

		if(debug)
		{
			print(objects.Length + " GameObjects found.");
			print(affectedGameObjects.Length + " GameObjects with gaze-contingent shading found.");
			print("ingameFocusRadiusRadians: " + ingameFocusRadiusRadians);
		}
		if(showFocusArea)
		{
			gameObject.AddComponent("FocusIllustrator");			
		}
	}

	void Update()
	{
		// Update FocusProvider's source setting (mouse, gaze, centre of screen...)
		FocusProvider.source = focusSource;

		// Get focus direction vector
		Vector3 focus = FocusProvider.GetFocusDirection();

		// Get focus radius
		ingameFocusRadiusRadians = GetIngameFocusRadiusRadians();

		float cosFocusAngle = Mathf.Cos(ingameFocusRadiusRadians);

		// Decide, for each object, whether to use high quality shader or low quality shader
		foreach(GameObject go in affectedGameObjects)
		{
			// Ignore focus area and all that jazz - just draw everything with one of the shaders
			if(debug) 
			{
				if(Input.GetKey("l"))
				{
					go.renderer.material.shader = highShader;
					continue;
				}
				else
				{
					go.renderer.material.shader = lowShader;
					continue;
				}
			}

			if(IsInFocusAreaBoundsTest(go, focus, ingameFocusRadiusRadians, cosFocusAngle))
			{
				go.renderer.material.shader = highShader;
			}
			else
			{
				go.renderer.material.shader = lowShader;
			}
		}
	}

	// Tests whether the transform position is within the focus cone
	private bool IsInFocusAreaSimple(GameObject g, Vector3 focusDirection, float cosFocusRadiusAngle)
	{
		Vector3 objectDir = g.transform.position - cam.transform.position;
		objectDir.Normalize();
		float dot = Vector3.Dot(focusDirection, objectDir);

		return (dot > cosFocusRadiusAngle);
	}

	// Tests whether a GameObject's bounding volume is within the focus cone
	private bool IsInFocusAreaBoundsTest(GameObject g, Vector3 focusDirection, float focusAngle, float cosFocusAngle)
	{
		Vector3 objectDir = g.renderer.bounds.center - cam.transform.position; // Use bounds.center, as it should give more precise centre estimate
		objectDir.Normalize();

		float dot = Vector3.Dot(focusDirection, objectDir);

		if(dot > cosFocusAngle) // Object centre is inside focus
		{
			return true;
		}
		else
		{
			// Rotate the focus direction towards the object, but only up to focusAngle degrees
			// then see if the ray in that direction intersects the bounds of the object.
			// This is a conservative approach: We can get false positives, but not false negatives.
			Vector3 towardsObject = Vector3.RotateTowards(focusDirection, objectDir, focusAngle, 1);
			Ray ray = new Ray(cam.transform.position, towardsObject);
			return g.renderer.bounds.IntersectRay(ray);
		}
	}

	// Uses the user's focus angle and user distance to screen to establish what the 
	// in-game focus angle should be to cover the pixels subtended by the user's 
	// focus area.
	private float GetIngameFocusRadiusRadians()
	{
		float focusRadiusPixels = FocusProvider.GetFocusRadiusPixels(userDistanceCentimetres, userFocusRadiusAngleDegrees);

		// Generate a vector on the edge of the focus area, relative to the
		// centre of the screen (i.e. cam's forward vector)
		Ray focusEdgeRay = cam.ScreenPointToRay
			(
				new Vector3(Screen.width  / 2 + focusRadiusPixels, // offset from centre by focusRadiusPixels
							Screen.height / 2, 
							0)
			);
		Vector3 focusEdge = focusEdgeRay.direction;

		if(debug)
		{ 
			print("Screen res: " + Screen.currentResolution.width + ", " + Screen.currentResolution.width);
			print("focusRadiusPixels: " + focusRadiusPixels);
			print("In-game radius angle in degrees" + Vector3.Angle(cam.transform.forward, focusEdge));
		}

		// For some reason, Vector3.Angle returns degrees, not radians.
		// Return the angle between camera forward, and that of the focus edge vector
		return Vector3.Angle(cam.transform.forward, focusEdge) * Mathf.Deg2Rad;
	}
}
