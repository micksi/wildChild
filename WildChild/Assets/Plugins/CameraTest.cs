﻿using UnityEngine;
using System.Collections;

public class CameraTest : MonoBehaviour {

	public GameObject Box;
	Vector3 temp;


	// Use this for initialization
	void Start () {
		//Screen.showCursor = false;
	}

	void Update() {
		temp = Input.mousePosition;
		temp.z = 100.0f;
		Box.transform.position = camera.ScreenToWorldPoint(temp);
	} 
}
