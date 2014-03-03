using UnityEngine;
using System.Collections;

public class SphereManagerScript : MonoBehaviour {

	public GameObject sphere4;
	public GameObject sphere7;
	public GameObject sphere10;

	Vector3 startPos;
	Vector3 vanishPos;

	// Use this for initialization
	void Start () {
		startPos = transform.position;
		vanishPos = new Vector3(0,-10,0);
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.mousePosition.x > Screen.width-100)
		{
			sphere10.transform.position = vanishPos;
			sphere7.transform.position = vanishPos;
			sphere4.transform.position = startPos;
		}
		else if(Input.mousePosition.x > Screen.width-200 && Input.mousePosition.x < Screen.width-100)
		{
			sphere10.transform.position = vanishPos;
			sphere7.transform.position = startPos;
			sphere4.transform.position = vanishPos;
		}
		else
		{
			sphere10.transform.position = startPos;
			sphere7.transform.position = vanishPos;
			sphere4.transform.position = vanishPos;
		}

		if(Input.GetButtonDown("Horizontal"))
		{
			startPos += new Vector3(5,0,0);
		}
		if(Input.GetButtonDown("Vertical"))
		{
			startPos += new Vector3(-5,0,0);
		}
	}
}
