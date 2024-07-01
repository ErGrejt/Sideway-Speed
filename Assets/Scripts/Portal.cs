using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class Portal : NetworkBehaviour
{
    private Transform teleportDestination;
	private Transform teleportDestintonSecond;
    private Rigidbody rb;

	private void Start()
	{
		teleportDestination = GameObject.Find("TeleportPlace").transform;
		teleportDestintonSecond = GameObject.Find("TeleportPlaceSecond").transform;
		rb = GetComponent<Rigidbody>();
	}

	private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("TeleportEntry"))
		{
			rb.MovePosition(teleportDestination.position);
			rb.velocity = Vector3.zero;
			rb.rotation = teleportDestination.rotation;
		}
		if (other.CompareTag("TeleportEntrySecond"))
		{
			rb.MovePosition(teleportDestintonSecond.position);
			rb.velocity = Vector3.zero;
			rb.rotation = teleportDestintonSecond.rotation;
		}
	}
  

}
