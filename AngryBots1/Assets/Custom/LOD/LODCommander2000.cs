using UnityEngine;
using System.Collections;

public class LODCommander2000 : MonoBehaviour {

	public LODGroup lodgroup;
	public int currentLOD = 0;
	
	void Update () 
	{
		if(Input.GetKeyDown("e"))
		{
			print("Key e down!");
			currentLOD++;

			if(currentLOD > 2)
			currentLOD = 0;
			if(currentLOD < 0)
				currentLOD = 2;

			lodgroup.ForceLOD(currentLOD);
		}

		
	}
}