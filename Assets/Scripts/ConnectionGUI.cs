using UnityEngine;
using System.Collections;

namespace nimgoble 
{ 
	namespace gui 
	{
		public class ConnectionGUI : BaseGUI
		{
			public string remoteIP = "127.0.0.1";
			public int remotePort = 25000;
			public int listenPort = 25000;
			public bool useNAT = false;
			public string yourIP = "";
			public string yourPort = "";
			public int maxConnections = 32;
			private DrawGUI guiDelegate;
			private DrawGUI connectedGUI;
			private DrawGUI disconnectedGUI;
			private GUIDone guiDone;
			
			public DrawGUI GetDrawGUIDelegate() 
			{
				return this.guiDelegate;
			}
			
			public ConnectionGUI()
			{
				connectedGUI = new DrawGUI(this.ConnectedGUI);
				disconnectedGUI = new DrawGUI(this.DisconnectedGUI);
				guiDelegate = connectedGUI;
			}
			
			public void Start(GUIDone doneDelegate)
			{
				guiDone = doneDelegate;
			}
			public void Stop()
			{
			}
			
			//GUI to show when we're connected.
			void ConnectedGUI()
			{
				string ipAddress = Network.player.ipAddress;
				string port = Network.player.port.ToString();
				
				GUI.Label(new Rect(140, 20, 250, 40), "IP Address: " + ipAddress + ":" + port);
				//Disconnect?
				if(GUI.Button(new Rect(10, 10, 100, 50), "Disconnect"))
				{
					Network.Disconnect(200);
					guiDelegate = disconnectedGUI;
				}
			}
			//GUI to show when we're disconnected.
			void DisconnectedGUI()
			{
				//Connect to a server
				if(GUI.Button(new Rect(10,10,100,30), "Connect"))
				{
					Network.Connect(remoteIP, remotePort);
					guiDelegate = connectedGUI;
				}
				//Start a server
				if(GUI.Button(new Rect(10,50,100,30), "Start Server"))
				{
					Network.InitializeServer(this.maxConnections, this.listenPort, this.useNAT);
					GameObject [] objects = GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[];
					//Alert game objects
					foreach(GameObject obj in objects)
						obj.SendMessage("OnNetworkLoadedLevel", SendMessageOptions.DontRequireReceiver);
				}
				
				//Allow user to manually enter connection info
				remoteIP = GUI.TextField(new Rect(120, 10, 100, 20), remoteIP);
				string port = GUI.TextField(new Rect(230, 10, 40, 20), remotePort.ToString());
				//If text box is empty, make it '0'
				try
				{
					remotePort = int.Parse(port);
				}
				catch
				{
					remotePort = 0;
				}
			}
		}
	}
}
