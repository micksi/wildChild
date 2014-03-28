using UnityEngine;
using System.Collections;

public class ShaderLODController : MonoBehaviour {

	private Shader shader;

	public int tempThresholdPixelCount = 300;
	public int tempThresholdPixelCount2 = 150;


	// Use this for initialization
	void Start () {
		shader = renderer.material.shader;
	}

	private int DetermineWantedLOD()
	{
		float distanceToFocus = FocusProvider.GetFocusDistance(transform.position);
		if(distanceToFocus < tempThresholdPixelCount2)
		{
			return 200;
		}
		else if(distanceToFocus < tempThresholdPixelCount)
		{
			return 100;
		}
		else
		{
			return 50;
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		// TODO change shader depending on distance to focus
		// and function determining at what distance shader 
		// change should occur

		shader.maximumLOD = DetermineWantedLOD();
		return;
/*
		if(Input.GetKeyDown("z"))
		{
			shader.maximumLOD = 200;
			print("max lod 200");
		}
		if(Input.GetKeyDown("x"))
		{
			shader.maximumLOD = 50;
			print("max lod 50");
		}
*/	}
}
