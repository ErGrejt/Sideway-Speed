using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;

public class GameManagerScript : NetworkBehaviour
{
    public int ScoreCoins = 0;
    public GameObject Gate;
    private TMP_Text scoreText;
    private bool EndGame = false;
    private NetCode NetworkManagerUI;

    private void Start()
    {
        scoreText = GameObject.Find("CollectiblesText").GetComponent<TMP_Text>();
        GameObject script = GameObject.Find("NetworkManagerUI");
        NetworkManagerUI = script.GetComponent<NetCode>();
    }
    public void AddCoin()
    {
        ScoreCoins++;
        Debug.Log("Dodanie punktu" + ScoreCoins);
        if(ScoreCoins == 10)
        {
            Gate.SetActive(false);
        }
        if(ScoreCoins == 11)
        {
            NetworkManagerUI.EndingGame();
        }
        scoreText.text = "Zebrane punkty: " + ScoreCoins;
    }
}
