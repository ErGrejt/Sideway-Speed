using UnityEngine;
using Unity.Netcode;
using System;

public class DisableCameraForHost : NetworkBehaviour
{
    public Camera camera;
    public Transform HostSpawn;
    public Transform ClientSpawn;

    void Start()
    {
        HostSpawn = GameObject.Find("SpawnHost").transform;
        ClientSpawn = GameObject.Find("SpawnClient").transform;
        if (!IsOwner)
        {
            camera.enabled = false;
        }

        if(IsLocalPlayer)
        {
            if(IsHost)
            {
                transform.position = HostSpawn.position;
				transform.rotation = HostSpawn.rotation;

			} else
            {
                transform.position = ClientSpawn.position;
                transform.rotation = ClientSpawn.rotation;

			}
        }
    }
}