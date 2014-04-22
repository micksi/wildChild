using UnityEngine;
using System.Collections;

public class Target : MonoBehaviour {

	private bool isAlive = true;
	public bool moveVertical = false;
	public bool moveHorizontal = false;

	bool moveDir = true;
	public float moveDistance = 0.5f;

	float maxPosHor;
	float minPosHor;
	float maxPosVer;
	float minPosVer;


	// Use this for initialization
	void Start () {
		maxPosVer = transform.position.y+moveDistance;
		minPosVer = transform.position.y-moveDistance;
		
		maxPosHor = transform.position.x+moveDistance;
		minPosHor = transform.position.x-moveDistance;

		print (maxPosHor + " " + minPosHor);
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if(moveVertical)
		{
			moveHorizontal = false;
			if(transform.position.y < maxPosVer && moveDir)
			{
				rigidbody.velocity = new Vector3(0,1,0);
			}
			else if(transform.position.y >= maxPosVer)
			{
				moveDir = false;
			}
			
			if(moveDir == false && transform.position.y > minPosVer)
			{
				rigidbody.velocity = new Vector3(0,-1,0);
			}
			else if(transform.position.y <= minPosVer)
			{
				moveDir = true;
			}
		}

		if(moveHorizontal)
		{
			if(transform.position.x < maxPosHor && moveDir)
			{
				rigidbody.velocity = new Vector3(1,0,0);
			}
			else if(transform.position.x >= maxPosHor)
			{
				moveDir = false;
			}
			
			if(moveDir == false && transform.position.x > minPosHor)
			{
				rigidbody.velocity = new Vector3(-1,0,0);
			}
			else if(transform.position.x <= minPosHor)
			{
				moveDir = true;
			}
		}
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
