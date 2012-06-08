using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace nimgoble
{
	namespace gui
	{
		public class UDPConnectionGUI : BaseGUI
		{
			private string maxPlayers = "32";
			private UdpClient server;
			private UdpClient client;
			private IPEndPoint recievePoint;
			private IPEndPoint sendPoint;
			private int port = 6767;
			private int listenPort = 25001;
			private string ip = "0.0.0.0";
			private string ipBroadcast = "255.255.255.255";
			private bool youServer = false;
			private bool connected = false;
			private string serverName = "";
			private int clearList = 0;
			private string networkAddress;
			private Thread serverThread;
			private Thread clientThread;
			
			public DrawGUI guiDelegate;
			private DrawGUI connectedGUI;
			private DrawGUI disconnectedGUI;
			private GUIDone guiDone;
			
			public DrawGUI GetDrawGUIDelegate() 
			{
				return this.guiDelegate;
			}
			
			public UDPConnectionGUI()
			{
				connectedGUI = new DrawGUI(this.ConnectedGUI);
				disconnectedGUI = new DrawGUI(this.DisconnectedGUI);
				serverThread = new Thread(new ThreadStart(StartServer));
				clientThread = new Thread(new ThreadStart(StartClient));
				networkAddress = Network.player.ipAddress.ToString();
				sendPoint = new IPEndPoint(IPAddress.Parse(this.ipBroadcast), this.port);
			}
			
			~UDPConnectionGUI()
			{
			}
			
			// Use this for initialization
			public void Start (GUIDone doneDelegate) 
			{
				Debug.Log("Start");
				guiDelegate = disconnectedGUI;
				guiDone = doneDelegate;
				LoadClient();
			}
			
			//Clean up
			public void Stop()
			{
				Network.Disconnect();
				if(this.client != null)
					this.client.Close();
				if(this.server != null)
					this.server.Close();
				if(serverThread.IsAlive)
					serverThread.Interrupt();
				if(clientThread.IsAlive)
					clientThread.Interrupt();
			}
			
			//Load us as a client
			public void LoadClient()
			{
				this.client = new UdpClient(0);
				this.recievePoint = new IPEndPoint(IPAddress.Parse(this.ip), this.port);
				clientThread.Start();
			}
			//Start client
			public void StartClient()
			{
				System.Text.ASCIIEncoding encode = new System.Text.ASCIIEncoding();
				try
				{
					//Receive loop
					while(true)
					{
						try
						{
							//Listen for any servers
							byte[] recvData = this.client.Receive(ref this.recievePoint);
							serverName = encode.GetString(recvData);
							
							//If we're connected, close us, then close the client.
							if(connected)
							{
								serverName = "";
								this.client.Close();
								break;
							}
						}
						catch(System.Exception e)
						{
							Debug.Log(e.ToString());
							break;
						}
					}
				}
				catch(System.Exception except)
				{
					Debug.Log(except.ToString());
				}
			}
			//Start the server
			public void StartServer()
			{
				try
				{
					System.Text.ASCIIEncoding encode = new System.Text.ASCIIEncoding();
					//Send loop
					while(true)
					{
						//Send out our IP so client know who to connect to.
						byte[] sendData = encode.GetBytes(networkAddress);
						this.server.Send(sendData, sendData.Length, this.sendPoint);
						//Every 100 ms
						Thread.Sleep(100);
					}
				}
				catch(System.Exception except)
				{
					Debug.Log(except.ToString());
				}
			}
			
			void ConnectedGUI()
			{
				//We are a server.  Do we want to disconnect?
				if(GUI.Button(new Rect(10, 10, 100, 30), "Disconnect"))
				{
					Network.Disconnect();
					youServer = false;
					this.server.Close();
					LoadClient();
					
				}
			}
			
			void DisconnectedGUI()
			{
				//We're a client.  Do we want to start a server?
				if(GUI.Button(new Rect(10, 10, 100, 30), "Start Server"))
				{
					youServer = true;
					Network.InitializeServer(int.Parse(maxPlayers), this.listenPort, !Network.HavePublicAddress());
					string ipaddress = Network.player.ipAddress.ToString();
					ip = ipaddress;
					this.client.Close();
					server = new UdpClient(0);
					this.recievePoint = new IPEndPoint(IPAddress.Parse(this.ip), this.port);
					serverThread.Start();
				}
				
				if(GUI.Button (new Rect(10, 40, 100, 30), "Main Menu"))
				{
					guiDone();
				}
		
				if(serverName != "")
				{
					//We have a server name, so make a button that allows us to connect to it.
					if(GUI.Button(new Rect(20, 100, 200, 50), serverName))
					{
						connected = true;
						Network.Connect(serverName, listenPort);
					}
				}
			}
		}
	}
}
