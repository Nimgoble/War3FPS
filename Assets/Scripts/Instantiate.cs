using UnityEngine;
using System.Collections;

public class Instantiate : MonoBehaviour 
{
	public UnityEngine.Object playerPrefab;
	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	
	void OnNetworkLoadedLevel()
	{
		Network.Instantiate(playerPrefab, this.transform.position, this.transform.rotation, 0);
	}
	
	void OnPlayerDisconnected(NetworkPlayer player)
	{
		Network.RemoveRPCs(player);
		Network.DestroyPlayerObjects(player);
	}
}
