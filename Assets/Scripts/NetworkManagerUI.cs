using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NetCode : MonoBehaviour
{
    [SerializeField] private Button HostButton;
    [SerializeField] private Button ClientButton;
    public Camera BlackBackground;
	public Camera ENDCAMERA;
	private Camera hostCamera;
	private Camera clientCamera;
	private float cameraSpeed = 55.0f;
	private bool Ending = false;
	private Quaternion cameraRotation;
	private bool Host = false;
	private bool Client = false;
	private string localIP;
	public TMP_Text IP;
	public TMP_InputField connectionIP;
	public TMP_Text error;
	private string HostIP;
	public RectTransform speedUI;
	public Timer _timer;
	private int StarsLeftThis;
	private float TimeLeftFromTimer;
	[Space(10)]
	[Header("Je¿eli true to nie trzeba wpisywaæ adresu przy ³¹czeniu clienta z hostem")]
	public bool FastDebugowanie = false;
	[Space(10)]
	[Header("Je¿eli true to po do³¹czeniu do gry jest animacja kamery do gracza")]
	public bool Animate = false;
	[Space(10)]
	[Header("Elementy które znikaj¹ i pojawiaj¹ siê na koñcu gry")]
	public GameObject SpeedTextUI;
	public GameObject CollectiblesUI;
	public GameObject DriftUI;
	public GameObject TimerUI;
	//Do pojawienia
	public GameObject EndGameScreen;
	public TMP_Text TimeLeft;
	public GameObject Stars;
	//Gwiazdki
	[Header("Z³ote gwiazdki")]
	public GameObject GoldStar1;
	public GameObject GoldStar2;
	public GameObject GoldStar3;
	public GameObject GoldStar4;
	public GameObject GoldStar5;
	[Header("Szare gwiazdki")]
	public GameObject GreyStar1;
	public GameObject GreyStar2;
	public GameObject GreyStar3;
	public GameObject GreyStar4;
	public GameObject GreyStar5;

	private void Start()
	{
		//Przypisanie elementu który ma skrypt Timer
		GameObject scripttimer = GameObject.Find("StartTimer");
		_timer = scripttimer.GetComponent<Timer>();

		speedUI.localScale = new Vector3(0.01f, 0.01f, 0.01f);
		IPHostEntry ipAddress = Dns.GetHostEntry(Dns.GetHostName());
		foreach(IPAddress ip in ipAddress.AddressList)
		{
			if(ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
			{
				if(IsRealIp(ip.ToString()))
				{
					localIP=ip.ToString();
					break;
				}
			}
		}
		IP.text = "IP: " + localIP;
	}
	bool IsRealIp(string ipAddress)
	{
		try
		{
			using (var ping = new System.Net.NetworkInformation.Ping())
			{
				var reply = ping.Send(ipAddress);
				return reply.Status == IPStatus.Success;
			}
		} catch
		{
			return false;
		}
		
	}
	private void Awake()
    {
        
        HostButton.onClick.AddListener(() =>
        {
			NetworkManager.Singleton.StartHost();
			Host = true;
			Dosomething();

		});
        ClientButton.onClick.AddListener(() =>
        {
			if(FastDebugowanie == true)
			{
				NetworkManager.Singleton.StartClient();
				NetworkManager.Singleton.OnClientConnectedCallback += ClientConnected;
			} else
			{
				HostIP = connectionIP.text;
				if (IsRealIp(HostIP.ToString()))
				{
					Debug.Log(HostIP.ToString());
					UnityTransport unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
					unityTransport.SetConnectionData(HostIP.ToString(), 7777);
					NetworkManager.Singleton.StartClient();
					NetworkManager.Singleton.OnClientConnectedCallback += ClientConnected;
				}
				else
				{
					error.text = "Nie znaleziono gry";
				}
			}
			
		});
        
    }
	void ClientConnected(ulong ClientId)
	{
		if (NetworkManager.Singleton.IsClient && NetworkManager.Singleton.LocalClientId == ClientId)
		{
			Client = true;
			Dosomething();
		}
	}
	private void Dosomething()
	{
		HostButton.gameObject.SetActive(false);
	    ClientButton.gameObject.SetActive(false);
		IP.gameObject.SetActive(false);
		error.gameObject.SetActive(false);
		connectionIP.gameObject.SetActive(false);
		if (NetworkManager.Singleton.IsHost)
		{
			GameObject host = GameObject.FindWithTag("HostPlayer");
			Rigidbody hostrb = host.GetComponent<Rigidbody>();
			hostrb.velocity = new Vector3(60, 0, 0);
			hostCamera = host.GetComponentInChildren<Camera>();
		} else
		{
			GameObject client = GameObject.FindWithTag("ClientPlayer");
			Rigidbody clientrb = client.GetComponent<Rigidbody>();
			clientrb.velocity = new Vector3(60, 0, 0);
			clientCamera = client.GetComponentInChildren<Camera>();
		}
        StartCoroutine(ExecuteAfterTime(1.2f));
	}
	IEnumerator ExecuteAfterTime(float time)
	{
		yield return new WaitForSeconds(time);
		cameraRotation = Quaternion.Euler(14, 90, 0);
		if(Animate == true)
		{
			Ending = true;
		} else
		{
			BlackBackground.enabled = false;
			speedUI.localScale = new Vector3(1f, 1f, 1f);
		}

	}
	private void Update()
	{
		if (Ending && Host)
		{
			MoveAndRotateCamera(hostCamera);
		}
		if (Ending && Client)
		{
			MoveAndRotateCamera(clientCamera);
		}
	}

	private void MoveAndRotateCamera(Camera targetCamera)
	{
		BlackBackground.transform.position = Vector3.MoveTowards(BlackBackground.transform.position,
			targetCamera.transform.position, cameraSpeed * Time.deltaTime);

		BlackBackground.transform.rotation = Quaternion.Lerp(BlackBackground.transform.rotation, cameraRotation, Time.deltaTime * 2);

		if (Vector3.Distance(BlackBackground.transform.position, targetCamera.transform.position) < 0.01f &&
			Quaternion.Angle(BlackBackground.transform.rotation, cameraRotation) < 0.01f)
		{
			BlackBackground.enabled = false;
			speedUI.localScale = new Vector3(1f, 1f, 1f);
			Ending = false;
		}
	}
	public void EndingGame()
	{
		ENDCAMERA.enabled = true;
		SpeedTextUI.SetActive(false);
		CollectiblesUI.SetActive(false);
		DriftUI.SetActive(false);
		TimerUI.SetActive(false);
		StarsLeftThis = _timer.StarsLeft;
		
		//int min = Mathf.FloorToInt(TimeLeftFromTimer / 60);
		//int sec = Mathf.FloorToInt(TimeLeftFromTimer % 60);
		TimeLeft.text = "Zdobyte gwiazdki:";
		EndGameScreen.SetActive(true);
		//Gwiazdki
		Stars.SetActive(true);
		switch (StarsLeftThis)
		{
			case 5:
				GoldStar1.SetActive(true);
				GoldStar2.SetActive(true);
				GoldStar3.SetActive(true);
				GoldStar4.SetActive(true);
				GoldStar5.SetActive(true);
				break;
			case 4:
				GoldStar1.SetActive(true);
				GoldStar2.SetActive(true);
				GoldStar3.SetActive(true);
				GoldStar4.SetActive(true);
				GreyStar5.SetActive(true);
				break;
			case 3:
				GoldStar1.SetActive(true);
				GoldStar2.SetActive(true);
				GoldStar3.SetActive(true);
				GreyStar4.SetActive(true);
				GreyStar5.SetActive(true);
				break;
			case 2:
				GoldStar1.SetActive(true);
				GoldStar2.SetActive(true);
				GreyStar3.SetActive(true);
				GreyStar4.SetActive(true);
				GreyStar5.SetActive(true);
				break;
			case 1:
				GoldStar1.SetActive(true);
				GreyStar2.SetActive(true);
				GreyStar3.SetActive(true);
				GreyStar4.SetActive(true);
				GreyStar5.SetActive(true);
				break;
			case 0:
				GreyStar1.SetActive(true);
				GreyStar2.SetActive(true);
				GreyStar3.SetActive(true);
				GreyStar4.SetActive(true);
				GreyStar5.SetActive(true);
				break;
		}
	}
}
