//kom
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;
using System.Threading.Tasks;
using Unity.Netcode.Components;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;

public class PrometeoCarController : NetworkBehaviour
{
	[Space(20)]
	//[Header("CAR SETUP")]
	[Space(10)]
	[Range(20, 260)]
	public int maxSpeed = 240; //The maximum speed that the car can reach in km/h.
	[Range(10, 120)]
	public int maxReverseSpeed = 45; //The maximum speed that the car can reach while going on reverse in km/h.
	[Range(1, 10)]
	public int accelerationMultiplier = 2; // How fast the car can accelerate. 1 is a slow acceleration and 10 is the fastest.
	[Space(10)]
	[Range(10, 45)]
	public int maxSteeringAngle = 27; // The maximum angle that the tires can reach while rotating the steering wheel.
	[Range(0.1f, 1f)]
	public float steeringSpeed = 0.5f; // How fast the steering wheel turns.
	[Space(10)]
	[Range(100, 600)]
	public int brakeForce = 350; // The strength of the wheel brakes.
	[Range(1, 10)]
	public int decelerationMultiplier = 2; // How fast the car decelerates when the user is not using the throttle.
	[Range(1, 10)]
	public int handbrakeDriftMultiplier = 5; // How much grip the car loses when the user hit the handbrake.
	[Space(10)]
	public Vector3 bodyMassCenter; // This is a vector that contains the center of mass of the car. I recommend to set this value
								   // in the points x = 0 and z = 0 of your car. You can select the value that you want in the y axis,
								   // however, you must notice that the higher this value is, the more unstable the car becomes.
								   // Usually the y value goes from 0 to 1.5.
	public GameObject frontLeftMesh;
	public WheelCollider frontLeftCollider;
	[Space(10)]
	public GameObject frontRightMesh;
	public WheelCollider frontRightCollider;
	[Space(10)]
	public GameObject rearLeftMesh;
	public WheelCollider rearLeftCollider;
	[Space(10)]
	public GameObject rearRightMesh;
	public WheelCollider rearRightCollider;

	[Space(20)]
	//[Header("EFFECTS")]
	[Space(10)]
	//The following variable lets you to set up particle systems in your car
	public bool useEffects = false;

	// The following particle systems are used as tire smoke when the car drifts.
	public ParticleSystem RLWParticleSystem;
	public ParticleSystem RRWParticleSystem;

	[Space(10)]
	// The following trail renderers are used as tire skids when the car loses traction.
	public TrailRenderer RLWTireSkid;
	public TrailRenderer RRWTireSkid;

	[Space(20)]
	//[Header("UI")]
	[Space(10)]
	//The following variable lets you to set up a UI text to display the speed of your car.
	public bool useUI = false;
	public Text carSpeedText; // Used to store the UI object that is going to show the speed of the car.

	[Space(20)]
	//[Header("Sounds")]
	[Space(10)]
	//The following variable lets you to set up sounds for your car such as the car engine or tire screech sounds.
	public bool useSounds = false;
	public AudioSource carEngineSound; // This variable stores the sound of the car engine.
	public AudioSource tireScreechSound; // This variable stores the sound of the tire screech (when the car is drifting).
	float initialCarEngineSoundPitch; // Used to store the initial pitch of the car engine sound.

	[Space(20)]
	//[Header("CONTROLS")]
	[Space(10)]
	//The following variables lets you to set up touch controls for mobile devices.
	public bool useTouchControls = false;
	public GameObject throttleButton;
	PrometeoTouchInput throttlePTI;
	public GameObject reverseButton;
	PrometeoTouchInput reversePTI;
	public GameObject turnRightButton;
	PrometeoTouchInput turnRightPTI;
	public GameObject turnLeftButton;
	PrometeoTouchInput turnLeftPTI;
	public GameObject handbrakeButton;
	PrometeoTouchInput handbrakePTI;


	Rigidbody carRigidbody; // Stores the car's rigidbody.
	float steeringAxis; // Used to know whether the steering wheel has reached the maximum value. It goes from -1 to 1.
	float throttleAxis; // Used to know whether the throttle has reached the maximum value. It goes from -1 to 1.
	float driftingAxis;
	float localVelocityZ;
	float localVelocityX;
	bool deceleratingCar;

	WheelFrictionCurve FLwheelFriction;
	float FLWextremumSlip;
	WheelFrictionCurve FRwheelFriction;
	float FRWextremumSlip;
	WheelFrictionCurve RLwheelFriction;
	float RLWextremumSlip;
	WheelFrictionCurve RRwheelFriction;
	float RRWextremumSlip;

	private Transform RespawnHostDestination;
	private Transform RespawnClientDestination;
	//Checkpoints
	private Transform Checkpoint1;
	private Transform Checkpoint2;
	// Start is called before the first frame update



	public float carSpeed; // Used to store the speed of the car.
	public bool isDrifting; // Used to know whether the car is drifting or not.
	public bool isTractionLocked; // Used to know whether the traction of the car is locked or not.


	private float currentScore;
	private float totalScore;
	private float driftFactor = 1;
	private IEnumerator stopDriftingCoroutine = null;
	private float driftAngle = 0;

	public float minimumSpeed = 5;
	public float minimumAngle = 10;
	public float driftingDelay = 0.02f;
	public GameObject driftingObject;

	private TMP_Text totalScoreText;
	private TMP_Text currentDrift;
	private TMP_Text currentScoreText;
	private TMP_Text factorText;

	private GameObject elevator;
	private bool isOnElevator = false;
	private Vector3 localPositionOffset;
	private GameObject elevatorVertical;
	private bool RespawnBlockade = false;

	private TMP_Text RespawnError;

	private float minSpeedArrowAngle = 23;
	private float maxSpeedArrowAngle = -205;


	private TMP_Text speedLabel;
	private RectTransform arrow;
	private Camera camera;
	private float step = 500f;
	private GameObject gateDriftScore;
	private bool gateActive = true;
	//Gondola
	private Transform elevatortransformtest;
	public Elevator elevatorScriptTest;
	public Elevator elevatorScriptTestSecond;
	private bool testUP;
	private bool testUPSecond;
	private bool testDOWN;
	private bool testDOWNSecond;
	//Winda pionowa
	private Transform elevatorverticaltransformtest;
	public Elevator elevatorScriptVertical;
	public Elevator elevatorScriptVerticalSecond;
	private bool VerticalUP;
	private bool VerticalUPSecond;
	private bool VerticalDOWN;
	private bool VerticalDOWNSecond;

	void Start()
	{
		//Gondola
		GameObject scriptsearch = GameObject.Find("GondolaButton");
		elevatorScriptTest = scriptsearch.GetComponent<Elevator>();
		GameObject scriptsearchsecond = GameObject.Find("GondolaButtonSecond");
		elevatorScriptTestSecond = scriptsearchsecond.GetComponent<Elevator>();
		elevatortransformtest = GameObject.Find("ClientSet").transform;
		//Winda pionowa
		GameObject scriptverticalsearch = GameObject.Find("VerticalElevatorButton");
		elevatorScriptVertical = scriptverticalsearch.GetComponent<Elevator>();
		GameObject scriptverticalsearchsecond = GameObject.Find("VerticalElevatorButtonSecond");
		elevatorScriptVerticalSecond = scriptverticalsearchsecond.GetComponent<Elevator>();
		elevatorverticaltransformtest = GameObject.Find("ClientVerticalSet").transform;

		camera = GetComponentInChildren<Camera>();
		GameObject SpeedLabel = GameObject.Find("SpeedLabel");
		speedLabel = SpeedLabel.GetComponent<TMP_Text>();

		gateDriftScore = GameObject.Find("BramaDriftScore");


		GameObject FindArrow = GameObject.Find("SpeedArrow");
		arrow = FindArrow.GetComponent<RectTransform>();

		elevator = GameObject.Find("Main");
		elevatorVertical = GameObject.Find("MainVertical");

		driftingObject = GameObject.Find("DriftPanel");

		GameObject scoreGameObject = GameObject.Find("TotalScore");
		totalScoreText = scoreGameObject.GetComponent<TMP_Text>();

		GameObject errorRespawn = GameObject.Find("ErrorRespawn");
		RespawnError = errorRespawn.GetComponent<TMP_Text>();

		GameObject currentDriftObject = GameObject.Find("CurrentDrift");
		currentDrift = currentDriftObject.GetComponent<TMP_Text>();

		GameObject currentscoreGameObject = GameObject.Find("CurrentScore");
		currentScoreText = currentscoreGameObject.GetComponent<TMP_Text>();

		GameObject factorGameObject = GameObject.Find("Factor");
		factorText = factorGameObject.GetComponent<TMP_Text>();

		RespawnHostDestination = GameObject.Find("RespawnHost").transform;
		RespawnClientDestination = GameObject.Find("RespawnClient").transform;
		//Checkpoint init
		Checkpoint1 = GameObject.Find("CheckpointSpawn").transform;
		//Checkpoint2 = GameObject.Find("CheckpointSpawn2").transform;
		carRigidbody = gameObject.GetComponent<Rigidbody>();
		carRigidbody.centerOfMass = bodyMassCenter;

		FLwheelFriction = new WheelFrictionCurve();
		FLwheelFriction.extremumSlip = frontLeftCollider.sidewaysFriction.extremumSlip;
		FLWextremumSlip = frontLeftCollider.sidewaysFriction.extremumSlip;
		FLwheelFriction.extremumValue = frontLeftCollider.sidewaysFriction.extremumValue;
		FLwheelFriction.asymptoteSlip = frontLeftCollider.sidewaysFriction.asymptoteSlip;
		FLwheelFriction.asymptoteValue = frontLeftCollider.sidewaysFriction.asymptoteValue;
		FLwheelFriction.stiffness = frontLeftCollider.sidewaysFriction.stiffness;
		FRwheelFriction = new WheelFrictionCurve();
		FRwheelFriction.extremumSlip = frontRightCollider.sidewaysFriction.extremumSlip;
		FRWextremumSlip = frontRightCollider.sidewaysFriction.extremumSlip;
		FRwheelFriction.extremumValue = frontRightCollider.sidewaysFriction.extremumValue;
		FRwheelFriction.asymptoteSlip = frontRightCollider.sidewaysFriction.asymptoteSlip;
		FRwheelFriction.asymptoteValue = frontRightCollider.sidewaysFriction.asymptoteValue;
		FRwheelFriction.stiffness = frontRightCollider.sidewaysFriction.stiffness;
		RLwheelFriction = new WheelFrictionCurve();
		RLwheelFriction.extremumSlip = rearLeftCollider.sidewaysFriction.extremumSlip;
		RLWextremumSlip = rearLeftCollider.sidewaysFriction.extremumSlip;
		RLwheelFriction.extremumValue = rearLeftCollider.sidewaysFriction.extremumValue;
		RLwheelFriction.asymptoteSlip = rearLeftCollider.sidewaysFriction.asymptoteSlip;
		RLwheelFriction.asymptoteValue = rearLeftCollider.sidewaysFriction.asymptoteValue;
		RLwheelFriction.stiffness = rearLeftCollider.sidewaysFriction.stiffness;
		RRwheelFriction = new WheelFrictionCurve();
		RRwheelFriction.extremumSlip = rearRightCollider.sidewaysFriction.extremumSlip;
		RRWextremumSlip = rearRightCollider.sidewaysFriction.extremumSlip;
		RRwheelFriction.extremumValue = rearRightCollider.sidewaysFriction.extremumValue;
		RRwheelFriction.asymptoteSlip = rearRightCollider.sidewaysFriction.asymptoteSlip;
		RRwheelFriction.asymptoteValue = rearRightCollider.sidewaysFriction.asymptoteValue;
		RRwheelFriction.stiffness = rearRightCollider.sidewaysFriction.stiffness;


		if (carEngineSound != null)
		{
			initialCarEngineSoundPitch = carEngineSound.pitch;
		}

		if (useUI)
		{
			InvokeRepeating("CarSpeedUI", 0f, 0.1f);
		}
		else if (!useUI)
		{
			if (carSpeedText != null)
			{
				carSpeedText.text = "0";
			}
		}

		if (useSounds)
		{
			InvokeRepeating("CarSounds", 0f, 0.1f);
		}
		else if (!useSounds)
		{
			if (carEngineSound != null)
			{
				carEngineSound.Stop();
			}
			if (tireScreechSound != null)
			{
				tireScreechSound.Stop();
			}
		}

		if (!useEffects)
		{
			if (RLWParticleSystem != null)
			{
				RLWParticleSystem.Stop();
			}
			if (RRWParticleSystem != null)
			{
				RRWParticleSystem.Stop();
			}
			if (RLWTireSkid != null)
			{
				RLWTireSkid.emitting = false;
			}
			if (RRWTireSkid != null)
			{
				RRWTireSkid.emitting = false;
			}
		}
	}
	// Update is called once per frame
	void Update()
	{

		if (!IsOwner) return;
		// We determine the speed of the car.
		carSpeed = (2 * Mathf.PI * frontLeftCollider.radius * frontLeftCollider.rpm * 60) / 1000;
		// Save the local velocity of the car in the x axis. Used to know if the car is drifting.
		localVelocityX = transform.InverseTransformDirection(carRigidbody.velocity).x;
		// Save the local velocity of the car in the z axis. Used to know if the car is going forward or backwards.
		localVelocityZ = transform.InverseTransformDirection(carRigidbody.velocity).z;

		//Zmienne gondoli
		testUP = elevatorScriptTest.isMoving;
		testDOWN = elevatorScriptTest.isMovingDown;
		testUPSecond = elevatorScriptTestSecond.isMoving;
		testDOWNSecond = elevatorScriptTestSecond.isMovingDown;
		//Zmienne windy pionowej
		VerticalUP = elevatorScriptVertical.isMoving;
		VerticalDOWN = elevatorScriptVertical.isMovingDown;
		VerticalUPSecond = elevatorScriptVerticalSecond.isMoving;
		VerticalDOWNSecond = elevatorScriptVerticalSecond.isMovingDown;

		ManageDrift();
		ManageUI();
		Speedometer();

		if (Input.GetKey(KeyCode.W))
		{
			CancelInvoke("DecelerateCar");
			deceleratingCar = false;
			GoForwardServerRpc();
		}
		if (Input.GetKey(KeyCode.S))
		{
			CancelInvoke("DecelerateCar");
			deceleratingCar = false;
			GoReverseServerRpc();
		}

		if (Input.GetKey(KeyCode.A))
		{
			TurnLeftServerRpc();
		}
		if (Input.GetKey(KeyCode.D))
		{
			TurnRightServerRpc();
		}
		if (Input.GetKey(KeyCode.Space))
		{
			CancelInvoke("DecelerateCar");
			deceleratingCar = false;
			HandbrakeServerRpc();
		}
		if (Input.GetKeyUp(KeyCode.Space))
		{
			RecoverTractionServerRpc();

		}
		if ((!Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W)))
		{
			ThrottleOffServerRpc();

		}
		if ((!Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W)) && !Input.GetKey(KeyCode.Space) && !deceleratingCar)
		{
			InvokeRepeating("DecelerateCar", 0f, 0.1f);
			deceleratingCar = true;
		}
		if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D) && steeringAxis != 0f)
		{
			ResetSteeringAngleServerRpc();

		}
		if (Input.GetKeyDown(KeyCode.R))
		{
			Respawn();
			Debug.Log("Respawn");
		}
		if (Input.GetKeyDown(KeyCode.PageUp))
		{
			if (camera.farClipPlane < 4000)
			{
				camera.farClipPlane += step;
			}
		}
		if (Input.GetKeyDown(KeyCode.PageDown))
		{
			if (camera.farClipPlane > 500)
			{
				camera.farClipPlane -= step;
			}
		}

		if (isOnElevator && ((testDOWN || testUP) || (testUPSecond || testDOWNSecond)))
		{
			transform.position = elevatortransformtest.position;
		}
		if (isOnElevator && ((VerticalDOWN || VerticalUP) || (VerticalUPSecond || VerticalDOWNSecond)))
		{
			transform.position = elevatorverticaltransformtest.position;
		}

		// We call the method AnimateWheelMeshes() in order to match the wheel collider movements with the 3D meshes of the wheels.
		AnimateServerRpc();
	}
	
	IEnumerator Waiting(float time)
	{
		yield return new WaitForSeconds(time);
		RespawnError.text = "";
	}
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Checkpoint1"))
		{
			if (IsHost)
			{
				RespawnHostDestination = Checkpoint1;
				RespawnError.text = "Nowy checkpoint ustawiony";
				StartCoroutine(Waiting(2.0f));
			}
			else
			{
				RespawnClientDestination = Checkpoint1;
				RespawnError.text = "Nowy checkpoint ustawiony";
				StartCoroutine(Waiting(2.0f));
			}
		}
		if (other.CompareTag("Checkpoint2"))
		{
			if (IsHost)
			{
				RespawnHostDestination = Checkpoint2;
			}
			else
			{
				RespawnClientDestination = Checkpoint2;
			}
		}
		//Trigger gondoli
		if (other.CompareTag("Trigggger"))
		{
			RespawnBlockade = true;
			if (IsServer)
			{
				SetParentClientRpc(this.GetComponent<NetworkObject>().NetworkObjectId, elevator.GetComponent<NetworkObject>().NetworkObjectId);
			}
			else
			{
				isOnElevator = true;
				localPositionOffset = transform.position - elevatortransformtest.transform.position;
				SetParentServerRpc(this.GetComponent<NetworkObject>().NetworkObjectId, elevator.GetComponent<NetworkObject>().NetworkObjectId);
			}
		}
		//Trigger windy pionowej
		if (other.CompareTag("TriggggerVertical"))
		{
			RespawnBlockade = true;
			if (IsServer)
			{
				SetParentClientRpc(this.GetComponent<NetworkObject>().NetworkObjectId, elevatorVertical.GetComponent<NetworkObject>().NetworkObjectId);
			}
			else
			{
				isOnElevator = true;
				localPositionOffset = transform.position - elevatorverticaltransformtest.transform.position;
				SetParentServerRpc(this.GetComponent<NetworkObject>().NetworkObjectId, elevatorVertical.GetComponent<NetworkObject>().NetworkObjectId);
			}
		}
	}


	private void OnTriggerExit(Collider other)
	{
		//Trigger gondoli
		if (other.CompareTag("Trigggger"))
		{
			RespawnBlockade = false;
			if (IsServer)
			{
				ResetParentClientRpc(this.GetComponent<NetworkObject>().NetworkObjectId);
			}
			else
			{
				isOnElevator = false;
				ResetParentServerRpc(this.GetComponent<NetworkObject>().NetworkObjectId);
			}
		}
		//Trigger windy vertical
		if (other.CompareTag("TriggggerVertical"))
		{
			RespawnBlockade = false;
			if (IsServer)
			{
				ResetParentClientRpc(this.GetComponent<NetworkObject>().NetworkObjectId);
			}
			else
			{
				isOnElevator = false;
				ResetParentServerRpc(this.GetComponent<NetworkObject>().NetworkObjectId);
			}
		}
	}
	// This method converts the car speed data from float to string, and then set the text of the UI carSpeedText with this value.
	public void CarSpeedUI()
	{

		if (useUI)
		{
			try
			{
				float absoluteCarSpeed = Mathf.Abs(carSpeed);
				carSpeedText.text = Mathf.RoundToInt(absoluteCarSpeed).ToString();
			}
			catch (Exception ex)
			{
				Debug.LogWarning(ex);
			}
		}

	}
	public void Respawn()
	{
		if (RespawnBlockade == false)
		{
			Transform respawnDestination;
			if (IsHost)
			{
				respawnDestination = RespawnHostDestination;
			}
			else
			{
				respawnDestination = RespawnClientDestination;
			}
			carRigidbody.MovePosition(respawnDestination.position);
			carRigidbody.MoveRotation(respawnDestination.rotation);
			carRigidbody.velocity = Vector3.zero;
			carRigidbody.angularVelocity = Vector3.zero;
		}
		else
		{
			RespawnError.text = "Respawn niemozliwy w windzie";
			StartCoroutine(Waiting(2.0f));
		}

	}
	public void CarSounds()
	{

		if (useSounds)
		{
			try
			{
				if (carEngineSound != null)
				{
					float engineSoundPitch = initialCarEngineSoundPitch + (Mathf.Abs(carRigidbody.velocity.magnitude) / 25f);
					carEngineSound.pitch = engineSoundPitch;
				}
				if ((isDrifting) || (isTractionLocked && Mathf.Abs(carSpeed) > 12f))
				{
					if (!tireScreechSound.isPlaying)
					{
						tireScreechSound.Play();
					}
				}
				else if ((!isDrifting) && (!isTractionLocked || Mathf.Abs(carSpeed) < 12f))
				{
					tireScreechSound.Stop();
				}
			}
			catch (Exception ex)
			{
				Debug.LogWarning(ex);
			}
		}
		else if (!useSounds)
		{
			if (carEngineSound != null && carEngineSound.isPlaying)
			{
				carEngineSound.Stop();
			}
			if (tireScreechSound != null && tireScreechSound.isPlaying)
			{
				tireScreechSound.Stop();
			}
		}

	}

	//The following method turns the front car wheels to the left. The speed of this movement will depend on the steeringSpeed variable.
	public void TurnLeft()
	{
		steeringAxis = steeringAxis - (Time.deltaTime * 10f * steeringSpeed);
		if (steeringAxis < -1f)
		{
			steeringAxis = -1f;
		}
		var steeringAngle = steeringAxis * maxSteeringAngle;
		frontLeftCollider.steerAngle = Mathf.Lerp(frontLeftCollider.steerAngle, steeringAngle, steeringSpeed);
		frontRightCollider.steerAngle = Mathf.Lerp(frontRightCollider.steerAngle, steeringAngle, steeringSpeed);
	}

	//The following method turns the front car wheels to the right. The speed of this movement will depend on the steeringSpeed variable.
	public void TurnRight()
	{
		steeringAxis = steeringAxis + (Time.deltaTime * 10f * steeringSpeed);
		if (steeringAxis > 1f)
		{
			steeringAxis = 1f;
		}
		var steeringAngle = steeringAxis * maxSteeringAngle;
		frontLeftCollider.steerAngle = Mathf.Lerp(frontLeftCollider.steerAngle, steeringAngle, steeringSpeed);
		frontRightCollider.steerAngle = Mathf.Lerp(frontRightCollider.steerAngle, steeringAngle, steeringSpeed);
	}

	//The following method takes the front car wheels to their default position (rotation = 0). The speed of this movement will depend
	// on the steeringSpeed variable.
	public void ResetSteeringAngle()
	{
		if (steeringAxis < 0f)
		{
			steeringAxis = steeringAxis + (Time.deltaTime * 10f * steeringSpeed);
		}
		else if (steeringAxis > 0f)
		{
			steeringAxis = steeringAxis - (Time.deltaTime * 10f * steeringSpeed);
		}
		if (Mathf.Abs(frontLeftCollider.steerAngle) < 1f)
		{
			steeringAxis = 0f;
		}
		var steeringAngle = steeringAxis * maxSteeringAngle;
		frontLeftCollider.steerAngle = Mathf.Lerp(frontLeftCollider.steerAngle, steeringAngle, steeringSpeed);
		frontRightCollider.steerAngle = Mathf.Lerp(frontRightCollider.steerAngle, steeringAngle, steeringSpeed);
	}

	// This method matches both the position and rotation of the WheelColliders with the WheelMeshes.
	void AnimateWheelMeshes()
	{
		Quaternion FLWRotation;
		Vector3 FLWPosition;
		frontLeftCollider.GetWorldPose(out FLWPosition, out FLWRotation);
		frontLeftMesh.transform.position = FLWPosition;
		frontLeftMesh.transform.rotation = FLWRotation;

		Quaternion FRWRotation;
		Vector3 FRWPosition;
		frontRightCollider.GetWorldPose(out FRWPosition, out FRWRotation);
		frontRightMesh.transform.position = FRWPosition;
		frontRightMesh.transform.rotation = FRWRotation;

		Quaternion RLWRotation;
		Vector3 RLWPosition;
		rearLeftCollider.GetWorldPose(out RLWPosition, out RLWRotation);
		rearLeftMesh.transform.position = RLWPosition;
		rearLeftMesh.transform.rotation = RLWRotation;

		Quaternion RRWRotation;
		Vector3 RRWPosition;
		rearRightCollider.GetWorldPose(out RRWPosition, out RRWRotation);
		rearRightMesh.transform.position = RRWPosition;
		rearRightMesh.transform.rotation = RRWRotation;
	}

	// This method apply positive torque to the wheels in order to go forward.
	public void GoForward()
	{

		if (Mathf.Abs(localVelocityX) > 2.5f)
		{
			isDrifting = true;
			DriftCarPS();
		}
		else
		{
			isDrifting = false;
			DriftCarPS();
		}
		// The following part sets the throttle power to 1 smoothly.
		throttleAxis = throttleAxis + (Time.deltaTime * 3f);
		if (throttleAxis > 1f)
		{
			throttleAxis = 1f;
		}

		if (localVelocityZ < -1f)
		{
			BrakesServerRpc();
		}
		else
		{
			if (Mathf.RoundToInt(carSpeed) < maxSpeed)
			{
				//Apply positive torque in all wheels to go forward if maxSpeed has not been reached.
				frontLeftCollider.brakeTorque = 0;
				frontLeftCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
				frontRightCollider.brakeTorque = 0;
				frontRightCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
				rearLeftCollider.brakeTorque = 0;
				rearLeftCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
				rearRightCollider.brakeTorque = 0;
				rearRightCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
			}
			else
			{

				frontLeftCollider.motorTorque = 0;
				frontRightCollider.motorTorque = 0;
				rearLeftCollider.motorTorque = 0;
				rearRightCollider.motorTorque = 0;
			}
		}
	}

	// This method apply negative torque to the wheels in order to go backwards.
	public void GoReverse()
	{

		if (Mathf.Abs(localVelocityX) > 2.5f)
		{
			isDrifting = true;
			DriftCarPS();
		}
		else
		{
			isDrifting = false;
			DriftCarPS();
		}

		throttleAxis = throttleAxis - (Time.deltaTime * 3f);
		if (throttleAxis < -1f)
		{
			throttleAxis = -1f;
		}

		if (localVelocityZ > 1f)
		{
			BrakesServerRpc();
		}
		else
		{
			if (Mathf.Abs(Mathf.RoundToInt(carSpeed)) < maxReverseSpeed)
			{
				//Apply negative torque in all wheels to go in reverse if maxReverseSpeed has not been reached.
				frontLeftCollider.brakeTorque = 0;
				frontLeftCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
				frontRightCollider.brakeTorque = 0;
				frontRightCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
				rearLeftCollider.brakeTorque = 0;
				rearLeftCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
				rearRightCollider.brakeTorque = 0;
				rearRightCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
			}
			else
			{

				frontLeftCollider.motorTorque = 0;
				frontRightCollider.motorTorque = 0;
				rearLeftCollider.motorTorque = 0;
				rearRightCollider.motorTorque = 0;
			}
		}
	}

	//The following function set the motor torque to 0 (in case the user is not pressing either W or S).
	public void ThrottleOff()
	{
		frontLeftCollider.motorTorque = 0;
		frontRightCollider.motorTorque = 0;
		rearLeftCollider.motorTorque = 0;
		rearRightCollider.motorTorque = 0;
	}

	public void DecelerateCar()
	{
		if (Mathf.Abs(localVelocityX) > 2.5f)
		{
			isDrifting = true;
			DriftCarPS();
		}
		else
		{
			isDrifting = false;
			DriftCarPS();
		}
		// The following part resets the throttle power to 0 smoothly.
		if (throttleAxis != 0f)
		{
			if (throttleAxis > 0f)
			{
				throttleAxis = throttleAxis - (Time.deltaTime * 10f);
			}
			else if (throttleAxis < 0f)
			{
				throttleAxis = throttleAxis + (Time.deltaTime * 10f);
			}
			if (Mathf.Abs(throttleAxis) < 0.15f)
			{
				throttleAxis = 0f;
			}
		}
		carRigidbody.velocity = carRigidbody.velocity * (1f / (1f + (0.025f * decelerationMultiplier)));
		// Since we want to decelerate the car, we are going to remove the torque from the wheels of the car.
		frontLeftCollider.motorTorque = 0;
		frontRightCollider.motorTorque = 0;
		rearLeftCollider.motorTorque = 0;
		rearRightCollider.motorTorque = 0;
		// If the magnitude of the car's velocity is less than 0.25f (very slow velocity), then stop the car completely and
		// also cancel the invoke of this method.
		if (carRigidbody.velocity.magnitude < 0.25f)
		{
			carRigidbody.velocity = Vector3.zero;
			CancelInvoke("DecelerateCar");
		}
	}

	// This function applies brake torque to the wheels according to the brake force given by the user.
	public void Brakes()
	{
		frontLeftCollider.brakeTorque = brakeForce;
		frontRightCollider.brakeTorque = brakeForce;
		rearLeftCollider.brakeTorque = brakeForce;
		rearRightCollider.brakeTorque = brakeForce;
	}
	public void Handbrake()
	{
		CancelInvoke("RecoverTraction");

		driftingAxis = driftingAxis + (Time.deltaTime);
		float secureStartingPoint = driftingAxis * FLWextremumSlip * handbrakeDriftMultiplier;

		if (secureStartingPoint < FLWextremumSlip)
		{
			driftingAxis = FLWextremumSlip / (FLWextremumSlip * handbrakeDriftMultiplier);
		}
		if (driftingAxis > 1f)
		{
			driftingAxis = 1f;
		}

		if (Mathf.Abs(localVelocityX) > 2.5f)
		{
			isDrifting = true;
		}
		else
		{
			isDrifting = false;
		}

		if (driftingAxis < 1f)
		{
			FLwheelFriction.extremumSlip = FLWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
			frontLeftCollider.sidewaysFriction = FLwheelFriction;

			FRwheelFriction.extremumSlip = FRWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
			frontRightCollider.sidewaysFriction = FRwheelFriction;

			RLwheelFriction.extremumSlip = RLWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
			rearLeftCollider.sidewaysFriction = RLwheelFriction;

			RRwheelFriction.extremumSlip = RRWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
			rearRightCollider.sidewaysFriction = RRwheelFriction;
		}

		// Whenever the player uses the handbrake, it means that the wheels are locked, so we set 'isTractionLocked = true'
		// and, as a consequense, the car starts to emit trails to simulate the wheel skids.
		isTractionLocked = true;
		DriftCarPS();

	}

	// This function is used to emit both the particle systems of the tires' smoke and the trail renderers of the tire skids
	// depending on the value of the bool variables 'isDrifting' and 'isTractionLocked'.
	public void DriftCarPS()
	{
		if (!IsOwner) return;
		if (isDrifting)
		{
			RRWParticleSystemServerRpc();
			RlWParticleSystemServerRpc();

		}
		else if (!isDrifting)
		{
			RRWParticleSystemStopServerRpc();
			RLWParticleSystemStopServerRpc();
		}
		if ((isTractionLocked || Mathf.Abs(localVelocityX) > 5f) && Mathf.Abs(carSpeed) > 12f)
		{
			RLWTireSkidServerRpc();
			RRWTireSkidServerRpc();
		}
		else
		{
			RLWTireSkidStopServerRpc();
			RRWTireSkidStopServerRpc();
		}
	}



	// This function is used to recover the traction of the car when the user has stopped using the car's handbrake.
	public void RecoverTraction()
	{
		isTractionLocked = false;
		driftingAxis = driftingAxis - (Time.deltaTime / 1.5f);
		if (driftingAxis < 0f)
		{
			driftingAxis = 0f;
		}

		//If the 'driftingAxis' value is not 0f, it means that the wheels have not recovered their traction.
		//We are going to continue decreasing the sideways friction of the wheels until we reach the initial
		// car's grip.
		if (FLwheelFriction.extremumSlip > FLWextremumSlip)
		{
			FLwheelFriction.extremumSlip = FLWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
			frontLeftCollider.sidewaysFriction = FLwheelFriction;

			FRwheelFriction.extremumSlip = FRWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
			frontRightCollider.sidewaysFriction = FRwheelFriction;

			RLwheelFriction.extremumSlip = RLWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
			rearLeftCollider.sidewaysFriction = RLwheelFriction;

			RRwheelFriction.extremumSlip = RRWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
			rearRightCollider.sidewaysFriction = RRwheelFriction;

			Invoke("RecoverTraction", Time.deltaTime);

		}
		else if (FLwheelFriction.extremumSlip < FLWextremumSlip)
		{
			FLwheelFriction.extremumSlip = FLWextremumSlip;
			frontLeftCollider.sidewaysFriction = FLwheelFriction;

			FRwheelFriction.extremumSlip = FRWextremumSlip;
			frontRightCollider.sidewaysFriction = FRwheelFriction;

			RLwheelFriction.extremumSlip = RLWextremumSlip;
			rearLeftCollider.sidewaysFriction = RLwheelFriction;

			RRwheelFriction.extremumSlip = RRWextremumSlip;
			rearRightCollider.sidewaysFriction = RRwheelFriction;

			driftingAxis = 0f;
		}
	}


	void ManageDrift()
	{
		driftAngle = Vector3.Angle(carRigidbody.transform.forward, (carRigidbody.velocity + carRigidbody.transform.forward).normalized);
		//Debug.Log($"IsDrifting " + isDrifting);
		//Debug.Log(carSpeed);
		if (isDrifting)
		{
			if (driftAngle > 120)
			{
				driftAngle = 0;
			}
			if (driftAngle >= minimumAngle || carSpeed > minimumSpeed)
			{
				if (isDrifting == false || stopDriftingCoroutine != null)
				{
					StartDrift();
				}
			}
			else
			{
				if (isDrifting && stopDriftingCoroutine == null)
				{
					Debug.Log("Stop drift");
					StopDrift();
					StoppingDrift();
				}
			}
			if (isDrifting)
			{

				currentScore += Time.deltaTime * driftFactor * driftAngle;
				driftFactor += Time.deltaTime;
				//Debug.Log($"currentscore: " + currentScore);
				//Debug.Log($"drifting axis: " + driftAngle);
				//Debug.Log($"Factor: " + driftFactor);

				driftingObject.SetActive(true);
			}

		}
		else if (isDrifting == false)
		{
			StoppingDrift();
			StopDrift();
		}

	}

	async void StartDrift()
	{
		if (!isDrifting)
		{
			await Task.Delay(Mathf.RoundToInt(1000 * driftingDelay));
			driftFactor = 1;
		}
		if (stopDriftingCoroutine != null)
		{
			StopCoroutine(stopDriftingCoroutine);
			stopDriftingCoroutine = null;
		}

	}
	void StopDrift()
	{
		stopDriftingCoroutine = StoppingDrift();
		StartCoroutine(stopDriftingCoroutine);

	}
	private IEnumerator StoppingDrift()
	{
		yield return new WaitForSeconds(0.1f);
		//yield return new WaitForSeconds(driftingDelay * 4f);
		totalScore += currentScore;
		//yield return new WaitForSeconds(0.5f);
		currentScore = 0;
		driftFactor = 1;
		//yield return new WaitForSeconds(2f);
		driftingObject.SetActive(false);
	}

	void ManageUI()
	{

		currentDrift.text = "Current Drift";
		if (totalScore > 0)
		{
			totalScoreText.text = "Score: " + (totalScore).ToString("###,###,000");
		}
		if (totalScore >= 2500 && gateActive == true)
		{
			gateDriftScore.SetActive(false);
			gateActive = false;
			RespawnError.text = "Brama zosta�a otwarta!";
			StartCoroutine(Waiting(2.0f));
		}
		factorText.text = driftFactor.ToString("###,###,##0.0") + "X";
		currentScoreText.text = currentScore.ToString("###,###,000");


	}

	void Speedometer()
	{
		if (speedLabel != null)
			speedLabel.text = ((int)carSpeed) + " km/h";
		if (arrow != null)
			arrow.localEulerAngles =
				new Vector3(0, 0, Mathf.Lerp(minSpeedArrowAngle, maxSpeedArrowAngle, carSpeed / maxSpeed));
	}



	//Synchronizacja hosta i clienta miedzy soba
	[ServerRpc]
	void TurnLeftServerRpc()
	{

		TurnLeftClientRpc();
	}

	[ClientRpc]
	void TurnLeftClientRpc()
	{

		TurnLeft();
	}
	[ServerRpc]
	void TurnRightServerRpc()
	{

		TurnRightClientRpc();
	}

	[ClientRpc]
	void TurnRightClientRpc()
	{

		TurnRight();
	}
	[ServerRpc]
	void AnimateServerRpc()
	{

		AnimateClientRpc();
	}

	[ClientRpc]
	void AnimateClientRpc()
	{

		AnimateWheelMeshes();
	}
	[ServerRpc]
	void ResetSteeringAngleServerRpc()
	{

		ResetSteeringAngleClientRpc();
	}

	[ClientRpc]
	void ResetSteeringAngleClientRpc()
	{

		ResetSteeringAngle();
	}
	[ServerRpc]
	void GoForwardServerRpc()
	{

		GoForwardClientRpc();
	}

	[ClientRpc]
	void GoForwardClientRpc()
	{

		GoForward();
	}
	[ServerRpc]
	void GoReverseServerRpc()
	{

		GoReverseClientRpc();
	}

	[ClientRpc]
	void GoReverseClientRpc()
	{

		GoReverse();
	}
	[ServerRpc]
	void HandbrakeServerRpc()
	{

		HandbrakeClientRpc();
	}

	[ClientRpc]
	void HandbrakeClientRpc()
	{

		Handbrake();
	}

	[ServerRpc]
	void RecoverTractionServerRpc()
	{

		RecoverTractionClientRpc();
	}

	[ClientRpc]
	void RecoverTractionClientRpc()
	{

		RecoverTraction();
	}
	[ServerRpc]
	void ThrottleOffServerRpc()
	{

		ThrottleOffClientRpc();
	}

	[ClientRpc]
	void ThrottleOffClientRpc()
	{

		ThrottleOff();
	}

	[ServerRpc]
	void BrakesServerRpc()
	{

		BrakesClientRpc();
	}

	[ClientRpc]
	void BrakesClientRpc()
	{

		Brakes();
	}
	[ServerRpc]
	void RRWParticleSystemServerRpc()
	{
		RRWParticleSystem.Play();
		RRWParticleSystemClientRpc();
	}
	[ServerRpc(RequireOwnership = false)]
	private void SetParentServerRpc(ulong playerNetworkObjectId, ulong elevatorNetworkObjectId, ServerRpcParams rpcParams = default)
	{
		var playerObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[playerNetworkObjectId];
		var elevatorObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[elevatorNetworkObjectId];
		playerObject.transform.SetParent(elevatorObject.transform);
		SetParentClientRpc(playerNetworkObjectId, elevatorNetworkObjectId);
	}

	[ClientRpc]
	private void SetParentClientRpc(ulong playerNetworkObjectId, ulong elevatorNetworkObjectId, ClientRpcParams rpcParams = default)
	{
		var playerObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[playerNetworkObjectId];
		var elevatorObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[elevatorNetworkObjectId];
		playerObject.transform.SetParent(elevatorObject.transform);
	}

	[ServerRpc(RequireOwnership = false)]
	private void ResetParentServerRpc(ulong playerNetworkObjectId, ServerRpcParams rpcParams = default)
	{
		var playerObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[playerNetworkObjectId];
		playerObject.transform.SetParent(null);
		ResetParentClientRpc(playerNetworkObjectId);
	}

	[ClientRpc]
	private void ResetParentClientRpc(ulong playerNetworkObjectId, ClientRpcParams rpcParams = default)
	{
		var playerObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[playerNetworkObjectId];
		playerObject.transform.SetParent(null);
	}
	//Prawa opona on
	[ClientRpc]
	void RRWParticleSystemClientRpc()
	{
		RRWParticleSystem.Play();
	}
	[ServerRpc]
	void RlWParticleSystemServerRpc()
	{
		RLWParticleSystem.Play();
		RLWParticleSystemClientRpc();
	}
	//Lewa opona on
	[ClientRpc]
	void RLWParticleSystemClientRpc()
	{

		RLWParticleSystem.Play();

	}
	[ServerRpc]
	void RRWParticleSystemStopServerRpc()
	{

		RRWParticleSystem.Stop();
		RRWParticleSystemStopClientRpc();

	}
	//Prawa opona off
	[ClientRpc]
	void RRWParticleSystemStopClientRpc()
	{
		RRWParticleSystem.Stop();

	}
	[ServerRpc]
	void RLWParticleSystemStopServerRpc()
	{

		RLWParticleSystem.Stop();
		RLWParticleSystemStopClientRpc();

	}
	//Lewa opona off
	[ClientRpc]
	void RLWParticleSystemStopClientRpc()
	{
		RLWParticleSystem.Stop();
	}
	[ServerRpc]
	void RLWTireSkidServerRpc()
	{
		RLWTireSkid.emitting = true;
		RLWTireSkidClientRpc();
	}
	[ClientRpc]
	void RLWTireSkidClientRpc()
	{
		RLWTireSkid.emitting = true;
	}
	[ServerRpc]
	void RRWTireSkidServerRpc()
	{
		RRWTireSkid.emitting = true;
		RRWTireSkidClientRpc();
	}
	[ClientRpc]
	void RRWTireSkidClientRpc()
	{
		RRWTireSkid.emitting = true;
	}
	[ServerRpc]
	void RRWTireSkidStopServerRpc()
	{
		RRWTireSkid.emitting = false;
		RRWTireSkidStopClientRpc();
	}
	[ClientRpc]
	void RRWTireSkidStopClientRpc()
	{
		RRWTireSkid.emitting = false;
	}
	[ServerRpc]
	void RLWTireSkidStopServerRpc()
	{
		RLWTireSkid.emitting = false;
		RLWTireSkidStopClientRpc();
	}
	[ClientRpc]
	void RLWTireSkidStopClientRpc()
	{
		RLWTireSkid.emitting = false;
	}
}