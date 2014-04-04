using UnityEngine;
using System.Collections.Generic;

public class MasterOfLOD : MonoBehaviour {

	private float _ingameFocusRadiusRadians = 0.25f;
	public float ingameFocusRadiusRadians
	{
		get;
		private set;
	}

	public float userDistanceCM = 60f;
	[Range(0.0F, 50.0F)]
	public float userFocusRadiusAngleDegrees = 10f;

	public Shader lowShader, highShader;
	public FocusProvider.Source focusSource = FocusProvider.Source.ScreenCentre;
	
	public bool debug = false;
	public bool showFocusArea = false;

	private GameObject[] customGOs;

	// Storing reference to main camera, rather than accessing it 1k times each frame,
	// improved FPS by around 2x
	private static Camera mainCam;

	// Use this for initialization
	void Start () {
		// Get ALL GameObjects
		Object[] objects = GameObject.FindObjectsOfType<GameObject>();

		Shader ourShader = Shader.Find("Custom/Bumped specular");
		
		if(lowShader == null)
			lowShader = Shader.Find("VertexLit");
		if(highShader == null)
			highShader = Shader.Find("Reflective/Bumped Specular");

		List<GameObject> ourGOs = new List<GameObject>(objects.Length/2);

		// Get gameobjects with our placeholder shader
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

		ingameFocusRadiusRadians = GetIngameFocusRadiusRadians();

		if(debug)
		{
			print(objects.Length + " GameObjects found.");
			print(customGOs.Length + " GameObjects with gaze-contingent shading found.");
			print("ingameFocusRadiusRadians: " + ingameFocusRadiusRadians);
		}
		if(showFocusArea)
		{
			gameObject.AddComponent("FocusIllustrator");			
		}
	}

	void Update()
	{
		//ingameFocusRadiusRadians = GetIngameFocusRadiusRadians();

		float focusDotProductMin = Mathf.Cos(ingameFocusRadiusRadians);
		FocusProvider.source = focusSource;

		// Get focus direction vector
		Vector3 focus = FocusProvider.GetFocusDirection();

		// Compare each shaded object's centre dir from cam with focus dir	
		Vector3 objectDir;
		Vector3 camPos = Camera.main.transform.position;
		float cosFocusAngle = Mathf.Cos(ingameFocusRadiusRadians);

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

	private float GetIngameFocusRadiusRadians()
	{
		//float focusRadiusPixels = FocusProvider.GetFocusRadiusPixels(userDistanceCM, userFocusRadiusAngleDegrees);
		//Vector3 focusEdgePoint = new Vector3(1440 / 2 - focusRadiusPixels, 9)

		float focusRadiusPixels = FocusProvider.GetFocusRadiusPixels(userDistanceCM, userFocusRadiusAngleDegrees);
		Ray focusEdgeRay = mainCam.ScreenPointToRay(new Vector3(Screen.width / 2 + focusRadiusPixels, Screen.height / 2, 0));
		Vector3 focusEdge = focusEdgeRay.direction;

		if(debug)
		{ 
			print("Screen res: " + Screen.currentResolution.width + ", " + Screen.currentResolution.width);
			print("focusRadiusPixels: " + focusRadiusPixels);
			print("In-game radius angle in degrees" + Vector3.Angle(mainCam.transform.forward, focusEdge));
		}

		return Vector3.Angle(mainCam.transform.forward, focusEdge) * Mathf.Deg2Rad;
	}
}
