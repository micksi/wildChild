using UnityEngine;
using System.Collections;

public class ShaderLODController : MonoBehaviour {

	private Shader shader;

	// Use this for initialization
	void Start () {
		shader = renderer.material.shader;
	}
	
	// Update is called once per frame
	void Update () 
	{
		// TODO change shader depending on distance to focus
		// and function determining at what distance shader 
		// change should occur

		
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
	}
}
