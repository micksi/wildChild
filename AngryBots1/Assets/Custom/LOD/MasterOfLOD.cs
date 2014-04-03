using UnityEngine;
using System.Collections;

public class MasterOfLOD : MonoBehaviour {

	private float thresholdRadius = 0.3f;

	// Use this for initialization
	void Start () {
		// Get ALL GameObjects
		Object[] objects = GameObject.FindObjectsOfType<GameObject>();

		print(objects.Length);

		foreach(Object o in objects)
		{
			if(o is GameObject)
			{
				GameObject go = (GameObject)o;
				if(go.renderer)
				{
					// Put LOD script on 'em
					go.AddComponent("SlaveOfLOD");
				}
			}
		}
	}
}
