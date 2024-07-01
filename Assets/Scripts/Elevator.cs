using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Elevator : NetworkBehaviour
{
	public Transform Platforma;
	public float ElevatorSpeed = 30.0f;
	public bool isMoving = false;
	public bool isMovingDown = false;
	public Transform targetPositiontest;
	private Vector3 targetPositionDown;
	private Vector3 originalposition;
	public bool elevatorMoving;

	private void Awake()
	{
		originalposition = Platforma.position;
	}

	private void Update()
	{
		if (isMoving)
		{
			Debug.Log("Do góry: " + isMoving);
			elevatorMoving = true;
			float step = ElevatorSpeed * Time.deltaTime;
			Platforma.position = Vector3.MoveTowards(Platforma.position, targetPositiontest.position, step);

			if (Vector3.Distance(Platforma.position, targetPositiontest.position) < 0.01f)
			{
				isMoving = false;
				// Winda dojecha³a do góry
				//elevatorMoving = false;
			}
		}
		else if (isMovingDown)
		{
			Debug.Log("W dó³: " + isMovingDown);
			elevatorMoving = true;
			float stepDown = ElevatorSpeed * Time.deltaTime;
			Platforma.position = Vector3.MoveTowards(Platforma.position, targetPositionDown, stepDown);

			if (Vector3.Distance(Platforma.position, targetPositionDown) < 0.01f)
			{
				isMovingDown = false;
				// Winda zjecha³a na dó³
				//elevatorMoving = false;
			}
		}

		
	}

	private void OnTriggerEnter(Collider other)
	{
		if (isMovingDown)
		{
			isMovingDown = false;
			isMoving = true;
		}
		else
		{
			LiftPlatform();
		}
	}

	private void LiftPlatform()
	{
		isMoving = true;
		elevatorMoving = true;
	}

	private void OnTriggerExit(Collider other)
	{
		DownPlatform();
	}

	private void DownPlatform()
	{
		isMoving = false;
		isMovingDown = true;
		elevatorMoving = true;
		targetPositionDown = originalposition;
	}
}
