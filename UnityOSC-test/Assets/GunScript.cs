using UnityEngine;
using System.Collections;

public class GunScript : MonoBehaviour {

	public GameObject shotPrefab;
	GameObject shot = null;

	public GameObject Osc;

	public Vector3 gunDir;

	// Use this for initialization
	void Start () {
	

	}
	
	// Update is called once per frame
	void Update () {

		if(Input.GetButtonDown("Fire1"))
		{
			//transform.parent.position
			print("Shoot!");
			shot = (GameObject) Instantiate(shotPrefab, transform.position + transform.forward*3.0f, Quaternion.identity);
			shot.rigidbody.AddForce(transform.forward*500.0f);

			//Instantiate(spark, transform.position + transform.forward * 1.0f, transform.rotation);
		}

	}

	public void shoot()
	{
		//transform.parent.position
		print("Shoot!");
		shot = (GameObject) Instantiate(shotPrefab, transform.position + transform.forward*3.0f, Quaternion.identity);
		shot.rigidbody.AddForce(transform.forward*500.0f);
	}

}
