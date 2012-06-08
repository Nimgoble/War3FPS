using UnityEngine;
using System.Collections;
using nimgoble.gui;

public class Menu : MonoBehaviour 
{
	private GUIDone guiDone;
	private DrawGUI guiDelegate;
	private DrawGUI internalGUIDelegate;
	private MasterServerGUI masterServerGUI;
	private UDPConnectionGUI udpConnectionGUI;
	private BaseGUI currentGUI;
	// Use this for initialization
	void Start () 
	{
		masterServerGUI = new MasterServerGUI();
		udpConnectionGUI = new UDPConnectionGUI();
		guiDelegate = new DrawGUI(this.MainMenu);
		internalGUIDelegate = new DrawGUI(this.MainMenu);
		guiDone = new GUIDone(this.OnGUIDone);
		currentGUI = null;
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	
	void OnGUI()
	{
		guiDelegate();
	}
	
	void MainMenu()
	{
		GUI.Label(new Rect((Screen.width/2)-80,(Screen.height/2)-130,200,50),"SELECT CONNECTION TYPE");
		GUI.Label(new Rect((Screen.width-220),(Screen.height-30),220,30),"MULTIPLAYER DEMO");
		if(GUI.Button(new Rect((Screen.width/2)-100,(Screen.height/2)-100,200,50),"Master Server Connection"))
		{
			currentGUI = masterServerGUI;
			currentGUI.Start(this.guiDone);
			guiDelegate = currentGUI.GetDrawGUIDelegate();
		}
		if(GUI.Button(new Rect((Screen.width/2)-100,(Screen.height/2)-40,200,50),"Direct Connection"))
		{
			//Network.InitializeServer(this.maxConnections, this.listenPort, this.useNAT);
			masterServerGUI.Stop();
			udpConnectionGUI.Stop ();
			Application.LoadLevel("MainGame");
		}
		if(GUI.Button(new Rect((Screen.width/2)-100,(Screen.height/2)+20,200,50),"UDPConnection"))
		{
			currentGUI = udpConnectionGUI;
			currentGUI.Start(this.guiDone);
			guiDelegate = currentGUI.GetDrawGUIDelegate();
		}
	}
	
	void OnGUIDone()
	{
		currentGUI.Stop();
		guiDelegate = this.internalGUIDelegate;
	}
}
