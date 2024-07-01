using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerTagSetter : NetworkBehaviour
{
	public override void OnNetworkSpawn()
	{
		base.OnNetworkSpawn();
		if(GameObject.FindWithTag("HostPlayer") == null)
		{
			gameObject.tag = "HostPlayer";
		} else
		{
			gameObject.tag = "ClientPlayer";
		}
	}
}
