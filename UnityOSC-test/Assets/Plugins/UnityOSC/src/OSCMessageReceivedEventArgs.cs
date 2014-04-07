
using System;
namespace UnityOSC
{
	public class OSCMessageReceivedEventArgs : EventArgs
	{
		public string ServerName {get; internal set;}
		public string Log {get; internal set;}

		public OSCMessageReceivedEventArgs(string name, string log)
		{
			this.ServerName = name;
			this.Log = log;
		}
	}
}

