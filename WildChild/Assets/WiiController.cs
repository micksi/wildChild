using UnityEngine;
using System.Collections;

public class WiiController : MonoBehaviour {
	
	private WiimoteReceiver receiver;
	private WiimoteTransmitter transmitter;
	private Quaternion initPosition;
	public Texture2D cursorOne;
	// Use this for initialization
	void Start () {
		receiver = WiimoteReceiver.Instance;
		receiver.connect(8876);
		initPosition = transform.rotation;
		
		transmitter = WiimoteTransmitter.Instance;
		transmitter.setConnectionInfo("localhost", 8000);
		transmitter.connect();
	}
	
	// Update is called once per frame
	void Update () {
		// Balance board Example (Balance board as id 2):
		if(receiver.wiimotes.ContainsKey(2))
		{
			Wiimote mymote = receiver.wiimotes[2];
			if(mymote.BALANCE_SUM > 0.2f)
			{
				transform.rotation = initPosition;
				float zTilt = ((mymote.BALANCE_TOPRIGHT + mymote.BALANCE_BOTTOMRIGHT)-(mymote.BALANCE_TOPLEFT + mymote.BALANCE_BOTTOMLEFT)) *20;
				float xTilt = ((mymote.BALANCE_BOTTOMLEFT + mymote.BALANCE_BOTTOMRIGHT)-(mymote.BALANCE_TOPLEFT + mymote.BALANCE_TOPRIGHT)) *20;
				transform.Rotate(-xTilt,0,-zTilt);
			}
			else {transform.rotation = Quaternion.Slerp(transform.rotation,initPosition,Time.deltaTime * 2.0f);}
		}
		// Vibration example (Wiimote 1)
		if(receiver.wiimotes.ContainsKey(1)) {
			Wiimote mote = receiver.wiimotes[1];
			
			transmitter.vibrate(1,(int)mote.BUTTON_A);
			//			print("buttonA: " + (int)mote.BUTTON_A);
			//			print("Y: " +(double)mote.IR_Y);
			//			print("X: " +(double)mote.IR_X);
		}
	}
	
	void OnGUI()
	{
		if(receiver.wiimotes.ContainsKey(1) )
		{	
			Wiimote mymote = receiver.wiimotes[1];
			GUI.DrawTexture(new Rect(mymote.IR_X * Screen.width, (1-mymote.IR_Y)* Screen.height,32,32), cursorOne);
		}
		/*		
		if(receiver.wiimotes.ContainsKey(1) )
		{	
			Wiimote mymote = receiver.wiimotes[1];
			GUI.Label(new Rect(0,10,100,20), mymote.PRY_ACCEL.ToString() );
			GUI.Label(new Rect(mymote.IR_X * Screen.width, (1.0f - mymote.IR_Y) * Screen.height, 20,20), "C1" );
		}
		GUI.Label(new Rect(0,40,100,20), receiver.test2.ToString());
		GUI.Label(new Rect(0,60,100,20), receiver.test);
		if(receiver.wiimotes.ContainsKey(2) )
			GUI.Label(new Rect(0,100,100,20), receiver.wiimotes[2].BUTTON_A.ToString() );
*/
	}
}
