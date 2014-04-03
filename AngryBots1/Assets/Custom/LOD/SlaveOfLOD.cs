using UnityEngine;
using System.Collections;

public class SlaveOfLOD : MonoBehaviour {

	private Shader shader;

	private static float thresholdRadius = 0.2f;

	void Start () 
	{
		shader = renderer.material.shader;		
	}

	private int DetermineWantedLOD()
	{
		float distance = FocusProvider.GetNormalizedFocusDistance(transform.position);

		if(distance < thresholdRadius)
		{
			//renderer.enabled = true;
			return 1000;

		}
		else
		{
			//renderer.enabled = false;
			return 101;
		}
	}
	
	void Update () 
	{
		shader.maximumLOD = DetermineWantedLOD();
	}
}
