using UnityEngine;
using System.Collections;

public class MaterialShapeShifting2000 : MonoBehaviour {

	public Material high, low;
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetKey("l"))
		{
			renderer.material = low;
		}
		else if(Input.GetKey("k"))
		{
			renderer.material = high;
		}
	}
}
