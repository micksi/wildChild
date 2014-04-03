using UnityEngine;
using System.Collections.Generic;

public class MasterOfLOD : MonoBehaviour {

	//private float focusDotProductMin;
	[Range(0.0F, 1.571F)]
	public float hiLODAngleRadians = 0.25f;

	public Shader lowShader, highShader;

	private GameObject[] customGOs;

	// Use this for initialization
	void Start () {
		// Get ALL GameObjects
		Object[] objects = GameObject.FindObjectsOfType<GameObject>();

		print(objects.Length);

		Shader ourShader = Shader.Find("Custom/Bumped specular");
		
		if(!lowShader)
			lowShader = Shader.Find("VertexLit");//Shader.Find("Custom/VertexLit");
		if(!highShader)
			highShader = Shader.Find("Reflective/Bumped Specular");//Shader.Find("Custom/PixelLit");

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

		print(ourGOs.Count + " shaders affected by MasterOfLOD");
		customGOs = ourGOs.ToArray();
	}

	void Update()
	{
		// TODO debugging functionality
		bool keydown = Input.GetKey("l");

		float focusDotProductMin = Mathf.Cos(hiLODAngleRadians);

		// Get focus direction vector
		Vector3 focus = FocusProvider.GetFocusDirection();

		// Compare each shaded object's centre dir from cam with focus dir	
		Vector3 objectDir;
		Vector3 camPos = Camera.main.transform.position;

		//customGOs[0].renderer.material.shader = (customGOs[0].renderer.material.shader == high) ? low : high;
		//Debug.DrawLine(camPos, customGOs[0].transform.position, Color.white);

		foreach(GameObject go in customGOs)
		{
			/*objectDir = go.transform.position - camPos;
			objectDir.Normalize();

			float dot = Vector3.Dot(focus, objectDir);*/
			//if(dot  > focusDotProductMin)
			//IsInFocusAreaBoundsTest(go, new Vector2(0,0), 1f);
			// TODO debugging functionality
			Vector3 debuggingVec;
			if(keydown)
			//if(IsInFocusAreaBoundsTest(go, focus, hiLODAngleRadians, out debuggingVec))//IsInFocusAreaSimple(go, focus, focusDotProductMin))
			{
				go.renderer.material.shader = highShader;
				//Debug.DrawLine(camPos, go.transform.position, Color.red);
				//Debug.DrawLine(camPos, camPos + debuggingVec, Color.red);

			}
			else
			{
				go.renderer.material.shader = lowShader;
				//Debug.DrawLine(camPos, go.transform.position, Color.blue);
				//Debug.DrawLine(camPos, camPos + debuggingVec, Color.blue);
			}
		}
	}

	private static bool IsInFocusAreaSimple(GameObject g, Vector3 focusDirection, float cosFocusRadiusAngle)
	{
		Vector3 objectDir = g.transform.position - Camera.main.transform.position;
		objectDir.Normalize();
		float dot = Vector3.Dot(focusDirection, objectDir);

		return (dot > cosFocusRadiusAngle);
	}

	private bool IsInFocusAreaBoundsTest(GameObject g, Vector3 focusDirection, float focusAngle, out Vector3 dir)
	{
		Vector3 objectDir = g.renderer.bounds.center - Camera.main.transform.position;
		dir = new Vector3(0,0,0);
		float dist = objectDir.magnitude;
		objectDir.Normalize();

		float dot = Vector3.Dot(focusDirection, objectDir);

		if(dot > Mathf.Cos(focusAngle)) // Object centre is inside focus
		{
			return true;
		}
		else
		{
			Vector3 towardsObject = Vector3.RotateTowards(focusDirection, objectDir, focusAngle, 1);
			Ray ray = new Ray(Camera.main.transform.position, towardsObject);
			dir = towardsObject * dist;
			return g.renderer.bounds.IntersectRay(ray);
		}
	}
		// Use raycasting from specific pixel location


		// You may access collider or bounding box of objects

		// OTherwise, maybe do an orthographic projection, and then do the check using a cylinder collider to check.
		// Maybe render bounding boxes to screen space and check diff
		// Camera class has ToScreen for a vertex. Do this on the bounding box.

}
