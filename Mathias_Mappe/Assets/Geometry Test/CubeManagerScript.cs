using UnityEngine;
using System.Collections;

public class CubeManagerScript : MonoBehaviour {

	public GameObject cube1;
	public GameObject cube2;
	Vector3 startPos;

	// Use this for initialization
	void Start () {
		startPos = cube1.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.mousePosition.x > Screen.width-100)
		{
			cube1.transform.position = transform.position + transform.up*10;
		}
		else
		{
			cube1.transform.position = startPos;
		}
	}
}
