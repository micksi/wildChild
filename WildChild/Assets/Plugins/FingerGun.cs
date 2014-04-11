using UnityEngine;
using System.Collections;

public class FingerGun : MonoBehaviour {

	public GameObject target;
	public Vector3 gunDir;

	public Transform spark;

	Ray ray;
	RaycastHit hit;

	bool canShoot = true;
	float counter = 0;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.LookAt(target.transform.position);

		if(Input.GetMouseButtonDown(0)) //Check of shot is fired
		{
			shoot();
			OnTargetHit(Input.mousePosition); //Destroy target if hit
		}

		if(canShoot == false)
		{
			counter += Time.deltaTime;
			if(counter > 0.200f)
			{
				canShoot = true;
				print ("Reloaded");
			}
		}
	}

	void OnTargetHit(Vector3 hitPosition)
	{
		ray = Camera.main.ScreenPointToRay(hitPosition);
		if(Physics.Raycast(ray, out hit))
		{
			if(hit.collider.tag == "Target")
			{
				hit.collider.gameObject.GetComponent<Target>().IsShot();
				Instantiate(Resources.Load("Detonator/Prefab Examples/Detonator-Simple", typeof(GameObject)), 
				            hit.collider.gameObject.transform.position, Quaternion.identity);
				hit.collider.gameObject.SetActive(false);
			}
		}
	}

	public void shoot()
	{
		if(canShoot == true)
		{
			Instantiate(spark, transform.position + transform.forward * 0.5f, transform.rotation);
			OnTargetHit(Input.mousePosition); //Destroy target if hit
			counter = 0;
			canShoot = false;
		}
	}
}
