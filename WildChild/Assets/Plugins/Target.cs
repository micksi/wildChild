using UnityEngine;
using System.Collections;

public class Target : MonoBehaviour {

	private bool isAlive = true;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void IsShot()
	{
		isAlive = false;
	}

	public bool IsAlive()
	{
		return isAlive;
	}
}
