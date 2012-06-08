using UnityEngine;
using System.Collections;

public class ConnectionGUI : MonoBehaviour 
{
	public string remoteIP = "127.0.0.1";
	public int remotePort = 25000;
	public int listenPort = 25000;
	public bool useNAT = false;
	public string yourIP = "";
	public string yourPort = "";
	public int maxConnections = 32;
	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	
	void OnGUI()
	{
		//If not connected
		if(Network.peerType == NetworkPeerType.Disconnected)
		{
			if(GUI.Button(new Rect(10,10,100,30), "Connect"))
			{
				Network.Connect(remoteIP, remotePort);
			}
			if(GUI.Button(new Rect(10,50,100,30), "Start Server"))
			{
				Network.InitializeServer(this.maxConnections, this.listenPort, this.useNAT);
				UnityEngine.Object [] objects = FindObjectsOfType(typeof(GameObject));
				//Alert game objects
				foreach(UnityEngine.Object obj in objects)
					((GameObject)obj).SendMessage("OnNetworkLoadedLevel", SendMessageOptions.DontRequireReceiver);
				
			}
			
			remoteIP = GUI.TextField(new Rect(120, 10, 100, 20), remoteIP);
			string port = GUI.TextField(new Rect(230, 10, 40, 20), remotePort.ToString());
			try
			{
				remotePort = int.Parse(port);
			}
			catch(System.Exception except)
			{
				remotePort = 0;
			}
		}
		else
		{
			string ipAddress = Network.player.ipAddress;
			string port = Network.player.port.ToString();
			
			GUI.Label(new Rect(140, 20, 250, 40), "IP Address: " + ipAddress + ":" + port);
			//Disconnect?
			if(GUI.Button(new Rect(10, 10, 100, 50), "Disconnect"))
				Network.Disconnect(200);
		}
	}
	
	void OnConnectedToServer()
	{
		UnityEngine.Object [] objects = FindObjectsOfType(typeof(GameObject));
		//Alert game objects
		foreach(UnityEngine.Object obj in objects)
			((GameObject)obj).SendMessage("OnNetworkLoadedLevel", SendMessageOptions.DontRequireReceiver);
	}
}
