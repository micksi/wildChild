using TETCSharpClient;
using TETCSharpClient.Data;
using System.Net.Sockets;
using System.Threading;
using System;
using System.IO;
using Newtonsoft.Json.Linq;

public class GazePoint : IGazeListener 
{
	private TcpClient socket;
	private Thread incomingThread;
	private System.Timers.Timer timerHeartbeat;
	bool isRunning = false;

	public GazePoint()
	{
		// Connect client
		GazeManager.Instance.Activate(GazeManager.ApiVersion.VERSION_1_0, GazeManager.ClientMode.Push);
		
		// Register this class for events
		GazeManager.Instance.AddGazeListener(this);
	}
	
	public void OnGazeUpdate(GazeData gazeData)
	{
		double gX = gazeData.SmoothedCoordinates.X;
		double gY = gazeData.SmoothedCoordinates.Y;
		
		// Move point, do hit-testing, log coordinates etc.
	}

	public bool Connect(string host, int port)
	{
		try
		{
			socket = new TcpClient("localhost", 6555);
		}
		catch (Exception ex)
		{
			Console.Out.WriteLine("Error connecting: " + ex.Message);
			return false;
		}
		
		// Send the obligatory connect request message
		string REQ_CONNECT = "{\"values\":{\"push\":true,\"version\":1},\"category\":\"tracker\",\"request\":\"set\"}"; 
		Send(REQ_CONNECT);
		
		// Lauch a seperate thread to parse incoming data
		incomingThread = new Thread(ListenerLoop);
		incomingThread.Start();
		
		// Start a timer that sends a heartbeat every 250ms.
		// The minimum interval required by the server can be read out 
		// in the response to the initial connect request.   
		
		string REQ_HEATBEAT = "{\"category\":\"heartbeat\",\"request\":null}";
		timerHeartbeat = new System.Timers.Timer(250);
		timerHeartbeat.Elapsed += delegate { Send(REQ_HEATBEAT); };
		timerHeartbeat.Start();
		
		return true;
	}

	private void Send(string message)
	{
		if (socket != null && socket.Connected)
		{
			StreamWriter writer = new StreamWriter(socket.GetStream());
			writer.WriteLine(message);
			writer.Flush();
		}
	}

	public event EventHandler<ReceivedDataEventArgs> OnData;
	
	private void ListenerLoop()
	{
		StreamReader reader = new StreamReader(socket.GetStream());
		isRunning = true;
		
		while (isRunning)
		{
			string response = string.Empty;
			
			try
			{
				response = reader.ReadLine();
				
				JObject jObject = JObject.Parse(response);
				
				Packet p = new Packet();
				p.RawData = json;
				
				p.Category = (string)jObject["category"];
				p.Request = (string)jObject["request"];
				p.StatusCode = (string)jObject["statuscode"];
				
				JToken values = jObject.GetValue("values");
				
				if (values != null)
				{
					/* 
                  We can further parse the Key-Value pairs from the values here.
                  For example using a switch on the Category and/or Request 
                  to create Gaze Data or CalibrationResult objects and pass these 
                  via separate events.
                */
				}
				
				// Raise event with the data
				if(OnData != null)
					OnData(this, new ReceivedDataEventArgs(p));
			}
			catch (Exception ex)
			{
				Console.Out.WriteLine("Error while reading response: " + ex.Message);
			}
		}
	}
}

public class Packet
{
	public string time = DateTime.UtcNow.Ticks.ToString();
	public string category = string.Empty;
	public string request = string.Empty;
	public string statuscode = string.Empty;
	public string values = string.Empty;
	public string rawData = string.Empty;
	
	public Packet() { }
}

public class ReceivedDataEventArgs : EventArgs
{
	private Packet packet;
	
	public ReceivedDataEventArgs(Packet _packet)
	{
		this.packet = _packet;
	}
	
	public Packet Packet
	{
		get { return packet; }
	}
}