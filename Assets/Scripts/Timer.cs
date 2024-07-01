using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    private TextMeshProUGUI timerText; // Referencja do komponentu tekstowego, który wyœwietla czas
    private float elapsedTime = 0f;
    public float elapsedTimeStars;
    public int StarsLeft = 5;
    private bool isTiming = false; // Flaga okreœlaj¹ca, czy timer dzia³a


    //Boole 3 ¿ó³tych gwiazdek
    private bool FirstStar = true;
    private bool SecondStar = true;
    private bool ThirdStar = true;
    private bool FourthStar = true;
    private bool FifthStar = true;

    //GameObject gwiazdek
    private GameObject FirstStarGO;
    private GameObject SecondStarGO;
    private GameObject ThirdStarGO;
    private GameObject FourthStarGO;
    private GameObject FifthStarGO;



    private GameObject FirstStarGRAY ;
    private GameObject SecondStarGRAY;
    private GameObject ThirdStarGRAY;
    private GameObject FourthStarGRAY;
    private GameObject FifthStarGRAY;

    void Start()
    {
        // Znajdowanie komponentów tekstowych w scenie
        GameObject timerObject = GameObject.Find("TimerText");
        timerText = timerObject.GetComponent<TextMeshProUGUI>();


        FirstStarGO = GameObject.Find("GoldStar3");
        SecondStarGO = GameObject.Find("GoldStar2");
        ThirdStarGO = GameObject.Find("GoldStar1");
        FourthStarGO = GameObject.Find("GoldStar4");
        FifthStarGO = GameObject.Find("GoldStar5");
        

        FirstStarGRAY = GameObject.Find("GreyStar3");
        SecondStarGRAY = GameObject.Find("GreyStar2");
        ThirdStarGRAY = GameObject.Find("GreyStar1");
        FourthStarGRAY = GameObject.Find("GreyStar4");
        FifthStarGRAY = GameObject.Find("GreyStar5");

        //znikanie gwiazdek

        FirstStarGO.transform.localScale = new Vector3(0.001f,0.001f);
        SecondStarGO.transform.localScale = new Vector3(0.001f,0.001f);
        ThirdStarGO.transform.localScale = new Vector3(0.001f,0.001f);
        FourthStarGO.transform.localScale = new Vector3(0.001f,0.001f);
        FifthStarGO.transform.localScale = new Vector3(0.001f,0.001f);

        FirstStarGRAY.transform.localScale =new Vector3(0.001f,0.001f);
        SecondStarGRAY.transform.localScale= new Vector3(0.001f,0.001f);
        ThirdStarGRAY.transform.localScale =new Vector3(0.001f,0.001f);
        FourthStarGRAY.transform.localScale =new Vector3(0.001f,0.001f);
        FifthStarGRAY.transform.localScale =new Vector3(0.001f,0.001f);

    }

    void Update()
    {
        if (isTiming)
        {
            elapsedTime += Time.deltaTime;
            elapsedTimeStars += Time.deltaTime;
            int minutes = Mathf.FloorToInt(elapsedTime / 60);
            int seconds = Mathf.FloorToInt(elapsedTime % 60);
            timerText.text = string.Format("Time: {0:00}:{1:00}", minutes, seconds);

            FirstStarGO.transform.localScale = new Vector3(0.8f, 0.8f);
            SecondStarGO.transform.localScale = new Vector3(0.8f, 0.8f);
            ThirdStarGO.transform.localScale = new Vector3(0.8f, 0.8f);
            FourthStarGO.transform.localScale = new Vector3(0.8f, 0.8f);
            FifthStarGO.transform.localScale = new Vector3(0.8f, 0.8f);

            FirstStarGRAY.transform.localScale = new Vector3(0.8f, 0.8f);
            SecondStarGRAY.transform.localScale = new Vector3(0.8f, 0.8f);
            ThirdStarGRAY.transform.localScale = new Vector3(0.8f, 0.8f);
            FourthStarGRAY.transform.localScale = new Vector3(0.8f, 0.8f);
            FifthStarGRAY.transform.localScale = new Vector3(0.8f, 0.8f);

           }
        //Znikniêcie 5 gwiazdki
        if (elapsedTimeStars > 300  && FifthStar)
        {
            FifthStarGO.SetActive(false);
            FifthStar = false;
            StarsLeft = 4;
			Debug.Log("StarsLeft updated to: " + StarsLeft);

		}
        //Znikniêcie 4 gwiazdki
        if (elapsedTimeStars > 420 && FourthStar)
        {
            FourthStarGO.SetActive(false);
            FourthStar = false;
			StarsLeft = 3;
		}
        //Znikniêcie 3 gwiazdki
        if (elapsedTimeStars > 540 && FirstStar)
        {
            FirstStarGO.SetActive(false);
            FirstStar = false;
			StarsLeft = 2;
		}
        //Znikniêcie 2 gwiazdki
        if (elapsedTimeStars > 600 && SecondStar)
        {
            SecondStarGO.SetActive(false);
            SecondStar = false;
			StarsLeft = 1;
		}
        //Znikniêcie 1 gwiazdki
        if (elapsedTimeStars > 720 && ThirdStar)
        {
            ThirdStarGO.SetActive(false);
            ThirdStar = false;
			StarsLeft = 0;
		}
    }

    private void OnTriggerEnter(Collider other)
    {
            StartTimer();
      
    }

    private void StartTimer()
    {
        elapsedTime = 0f;
        isTiming = true;
    }

    private void StopAndResetTimer()
    {
        isTiming = false;
        int minutes = Mathf.FloorToInt(elapsedTime / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);
        timerText.text = string.Format("Time: {0:00}:{1:00}", minutes, seconds);
        elapsedTime = 0f;
    }
}
