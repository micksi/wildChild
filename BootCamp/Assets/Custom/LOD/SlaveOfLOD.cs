using UnityEngine;
using System.Collections;

public class SlaveOfLOD : MonoBehaviour {

	private Shader shader;

	private static float thresholdRadius = 0.2f;
	private static float thresholdRadiusSqr;

	void Start () 
	{
		thresholdRadiusSqr = thresholdRadius * thresholdRadius;
		shader = renderer.material.shader;		
	}

	private int DetermineWantedLOD()
	{
		float distanceSqr = FocusProvider.GetNormalizedFocusDistanceSquared(transform.position);

		if(distanceSqr < thresholdRadiusSqr)
		{
			return 600;

		}
		else
		{
			return 200;
		}
	}
	
	void Update () 
	{
		shader.maximumLOD = DetermineWantedLOD();
	}
}
