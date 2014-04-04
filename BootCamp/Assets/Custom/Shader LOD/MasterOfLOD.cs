using UnityEngine;
using System.Collections.Generic;

public class MasterOfLOD : MonoBehaviour {

	[Range(0.0F, 1.571F)]
	public float hiLODAngleRadians = 0.25f;

	public Shader lowShader, highShader;
	public FocusProvider.Source focusSource = FocusProvider.Source.ScreenCentre;
	
	public bool debug = false;
	public bool showFocusArea = false;

	private GameObject[] customGOs;

	// TODO use Screen.dpi, user distance to screen, and wanted focus 
	// radius in degrees to establish in-game focus area

	// Storing reference to main camera, rather than accessing it 1k times each frame,
	// improved FPS by around 2x
	private static Camera mainCam;

	// Use this for initialization
	void Start () {
		// Get ALL GameObjects
		Object[] objects = GameObject.FindObjectsOfType<GameObject>();

		Shader ourShader = Shader.Find("Custom/Bumped specular");
		
		if(!lowShader)
			lowShader = Shader.Find("VertexLit");
		if(!highShader)
			highShader = Shader.Find("Reflective/Bumped Specular");

		List<GameObject> ourGOs = new List<GameObject>(objects.Length/2);

		foreach(Object o in objects)
		{
			if(o is GameObject)
			{
				GameObject go = (GameObject)o;
				if(go.renderer)
				{
					if(go.renderer.material.shader == ourShader)
					{
						ourGOs.Add(go);
					}
				}
			}
		}

		customGOs = ourGOs.ToArray();
		mainCam = Camera.main;

		if(debug)
		{
			print(objects.Length + " GameObjects found.");
			print(customGOs.Length + " GameObjects with gaze-contingent shading found.");
		}
		if(showFocusArea)
		{
			gameObject.AddComponent("FocusIllustrator");			
		}
	}

	void Update()
	{
		float focusDotProductMin = Mathf.Cos(hiLODAngleRadians);
		FocusProvider.source = focusSource;

		// Get focus direction vector
		Vector3 focus = FocusProvider.GetFocusDirection();

		// Compare each shaded object's centre dir from cam with focus dir	
		Vector3 objectDir;
		Vector3 camPos = Camera.main.transform.position;
		float cosFocusAngle = Mathf.Cos(hiLODAngleRadians);

		foreach(GameObject go in customGOs)
		{
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
			if(IsInFocusAreaBoundsTest(go, focus, hiLODAngleRadians, cosFocusAngle))
			{
				go.renderer.material.shader = highShader;
			}
			else
			{
				go.renderer.material.shader = lowShader;
			}
		}
	}

	private bool IsInFocusAreaSimple(GameObject g, Vector3 focusDirection, float cosFocusRadiusAngle)
	{
		Vector3 objectDir = g.transform.position - mainCam.transform.position;
		objectDir.Normalize();
		float dot = Vector3.Dot(focusDirection, objectDir);

		return (dot > cosFocusRadiusAngle);
	}

	private bool IsInFocusAreaBoundsTest(GameObject g, Vector3 focusDirection, float focusAngle, float cosFocusAngle)
	{
		Vector3 objectDir = g.renderer.bounds.center - mainCam.transform.position;
		float dist = objectDir.magnitude;
		objectDir.Normalize();

		float dot = Vector3.Dot(focusDirection, objectDir);

		if(dot > cosFocusAngle) // Object centre is inside focus
		{
			return true;
		}
		else
		{
			Vector3 towardsObject = Vector3.RotateTowards(focusDirection, objectDir, focusAngle, 1);
			Ray ray = new Ray(mainCam.transform.position, towardsObject);
			return g.renderer.bounds.IntersectRay(ray);
		}
	}
}
