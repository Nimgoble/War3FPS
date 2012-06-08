using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NetworkView))]
public class NetworkLevelLoad : MonoBehaviour 
{
	private int lastLevelPrefix = 0;
	// Use this for initialization
	void Awake () 
	{
		DontDestroyOnLoad(this);
		networkView.group = 1;
		Application.LoadLevel("EmptyScene");
	}
	
	void OnGUI()
	{
		if(Network.peerType == NetworkPeerType.Disconnected)
			return;
		
		if(GUI.Button(new Rect(350, 10, 100, 30), "Main Game"))
		{
			Network.RemoveRPCsInGroup(0);
			Network.RemoveRPCsInGroup(1);
			networkView.RPC("LoadLevel", RPCMode.AllBuffered, "MainGame", lastLevelPrefix + 1);
		}
	}
	
	[RPC]
	public void LoadLevel(string level, int levelPrefix)
	{
		Debug.Log("Loading level " + level + " with prefix " + levelPrefix.ToString());
		lastLevelPrefix = levelPrefix;
		
		/*There is no reason to send any more data over the network on the default
		 * channel, because we are about to load the level, because all of those objects
		 * in the level are allowed to fire
		 */
		Network.SetSendingEnabled(0, false);
		
		/*We need to stop receiving because first the level must be loaded.
		 * Once the level is loaded, RPC's and other state update attached to objects in the
		 * level are allowed to fire*/
		Network.isMessageQueueRunning = false;
		
		/*All  network views loaded from a level will get a prefix into their NetworkViewID.
		 * This will prevent old updates from clients leaking into a newly created scene*/
		Network.SetLevelPrefix(levelPrefix);
		Application.LoadLevel(level);
		
		//Allow receiving data again
		Network.isMessageQueueRunning = true;
		//Now the level has been loaded and we can start sending out data
		Network.SetSendingEnabled(0, true);
		
		//Notify our objects that the level and the network is ready
		Transform [] objects = FindObjectsOfType(typeof(Transform)) as Transform[];
		foreach(Transform trans in objects)
			trans.SendMessage("OnNetworkLoadedLevel", SendMessageOptions.DontRequireReceiver);
	}
	
	public void OnDisconnectedFromServer()
	{
		Application.LoadLevel("EmptyScene");
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
