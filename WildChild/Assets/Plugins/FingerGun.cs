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
	//wiimote stuff
	private WiimoteReceiver receiver;
	private WiimoteTransmitter transmitter;
	public Texture2D cursorOne;
	public Vector3 wiiPos;

	// Use this for initialization
	void Start () 	
	{
		receiver = WiimoteReceiver.Instance;
		receiver.connect(8876);

		transmitter = WiimoteTransmitter.Instance;
		transmitter.setConnectionInfo("localhost", 7500);
		transmitter.connect();
	}
	
	// Update is called once per frame
	void Update () {

		if(receiver.wiimotes.ContainsKey(1) )
		{	
			Wiimote mymote = receiver.wiimotes[1];
			wiiPos = new Vector3((mymote.IR_X * Screen.width)+cursorOne.width/2, (mymote.IR_Y * Screen.height)-cursorOne.height/2);
		}

		transform.LookAt(target.transform.position);

		if(Input.GetKeyDown("s")) //Check of shot is fired
		{
			shoot();
			OnTargetHit(wiiPos); //Destroy target if hit
		}

		if(canShoot == false)
		{
			counter += Time.deltaTime;
			if(counter > 0.200f)
			{	
				transmitter.vibrate(1,0);
				canShoot = true;
				print ("Reloaded");			
			}
			else
				transmitter.vibrate(1,1);
		}


	}

	void OnGUI()
	{
		if(receiver.wiimotes.ContainsKey(1) )
		{	
			Wiimote mymote = receiver.wiimotes[1];
			GUI.DrawTexture(new Rect(mymote.IR_X * Screen.width, (1-mymote.IR_Y)* Screen.height,32,32), cursorOne);
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
			OnTargetHit(wiiPos); //Destroy target if hit
			counter = 0;
			canShoot = false;
		}
	}
}
