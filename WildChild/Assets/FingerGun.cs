using UnityEngine;
using System.Collections;

public class FingerGun : MonoBehaviour {
	
	Ray ray;
	RaycastHit hit;
	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () {

		if(Input.GetMouseButtonDown(0)) //Check of shot is fired
		{
			OnTargetHit(Input.mousePosition); //Destroy target if hit
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
}
