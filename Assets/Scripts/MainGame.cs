using UnityEngine;
using System.Collections;

public class MainGame : MonoBehaviour 
{
	void OnDestroy()
	{
		Network.Disconnect();
	}
	// Use this for initialization
	void Start () 
	{
		Network.InitializeServer(32, 25000, !Network.HavePublicAddress());
		GameObject [] objects = GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[];
		//Alert game objects
		foreach(GameObject obj in objects)
			obj.SendMessage("OnNetworkLoadedLevel", SendMessageOptions.DontRequireReceiver);
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	
	void OnGUI()
	{
		if(GUI.Button (new Rect(10, 10, 100, 50), "Disconnect"))
		{
			Network.Disconnect();
			Application.LoadLevel("MainMenu");
		}
	}
}
