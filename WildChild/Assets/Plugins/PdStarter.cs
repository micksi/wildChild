using UnityEngine;
using System.Collections;

public class PdStarter : MonoBehaviour {

	public string projectPath;

	// Use this for initialization
	void Start () {
		projectPath =  Application.dataPath;
		Application.OpenURL(projectPath + "/PD/send-osc-test.pd");
		//projectPath.Replace("Assets", "PD/send-osc-test.pd");
		//print (projectPath);
		//Application.OpenURL(projectPath);

	}
	
	// Update is called once per frame
	void Update () {
		//print (projectPath);
	}
}
