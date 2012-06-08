using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour 
{

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
		GUI.Label(new Rect((Screen.width/2)-80,(Screen.height/2)-130,200,50),"SELECT CONNECTION TYPE");
		GUI.Label(new Rect((Screen.width-220),(Screen.height-30),220,30),"STAR-TROOPER MULTIPLAYER DEMO");
		if(GUI.Button(new Rect((Screen.width/2)-100,(Screen.height/2)-100,200,50),"Master Server Connection"))
		{
			Application.LoadLevel("MasterServer");
		}
		if(GUI.Button(new Rect((Screen.width/2)-100,(Screen.height/2)-40,200,50),"Direct Connection"))
		{
			Application.LoadLevel("MainGame");
		}
		if(GUI.Button(new Rect((Screen.width/2)-100,(Screen.height/2)+20,200,50),"UDPConnection"))
		{
			Application.LoadLevel("UDPServer");
		}
	}
}
