using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Gate : NetworkBehaviour
{
    public Transform Brama;
    public float GateSpeed = 2.0f;
    public float GateHeight = 8.0f;
    private bool isMovingUp = false;
    private bool isMovingDown = false;
    private Vector3 targetPostionUp;
    private Vector3 targetPostionDown;
    private Vector3 originalposition;
    public void Awake()
    {
        originalposition = Brama.position;
    }
    private void Update()
    {
        if(isMovingUp)
        {
            float step = GateSpeed * Time.deltaTime;
            Brama.position = Vector3.MoveTowards(Brama.position, targetPostionUp, step);
            if (Vector3.Distance(Brama.position, targetPostionUp) < 0.1f || isMovingDown == true)
            {
                isMovingUp = false;
            }
        }
        if(isMovingDown)
        {
            float step = GateSpeed * Time.deltaTime;
            Brama.position = Vector3.MoveTowards(Brama.position, targetPostionDown, step);
            if (Vector3.Distance(Brama.position, targetPostionDown) < 0.1f || isMovingUp == true)
            {
                isMovingDown = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Guzik ON");
        if(isMovingDown)
        {
            isMovingDown = false;
            isMovingUp = true;
        }
        LiftGate();
    }
    private void LiftGate()
    {
        targetPostionUp = Brama.position + Vector3.back * GateHeight;
        isMovingUp = true;
    }
    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Guzik OFF");
        isMovingUp = false;
        isMovingDown = true;
        targetPostionDown = originalposition;
    }

}
