using UnityEngine;
using System.Collections;

public class MasterServerGUI : MonoBehaviour 
{
	public string gameName = "YourGameName";
	public int serverPort = 25002;
	private double timeoutHostList = 0.0;
	private double lastHostListRequest = -1000.0;
	private double hostListRefreshTimeout = 10.0;
	public int maxPlayers = 32;
	
	private ConnectionTesterStatus natCapable = ConnectionTesterStatus.Undetermined;
	private bool filterNATHosts = false;
	private bool probingPublicIP = false;
	private bool doneTesting = false;
	private float timer = 0.0f;
	
	private Rect windowRect = new Rect(Screen.width - 300, 0, 300, 100);
	private bool hideTest = false;
	private string testMessage = "Undetermined NAT capabilities";
	
	//Enable this if not running a client on the server machine
	// MasterServer.dedicatedServer = true;
	
	void OnFailedToConnectToMasterServer(NetworkConnectionError info)
	{
		Debug.Log(info);
	}
	
	void OnFailedToConnect(NetworkConnectionError info)
	{
		Debug.Log(info);
	}
	
	void OnGUI()
	{
		ShowGUI();
	}
	// Use this for initialization
	void Awake () 
	{
		DontDestroyOnLoad(this);
		
		//Start connection test
		natCapable = Network.TestConnection();
		
		/*What kind of IP does this machine have?  TestConnection also indicates this
		 * in the test results*/
		if(Network.HavePublicAddress())
			Debug.Log("This machine has a public IP address");
		else
			Debug.Log("This machine has a private IP address");
	}
	
	// Update is called once per frame
	void Update () 
	{
		//If test is undetermined, keep running
		if(!doneTesting)
			TestConnection();
	}
	
	void TestConnection()
	{
		//Start/Poll the connection test, report the results in a label and react to
		//the results accordingly
		natCapable = Network.TestConnection();
		switch(natCapable)
		{
		case ConnectionTesterStatus.Error:
			testMessage = "Problem determining NAT capabilities";
			doneTesting = true;
			break;
		case ConnectionTesterStatus.Undetermined:
			testMessage = "Undetermined NAT capabilities";
			doneTesting = false;
			break;
		case ConnectionTesterStatus.PublicIPPortBlocked:
			testMessage = "Non-connectible public IP address (port " + serverPort + " blocked),"
				+ " running a server is impossible.";
			//If no NAT punchthrough test has been performed on this public IP, force a test
			if(!probingPublicIP)
			{
				Debug.Log("Testing if firewall can be circumvented");
				natCapable = Network.TestConnectionNAT();
				probingPublicIP = true;
				timer = Time.time + 10;
			}
			else if(Time.time > timer)
			{
				probingPublicIP = false;
				doneTesting = true;
			}
			break;
		case ConnectionTesterStatus.PublicIPNoServerStarted:
			testMessage = "Public IP address but server not initialized," +
				" it must be started to check server accessibility. " +
				"Restart connection test when ready.";
			break;
		default:
			testMessage = "Error in test routine, got " + natCapable;
			break;
		}
	}
	
	void ShowGUI()
	{
		if(GUI.Button (new Rect(100, 10, 120, 30), "Retest connection"))
		{
			Debug.Log ("Redoing connection test");
			probingPublicIP = false;
			doneTesting = false;
			natCapable = Network.TestConnection(true);
		}
		
		if(Network.peerType == NetworkPeerType.Disconnected)
		{
			//Start new server
			if(GUI.Button (new Rect(10, 10, 90, 30), "Start Server"))
			{
				Network.InitializeServer(maxPlayers, serverPort);
				MasterServer.updateRate = 3;
				MasterServer.RegisterHost(gameName, "stuff", "profas chat test");
			}
			
			//Refresh hosts
			if(GUI.Button (new Rect(10, 40, 210, 30), "Refresh Available Servers") ||
				Time.realtimeSinceStartup > lastHostListRequest + hostListRefreshTimeout)
			{
				MasterServer.ClearHostList();
				MasterServer.RequestHostList(gameName);
				lastHostListRequest = Time.realtimeSinceStartup;
				Debug.Log("Refresh Click");
			}
			
			HostData [] hostData = MasterServer.PollHostList();
			int count = 0;
			foreach(HostData data in hostData)
			{
				//Do not display NAT enabled games if we cannot do NAT punchthrough
				if(!(filterNATHosts && data.useNat))
				{
					string name = System.String.Format("{0} {1}/{2}",
						data.gameName, data.connectedPlayers, data.playerLimit);
					string hostInfo = "[";
					foreach(string host in data.ip)
						hostInfo += (host + ":" + data.port + " ");
					
					hostInfo += "]";
					
					if(GUI.Button(new Rect(20, (count * 50) + 90, 400, 40), hostInfo))
					{
						//Enable NAT functionality based on what the hosts are configured to do
						Network.Connect(data);
					}
						
					
				}
			}
			
		}
		else
		{
		}
	}
}
