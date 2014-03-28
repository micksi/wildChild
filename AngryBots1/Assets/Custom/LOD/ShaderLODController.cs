using UnityEngine;
using System.Collections;

public class ShaderLODController : MonoBehaviour {

	private Shader shader;

	public int tempThresholdPixelCount = 200;

	// Use this for initialization
	void Start () {
		shader = renderer.material.shader;
	}

	/*private float GetDistanceToFocus()
	{
		//Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
		//Vector2 focusPos = FocusProvider.GetFocusPosition();

		return FocusProvider.GetFocusDistance(transform.position);

		//return (focusPos - screenPos).magnitude;
	}
*/
	/*private Vector3 GetFocusPosition()
	{
		// Use mouse position for now
		return Input.mousePosition;
	}*/

	private int DetermineWantedLOD()
	{
		float distanceToFocus = FocusProvider.GetFocusDistance(transform.position);//GetDistanceToFocus();
		if(distanceToFocus < tempThresholdPixelCount)
		{
			return 200;
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
