using UnityEngine;
using System.Collections;

public class ScreenEffectManager : MonoBehaviour {

	public ScreenFade transition;
	public MonoBehaviour effect;
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown("k"))
		{
			transition.Blink(5);
			effect.enabled = !effect.enabled;
		}
	}
}
