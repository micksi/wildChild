using UnityEngine;
using System.Collections;

public class RailPoint : MonoBehaviour {

	public bool isCheckPoint = false;
	public GameObject[] targets;
	private int targetsAlive;

	// Use this for initialization
	void Start () {
		if(isCheckPoint == false)
		{
			int i = 0;
			while(targets.Length > 0)
			{
				Destroy(targets[i].gameObject);
				i++;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(isCheckPoint == true)
		{
			bool allgone = true;
			foreach(GameObject g in targets)
			{
				if(g.GetComponent<Target>().IsAlive())
				{
					allgone = false;
					break;
				}
			}

			if(allgone)
			{
				isCheckPoint = false;
			}
		}
	}
}
