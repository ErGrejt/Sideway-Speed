using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	public Transform target;
	public float distance = 4.0f; 
	public float sensitivity = 2.0f;

	public float UpCameraLimit = 89f;
	public float DownCameraLimit = -10f;

	private float currentX = 0.0f;
	private float currentY = 0.0f;
	void Start()
	{
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
		currentX = target.eulerAngles.y;
		currentY = 22.0f;
	}
	void LateUpdate()
	{
		currentX += Input.GetAxis("Mouse X") * sensitivity;
		currentY -= Input.GetAxis("Mouse Y") * sensitivity;
		currentY = Mathf.Clamp(currentY, -DownCameraLimit, UpCameraLimit); 
		Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
		Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
		Vector3 position = rotation * negDistance + target.position;
		transform.rotation = rotation;
		transform.position = position;

		if (Input.GetKey(KeyCode.V))
		{
			ResetCamera();
		}
	}

	void ResetCamera()
	{
		currentX = target.eulerAngles.y;
		currentY = 22.0f;
	}
}
