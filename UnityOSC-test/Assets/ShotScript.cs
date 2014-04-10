using UnityEngine;
using System.Collections;

public class ShotScript : MonoBehaviour {

	public Transform explosionPrefab;

	// Use this for initialization
	void Start () {

		print ("Shot initiatet");
	}
	
	// Update is called once per frame
	void Update () {

	}

	void OnCollisionEnter(Collision collision) {
		//Instantiate(explosionPrefab, transform.position, Quaternion.identity);
		Destroy(gameObject);
	}
}
