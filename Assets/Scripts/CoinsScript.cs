using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CoinsScript : NetworkBehaviour
{
    private bool isCollected = false;
   
    private void OnTriggerEnter(Collider other)
    {
        if (!isCollected) {
            isCollected = true;
            GameManagerScript gameManager = FindObjectOfType<GameManagerScript>();
            if (gameManager != null)
            {
                gameManager.AddCoin();
            }
            else
            {
                Debug.LogError("Nie mo¿na znaleŸæ GameManagerScript w scenie");
            }
            Destroy(this.gameObject);
        }
    }
}
